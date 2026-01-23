using System.ComponentModel.DataAnnotations;

namespace MatchupCompanion.API.Models.DTOs;

/// <summary>
/// DTO para crear un nuevo matchup
/// </summary>
public class CreateMatchupRequest
{
    [Required(ErrorMessage = "El campeón del jugador es requerido")]
    public int PlayerChampionId { get; set; }

    [Required(ErrorMessage = "El campeón enemigo es requerido")]
    public int EnemyChampionId { get; set; }

    [Required(ErrorMessage = "El rol es requerido")]
    public int RoleId { get; set; }

    [Required(ErrorMessage = "La dificultad es requerida")]
    [RegularExpression("^(Easy|Medium|Hard|Extreme)$",
        ErrorMessage = "La dificultad debe ser: Fácil, Medio, Difícil o Extremo")]
    public string Difficulty { get; set; } = string.Empty;

    [MaxLength(1000, ErrorMessage = "El consejo general no puede exceder 1000 caracteres")]
    public string? GeneralAdvice { get; set; }
}
