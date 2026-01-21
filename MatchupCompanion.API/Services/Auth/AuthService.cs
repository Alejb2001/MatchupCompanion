using MatchupCompanion.API.Data;
using MatchupCompanion.API.Models.DTOs.Auth;
using MatchupCompanion.API.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MatchupCompanion.API.Services.Auth;

/// <summary>
/// Servicio de autenticación y gestión de usuarios
/// </summary>
public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _configuration;
    private readonly ApplicationDbContext _context;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        RoleManager<IdentityRole> roleManager,
        IConfiguration configuration,
        ApplicationDbContext context)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _configuration = configuration;
        _context = context;
    }

    /// <summary>
    /// Registra un nuevo usuario
    /// </summary>
    public async Task<AuthResponse?> RegisterAsync(RegisterRequest request)
    {
        // Verificar si el email ya existe
        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
        {
            return null; // Email ya existe
        }

        // Crear nuevo usuario
        var user = new ApplicationUser
        {
            UserName = request.UserName,
            Email = request.Email,
            DisplayName = request.DisplayName ?? request.UserName,
            PreferredRoleId = request.PreferredRoleId,
            CreatedAt = DateTime.UtcNow,
            IsGuest = false
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            return null; // Error al crear usuario
        }

        // Generar token JWT
        return await GenerateAuthResponse(user);
    }

    /// <summary>
    /// Inicia sesión de un usuario
    /// </summary>
    public async Task<AuthResponse?> LoginAsync(LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            return null; // Usuario no encontrado
        }

        // Verificar si es un invitado expirado
        if (user.IsGuest && user.GuestExpiresAt.HasValue && user.GuestExpiresAt.Value < DateTime.UtcNow)
        {
            return null; // Invitado expirado
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: false);
        if (!result.Succeeded)
        {
            return null; // Contraseña incorrecta
        }

        // Actualizar último login
        user.LastLoginAt = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);

        // Generar token JWT
        return await GenerateAuthResponse(user);
    }

    /// <summary>
    /// Renueva el token de acceso (implementación básica)
    /// </summary>
    public async Task<AuthResponse?> RefreshTokenAsync(RefreshTokenRequest request)
    {
        // Nota: Esta es una implementación simplificada
        // En producción deberías almacenar y validar refresh tokens en la BD
        var principal = GetPrincipalFromExpiredToken(request.Token);
        if (principal == null)
        {
            return null;
        }

        var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return null;
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return null;
        }

        return await GenerateAuthResponse(user);
    }

    /// <summary>
    /// Obtiene información del usuario actual
    /// </summary>
    public async Task<UserDto?> GetCurrentUserAsync(string userId)
    {
        var user = await _context.Users
            .Include(u => u.PreferredRole)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            return null;
        }

        var roles = await _userManager.GetRolesAsync(user);

        return new UserDto
        {
            Id = user.Id,
            Email = user.Email ?? string.Empty,
            UserName = user.UserName ?? string.Empty,
            DisplayName = user.DisplayName,
            PreferredRoleId = user.PreferredRoleId,
            PreferredRoleName = user.PreferredRole?.Name,
            IsGuest = user.IsGuest,
            CreatedAt = user.CreatedAt,
            Roles = roles.ToList()
        };
    }

    /// <summary>
    /// Crea una sesión de invitado temporal (válida por 24 horas)
    /// </summary>
    public async Task<AuthResponse?> CreateGuestSessionAsync()
    {
        var guestId = Guid.NewGuid().ToString();
        var user = new ApplicationUser
        {
            UserName = $"Guest_{guestId.Substring(0, 8)}",
            Email = $"guest_{guestId}@matchupcompanion.local",
            DisplayName = "Invitado",
            IsGuest = true,
            GuestExpiresAt = DateTime.UtcNow.AddHours(24),
            CreatedAt = DateTime.UtcNow,
            EmailConfirmed = true // Los invitados no requieren confirmación
        };

        // Crear usuario invitado con contraseña temporal
        var result = await _userManager.CreateAsync(user, $"Guest_{Guid.NewGuid()}");
        if (!result.Succeeded)
        {
            return null;
        }

        return await GenerateAuthResponse(user);
    }

    /// <summary>
    /// Verifica si un usuario puede editar un matchup (solo si lo creó)
    /// </summary>
    public async Task<bool> CanEditMatchupAsync(string userId, int matchupId)
    {
        var matchup = await _context.Matchups.FindAsync(matchupId);
        if (matchup == null)
        {
            return false;
        }

        // Solo el creador puede editar
        return matchup.CreatedById == userId;
    }

    public async Task<ApplicationUser?> GetUserByIdAsync(string userId)
    {
        return await _userManager.FindByIdAsync(userId);
    }

    public async Task AddUserToRoleAsync(ApplicationUser user, string roleName)
    {
        // Crear el rol si no existe
        if (!await _roleManager.RoleExistsAsync(roleName))
        {
            await _roleManager.CreateAsync(new IdentityRole(roleName));
        }

        // Añadir usuario al rol
        if (!await _userManager.IsInRoleAsync(user, roleName))
        {
            await _userManager.AddToRoleAsync(user, roleName);
        }
    }

    /// <summary>
    /// Genera la respuesta de autenticación con token JWT
    /// </summary>
    private async Task<AuthResponse> GenerateAuthResponse(ApplicationUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        var token = GenerateJwtToken(user, roles);

        var expirationMinutes = _configuration.GetValue<int>("Jwt:ExpirationInMinutes");
        var expiresAt = DateTime.UtcNow.AddMinutes(expirationMinutes);

        return new AuthResponse
        {
            Token = token,
            ExpiresAt = expiresAt,
            UserId = user.Id,
            Email = user.Email ?? string.Empty,
            UserName = user.UserName ?? string.Empty,
            DisplayName = user.DisplayName,
            IsGuest = user.IsGuest,
            Roles = roles.ToList()
        };
    }

    /// <summary>
    /// Genera un token JWT para el usuario
    /// </summary>
    private string GenerateJwtToken(ApplicationUser user, IList<string> roles)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
            new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
            new Claim("DisplayName", user.DisplayName ?? string.Empty),
            new Claim("IsGuest", user.IsGuest.ToString())
        };

        // Agregar roles como claims
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"] ?? throw new InvalidOperationException("JWT Secret Key not configured")));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expirationMinutes = _configuration.GetValue<int>("Jwt:ExpirationInMinutes");

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// Obtiene el principal de un token expirado (para refresh)
    /// </summary>
    private ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"] ?? throw new InvalidOperationException("JWT Secret Key not configured"))),
            ValidateLifetime = false, // No validar expiración
            ValidIssuer = _configuration["Jwt:Issuer"],
            ValidAudience = _configuration["Jwt:Audience"]
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                return null;
            }

            return principal;
        }
        catch
        {
            return null;
        }
    }
}
