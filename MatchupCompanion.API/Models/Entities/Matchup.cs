using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MatchupCompanion.API.Models.Entities;

/// <summary>
/// Representa un enfrentamiento entre dos campeones en un rol específico
/// </summary>
public class Matchup
{
    [Key]
    public int Id { get; set; }

    // Campeón del jugador
    [ForeignKey(nameof(PlayerChampion))]
    [Required]
    public int PlayerChampionId { get; set; }
    public Champion PlayerChampion { get; set; } = null!;

    // Campeón enemigo
    [ForeignKey(nameof(EnemyChampion))]
    [Required]
    public int EnemyChampionId { get; set; }
    public Champion EnemyChampion { get; set; } = null!;

    // Rol/Línea del matchup
    [ForeignKey(nameof(Role))]
    [Required]
    public int RoleId { get; set; }
    public Role Role { get; set; } = null!;

    /// <summary>
    /// Dificultad del matchup: Easy, Medium, Hard, Extreme
    /// </summary>
    [Required]
    [MaxLength(20)]
    public string Difficulty { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? GeneralAdvice { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Relaciones
    public ICollection<MatchupTip> Tips { get; set; } = new List<MatchupTip>();
}
