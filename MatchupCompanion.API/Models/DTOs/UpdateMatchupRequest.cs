using System.ComponentModel.DataAnnotations;

namespace MatchupCompanion.API.Models.DTOs;

/// <summary>
/// Request para actualizar un matchup existente
/// </summary>
public class UpdateMatchupRequest
{
    [Required(ErrorMessage = "Difficulty es requerido")]
    [RegularExpression("^(Easy|Medium|Hard|Extreme)$",
        ErrorMessage = "Difficulty debe ser: Easy, Medium, Hard o Extreme")]
    public string Difficulty { get; set; } = string.Empty;

    [MaxLength(1000, ErrorMessage = "GeneralAdvice no puede exceder 1000 caracteres")]
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

    // Estrategia
    [MaxLength(5000, ErrorMessage = "Strategy no puede exceder 5000 caracteres")]
    public string? Strategy { get; set; }
}
