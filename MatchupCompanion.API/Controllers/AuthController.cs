using MatchupCompanion.API.Models.DTOs.Auth;
using MatchupCompanion.API.Services.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MatchupCompanion.API.Controllers;

/// <summary>
/// Controlador para autenticación y gestión de usuarios
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Registra un nuevo usuario
    /// </summary>
    /// <param name="request">Datos de registro</param>
    /// <returns>Token de autenticación y datos del usuario</returns>
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _authService.RegisterAsync(request);
        if (result == null)
        {
            return BadRequest(new { message = "No se pudo registrar el usuario. El email o nombre de usuario ya están en uso." });
        }

        _logger.LogInformation("Usuario registrado: {Email}", request.Email);
        return CreatedAtAction(nameof(GetCurrentUser), new { id = result.UserId }, result);
    }

    /// <summary>
    /// Inicia sesión con email y contraseña
    /// </summary>
    /// <param name="request">Credenciales de inicio de sesión</param>
    /// <returns>Token de autenticación y datos del usuario</returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _authService.LoginAsync(request);
        if (result == null)
        {
            return Unauthorized(new { message = "Email o contraseña incorrectos." });
        }

        _logger.LogInformation("Usuario inició sesión: {Email}", request.Email);
        return Ok(result);
    }

    /// <summary>
    /// Crea una sesión de invitado temporal
    /// </summary>
    /// <returns>Token de autenticación para invitado</returns>
    [HttpPost("guest")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateGuestSession()
    {
        var result = await _authService.CreateGuestSessionAsync();
        if (result == null)
        {
            return StatusCode(500, new { message = "No se pudo crear la sesión de invitado." });
        }

        _logger.LogInformation("Sesión de invitado creada: {UserId}", result.UserId);
        return Ok(result);
    }

    /// <summary>
    /// Renueva el token de acceso usando un refresh token
    /// </summary>
    /// <param name="request">Token y refresh token</param>
    /// <returns>Nuevo token de autenticación</returns>
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _authService.RefreshTokenAsync(request);
        if (result == null)
        {
            return Unauthorized(new { message = "Token inválido o expirado." });
        }

        return Ok(result);
    }

    /// <summary>
    /// Obtiene información del usuario actual
    /// </summary>
    /// <returns>Datos del usuario autenticado</returns>
    [Authorize]
    [HttpGet("me")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var user = await _authService.GetCurrentUserAsync(userId);
        if (user == null)
        {
            return NotFound(new { message = "Usuario no encontrado." });
        }

        return Ok(user);
    }

    /// <summary>
    /// Cierra sesión (cliente debe eliminar el token)
    /// </summary>
    /// <returns>Confirmación de cierre de sesión</returns>
    [Authorize]
    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Logout()
    {
        // En JWT, el logout se maneja en el cliente eliminando el token
        // Aquí solo confirmamos la acción
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        _logger.LogInformation("Usuario cerró sesión: {UserId}", userId);

        return Ok(new { message = "Sesión cerrada exitosamente." });
    }

    /// <summary>
    /// Verifica si el token actual es válido
    /// </summary>
    /// <returns>Estado de validez del token</returns>
    [Authorize]
    [HttpGet("validate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult ValidateToken()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        var isGuest = User.FindFirst("IsGuest")?.Value == "True";

        return Ok(new
        {
            isValid = true,
            userId,
            email,
            isGuest
        });
    }
}
