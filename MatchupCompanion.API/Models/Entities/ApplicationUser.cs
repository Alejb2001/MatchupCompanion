using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace MatchupCompanion.API.Models.Entities;

/// <summary>
/// Usuario de la aplicación, extiende IdentityUser con campos adicionales
/// </summary>
public class ApplicationUser : IdentityUser
{
    /// <summary>
    /// Nombre para mostrar del usuario
    /// </summary>
    [MaxLength(100)]
    public string? DisplayName { get; set; }

    /// <summary>
    /// Rol preferido del jugador (Top, Jungle, Mid, ADC, Support)
    /// </summary>
    public int? PreferredRoleId { get; set; }
    public Role? PreferredRole { get; set; }

    /// <summary>
    /// Fecha de creación de la cuenta
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Última vez que el usuario inició sesión
    /// </summary>
    public DateTime? LastLoginAt { get; set; }

    /// <summary>
    /// Indica si es un usuario invitado temporal
    /// </summary>
    public bool IsGuest { get; set; } = false;

    /// <summary>
    /// Fecha de expiración para usuarios invitados
    /// </summary>
    public DateTime? GuestExpiresAt { get; set; }

    // Relaciones
    public ICollection<Matchup> CreatedMatchups { get; set; } = new List<Matchup>();
    public ICollection<MatchupTip> CreatedTips { get; set; } = new List<MatchupTip>();
}
