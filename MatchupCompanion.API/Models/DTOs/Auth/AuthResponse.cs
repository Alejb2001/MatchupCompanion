namespace MatchupCompanion.API.Models.DTOs.Auth;

/// <summary>
/// DTO para respuesta de autenticación exitosa
/// </summary>
public class AuthResponse
{
    /// <summary>
    /// Token JWT de acceso
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// Token de refresco (para renovar el token de acceso)
    /// </summary>
    public string? RefreshToken { get; set; }

    /// <summary>
    /// Fecha de expiración del token
    /// </summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// ID del usuario
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Email del usuario
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Nombre de usuario
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// Nombre para mostrar
    /// </summary>
    public string? DisplayName { get; set; }

    /// <summary>
    /// Indica si es un usuario invitado
    /// </summary>
    public bool IsGuest { get; set; }

    /// <summary>
    /// Roles del usuario
    /// </summary>
    public List<string> Roles { get; set; } = new();
}
