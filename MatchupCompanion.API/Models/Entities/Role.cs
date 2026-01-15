using System.ComponentModel.DataAnnotations;

namespace MatchupCompanion.API.Models.Entities;

/// <summary>
/// Representa un rol o línea en League of Legends (Top, Jungle, Mid, ADC, Support)
/// </summary>
public class Role
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? Description { get; set; }

    // Relaciones
    public ICollection<Champion> Champions { get; set; } = new List<Champion>();
    public ICollection<Matchup> MatchupsAsPlayerRole { get; set; } = new List<Matchup>();
}
