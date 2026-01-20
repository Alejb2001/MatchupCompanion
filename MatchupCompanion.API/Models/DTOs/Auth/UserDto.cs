namespace MatchupCompanion.API.Models.DTOs.Auth;

/// <summary>
/// DTO para información de usuario
/// </summary>
public class UserDto
{
    /// <summary>
    /// ID del usuario
    /// </summary>
    public string Id { get; set; } = string.Empty;

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
    /// ID del rol preferido
    /// </summary>
    public int? PreferredRoleId { get; set; }

    /// <summary>
    /// Nombre del rol preferido
    /// </summary>
    public string? PreferredRoleName { get; set; }

    /// <summary>
    /// Indica si es un usuario invitado
    /// </summary>
    public bool IsGuest { get; set; }

    /// <summary>
    /// Fecha de creación de la cuenta
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Roles del usuario
    /// </summary>
    public List<string> Roles { get; set; } = new();
}
