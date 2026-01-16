using System.ComponentModel.DataAnnotations;

namespace MatchupCompanion.Shared.Models;

/// <summary>
/// DTO para crear un nuevo matchup
/// </summary>
public class CreateMatchupDto
{
    [Required(ErrorMessage = "PlayerChampionId es requerido")]
    public int PlayerChampionId { get; set; }

    [Required(ErrorMessage = "EnemyChampionId es requerido")]
    public int EnemyChampionId { get; set; }

    [Required(ErrorMessage = "RoleId es requerido")]
    public int RoleId { get; set; }

    [Required(ErrorMessage = "Difficulty es requerido")]
    [RegularExpression("^(Easy|Medium|Hard|Extreme)$",
        ErrorMessage = "Difficulty debe ser: Easy, Medium, Hard o Extreme")]
    public string Difficulty { get; set; } = string.Empty;

    [MaxLength(1000, ErrorMessage = "GeneralAdvice no puede exceder 1000 caracteres")]
    public string? GeneralAdvice { get; set; }
}
