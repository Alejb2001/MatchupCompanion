using System.ComponentModel.DataAnnotations;

namespace MatchupCompanion.API.Models.DTOs.Auth;

/// <summary>
/// DTO para solicitud de inicio de sesión
/// </summary>
public class LoginRequest
{
    /// <summary>
    /// Email del usuario
    /// </summary>
    [Required(ErrorMessage = "El email es requerido")]
    [EmailAddress(ErrorMessage = "El formato del email no es válido")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Contraseña del usuario
    /// </summary>
    [Required(ErrorMessage = "La contraseña es requerida")]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Mantener sesión iniciada (refresh token de larga duración)
    /// </summary>
    public bool RememberMe { get; set; } = false;
}
