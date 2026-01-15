using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MatchupCompanion.API.Models.Entities;

/// <summary>
/// Representa un consejo específico para un matchup
/// </summary>
public class MatchupTip
{
    [Key]
    public int Id { get; set; }

    [ForeignKey(nameof(Matchup))]
    [Required]
    public int MatchupId { get; set; }
    public Matchup Matchup { get; set; } = null!;

    /// <summary>
    /// Categoría del tip: EarlyGame, MidGame, LateGame, Items, Runes, Abilities
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Category { get; set; } = string.Empty;

    [Required]
    [MaxLength(2000)]
    public string Content { get; set; } = string.Empty;

    [Range(1, 10)]
    public int Priority { get; set; } = 5;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Información del autor (para futuras extensiones con autenticación)
    [MaxLength(100)]
    public string? AuthorName { get; set; }
}
