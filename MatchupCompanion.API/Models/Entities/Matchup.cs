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

    // ============================================
    // RUNAS RECOMENDADAS
    // ============================================

    /// <summary>
    /// ID del árbol primario de runas (8100=Domination, 8000=Precision, 8200=Sorcery, 8400=Resolve, 8300=Inspiration)
    /// </summary>
    public int? PrimaryTreeId { get; set; }

    /// <summary>
    /// ID de la runa Keystone (primera fila del árbol primario)
    /// </summary>
    public int? KeystoneId { get; set; }

    /// <summary>
    /// ID de la runa de la segunda fila del árbol primario
    /// </summary>
    public int? PrimaryRune1Id { get; set; }

    /// <summary>
    /// ID de la runa de la tercera fila del árbol primario
    /// </summary>
    public int? PrimaryRune2Id { get; set; }

    /// <summary>
    /// ID de la runa de la cuarta fila del árbol primario
    /// </summary>
    public int? PrimaryRune3Id { get; set; }

    /// <summary>
    /// ID del árbol secundario de runas
    /// </summary>
    public int? SecondaryTreeId { get; set; }

    /// <summary>
    /// ID de la primera runa del árbol secundario
    /// </summary>
    public int? SecondaryRune1Id { get; set; }

    /// <summary>
    /// ID de la segunda runa del árbol secundario
    /// </summary>
    public int? SecondaryRune2Id { get; set; }

    /// <summary>
    /// Stat shards seleccionados (formato: "Adaptive,Adaptive,Health")
    /// </summary>
    [MaxLength(100)]
    public string? StatShards { get; set; }

    // ============================================
    // ITEMS RECOMENDADOS
    // ============================================

    /// <summary>
    /// IDs de items iniciales separados por coma (ej: "1055,2003,2003")
    /// </summary>
    [MaxLength(200)]
    public string? StartingItems { get; set; }

    /// <summary>
    /// IDs de items core separados por coma (ej: "3116,3157,3089")
    /// </summary>
    [MaxLength(200)]
    public string? CoreItems { get; set; }

    /// <summary>
    /// IDs de items situacionales separados por coma
    /// </summary>
    [MaxLength(200)]
    public string? SituationalItems { get; set; }

    // ============================================
    // ESTRATEGIA
    // ============================================

    /// <summary>
    /// Estrategia detallada para el matchup (consejos de laning, power spikes, etc.)
    /// </summary>
    [MaxLength(5000)]
    public string? Strategy { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Relaciones
    public ICollection<MatchupTip> Tips { get; set; } = new List<MatchupTip>();
}
