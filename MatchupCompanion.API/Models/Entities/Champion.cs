using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MatchupCompanion.API.Models.Entities;

/// <summary>
/// Representa un campeón de League of Legends
/// </summary>
public class Champion
{
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// ID del campeón en la API de Riot Games
    /// </summary>
    [Required]
    public string RiotChampionId { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? Title { get; set; }

    [MaxLength(500)]
    public string? ImageUrl { get; set; }

    [MaxLength(1000)]
    public string? Description { get; set; }

    // Rol principal del campeón
    [ForeignKey(nameof(Role))]
    public int? PrimaryRoleId { get; set; }
    public Role? PrimaryRole { get; set; }

    // Habilidades del campeón (Q, W, E, R)
    [MaxLength(100)]
    public string? QSpellId { get; set; }
    [MaxLength(200)]
    public string? QSpellName { get; set; }
    [MaxLength(500)]
    public string? QSpellIcon { get; set; }

    [MaxLength(100)]
    public string? WSpellId { get; set; }
    [MaxLength(200)]
    public string? WSpellName { get; set; }
    [MaxLength(500)]
    public string? WSpellIcon { get; set; }

    [MaxLength(100)]
    public string? ESpellId { get; set; }
    [MaxLength(200)]
    public string? ESpellName { get; set; }
    [MaxLength(500)]
    public string? ESpellIcon { get; set; }

    [MaxLength(100)]
    public string? RSpellId { get; set; }
    [MaxLength(200)]
    public string? RSpellName { get; set; }
    [MaxLength(500)]
    public string? RSpellIcon { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Relaciones
    public ICollection<Matchup> MatchupsAsPlayerChampion { get; set; } = new List<Matchup>();
    public ICollection<Matchup> MatchupsAsEnemyChampion { get; set; } = new List<Matchup>();
}
