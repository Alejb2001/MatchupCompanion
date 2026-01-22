using System.ComponentModel.DataAnnotations;

namespace MatchupCompanion.API.Models.DTOs.Auth;

/// <summary>
/// DTO para solicitud de registro de nuevo usuario
/// </summary>
public class RegisterRequest
{
    /// <summary>
    /// Email del usuario
    /// </summary>
    [Required(ErrorMessage = "El email es requerido")]
    [EmailAddress(ErrorMessage = "El formato del email no es válido")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Nombre de usuario
    /// </summary>
    [Required(ErrorMessage = "El nombre de usuario es requerido")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "El nombre de usuario debe tener entre 3 y 50 caracteres")]
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// Nombre para mostrar (opcional)
    /// </summary>
    [StringLength(100, ErrorMessage = "El nombre para mostrar no puede exceder 100 caracteres")]
    public string? DisplayName { get; set; }

    /// <summary>
    /// Contraseña
    /// </summary>
    [Required(ErrorMessage = "La contraseña es requerida")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Confirmación de contraseña
    /// </summary>
    [Required(ErrorMessage = "La confirmación de contraseña es requerida")]
    [Compare("Password", ErrorMessage = "Las contraseñas no coinciden")]
    public string ConfirmPassword { get; set; } = string.Empty;

    /// <summary>
    /// ID del rol preferido del jugador (opcional)
    /// </summary>
    public int? PreferredRoleId { get; set; }
}
