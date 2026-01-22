using System.ComponentModel.DataAnnotations;

namespace MatchupCompanion.API.Models.DTOs.Auth;

/// <summary>
/// DTO para solicitud de renovación de token
/// </summary>
public class RefreshTokenRequest
{
    /// <summary>
    /// Token de acceso expirado
    /// </summary>
    [Required(ErrorMessage = "El token es requerido")]
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// Token de refresco
    /// </summary>
    [Required(ErrorMessage = "El refresh token es requerido")]
    public string RefreshToken { get; set; } = string.Empty;
}
