namespace MatchupCompanion.Shared.Models;

/// <summary>
/// DTO para transferir información de un matchup completo
/// </summary>
public class MatchupDto
{
    public int Id { get; set; }
    public ChampionDto PlayerChampion { get; set; } = null!;
    public ChampionDto EnemyChampion { get; set; } = null!;
    public RoleDto Role { get; set; } = null!;
    public string Difficulty { get; set; } = string.Empty;
    public string? GeneralAdvice { get; set; }
    public List<MatchupTipDto> Tips { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
