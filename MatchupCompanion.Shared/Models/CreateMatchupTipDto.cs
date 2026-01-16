using System.ComponentModel.DataAnnotations;

namespace MatchupCompanion.Shared.Models;

/// <summary>
/// DTO para crear un nuevo tip de matchup
/// </summary>
public class CreateMatchupTipDto
{
    [Required(ErrorMessage = "MatchupId es requerido")]
    public int MatchupId { get; set; }

    [Required(ErrorMessage = "Category es requerida")]
    [RegularExpression("^(EarlyGame|MidGame|LateGame|Items|Runes|Abilities|General)$",
        ErrorMessage = "Category debe ser: EarlyGame, MidGame, LateGame, Items, Runes, Abilities o General")]
    public string Category { get; set; } = string.Empty;

    [Required(ErrorMessage = "Content es requerido")]
    [MinLength(10, ErrorMessage = "Content debe tener al menos 10 caracteres")]
    [MaxLength(2000, ErrorMessage = "Content no puede exceder 2000 caracteres")]
    public string Content { get; set; } = string.Empty;

    [Range(1, 10, ErrorMessage = "Priority debe estar entre 1 y 10")]
    public int Priority { get; set; } = 5;

    [MaxLength(100, ErrorMessage = "AuthorName no puede exceder 100 caracteres")]
    public string? AuthorName { get; set; }
}
