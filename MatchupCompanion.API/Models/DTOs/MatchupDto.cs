namespace MatchupCompanion.API.Models.DTOs;

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
    public string? StatShards { get; set; }

    // Items
    public string? StartingItems { get; set; }
    public string? CoreItems { get; set; }
    public string? SituationalItems { get; set; }
    public string? FullBuildItems { get; set; }

    // Hechizos de invocador
    public int? SummonerSpell1Id { get; set; }
    public int? SummonerSpell2Id { get; set; }

    // Orden de habilidades
    public string? AbilityOrder { get; set; }

    // Estrategia
    public string? Strategy { get; set; }

    public List<MatchupTipDto> Tips { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
