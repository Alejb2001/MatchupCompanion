using System.ComponentModel.DataAnnotations;

namespace MatchupCompanion.Shared.Models;

/// <summary>
/// DTO para actualizar un matchup existente
/// </summary>
public class UpdateMatchupDto
{
    [Required(ErrorMessage = "La dificultad es requerida")]
    [RegularExpression("^(Easy|Medium|Hard|Extreme)$",
        ErrorMessage = "La dificultad debe ser: Fácil, Medio, Difícil o Extremo")]
    public string Difficulty { get; set; } = string.Empty;

    [MaxLength(1000, ErrorMessage = "El consejo general no puede exceder 1000 caracteres")]
    public string? GeneralAdvice { get; set; }

    // Runas primarias
    public int? PrimaryTreeId { get; set; }
    public int? KeystoneId { get; set; }
    public int? PrimaryRune1Id { get; set; }
    public int? PrimaryRune2Id { get; set; }
    public int? PrimaryRune3Id { get; set; }

    // Runas secundarias
    public int? SecondaryTreeId { get; set; }
    public int? SecondaryRune1Id { get; set; }
    public int? SecondaryRune2Id { get; set; }

    // Stat shards
    [MaxLength(100)]
    public string? StatShards { get; set; }

    // Items
    [MaxLength(200)]
    public string? StartingItems { get; set; }

    [MaxLength(200)]
    public string? CoreItems { get; set; }

    [MaxLength(200)]
    public string? SituationalItems { get; set; }

    [MaxLength(200)]
    public string? FullBuildItems { get; set; }

    // Hechizos de invocador
    public int? SummonerSpell1Id { get; set; }
    public int? SummonerSpell2Id { get; set; }

    // Orden de habilidades
    [MaxLength(100)]
    public string? AbilityOrder { get; set; }

    // Estrategia
    [MaxLength(5000, ErrorMessage = "La estrategia no puede exceder 5000 caracteres")]
    public string? Strategy { get; set; }
}
