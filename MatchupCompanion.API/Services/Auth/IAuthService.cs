using MatchupCompanion.API.Models.DTOs.Auth;

namespace MatchupCompanion.API.Services.Auth;

/// <summary>
/// Interfaz para servicio de autenticación
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Registra un nuevo usuario
    /// </summary>
    Task<AuthResponse?> RegisterAsync(RegisterRequest request);

    /// <summary>
    /// Inicia sesión de un usuario
    /// </summary>
    Task<AuthResponse?> LoginAsync(LoginRequest request);

    /// <summary>
    /// Renueva el token de acceso usando un refresh token
    /// </summary>
    Task<AuthResponse?> RefreshTokenAsync(RefreshTokenRequest request);

    /// <summary>
    /// Obtiene información del usuario actual
    /// </summary>
    Task<UserDto?> GetCurrentUserAsync(string userId);

    /// <summary>
    /// Crea una sesión de invitado temporal
    /// </summary>
    Task<AuthResponse?> CreateGuestSessionAsync();

    /// <summary>
    /// Verifica si un usuario tiene permisos para editar un matchup
    /// </summary>
    Task<bool> CanEditMatchupAsync(string userId, int matchupId);
}
