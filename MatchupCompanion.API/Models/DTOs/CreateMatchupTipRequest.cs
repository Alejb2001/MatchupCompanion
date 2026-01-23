using System.ComponentModel.DataAnnotations;

namespace MatchupCompanion.API.Models.DTOs;

/// <summary>
/// DTO para crear un nuevo tip de matchup
/// </summary>
public class CreateMatchupTipRequest
{
    [Required(ErrorMessage = "El matchup es requerido")]
    public int MatchupId { get; set; }

    [Required(ErrorMessage = "La categoría es requerida")]
    [RegularExpression("^(EarlyGame|MidGame|LateGame|Items|Runes|Abilities|General)$",
        ErrorMessage = "La categoría debe ser: Fase Inicial, Fase Media, Fase Tardía, Objetos, Runas, Habilidades o General")]
    public string Category { get; set; } = string.Empty;

    [Required(ErrorMessage = "El contenido es requerido")]
    [MinLength(10, ErrorMessage = "El contenido debe tener al menos 10 caracteres")]
    [MaxLength(2000, ErrorMessage = "El contenido no puede exceder 2000 caracteres")]
    public string Content { get; set; } = string.Empty;

    [Range(1, 10, ErrorMessage = "La prioridad debe estar entre 1 y 10")]
    public int Priority { get; set; } = 5;

    [MaxLength(100, ErrorMessage = "El nombre del autor no puede exceder 100 caracteres")]
    public string? AuthorName { get; set; }
}
