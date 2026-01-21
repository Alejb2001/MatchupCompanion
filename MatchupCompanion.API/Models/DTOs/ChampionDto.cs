namespace MatchupCompanion.API.Models.DTOs;

/// <summary>
/// DTO para transferir información de un campeón
/// </summary>
public class ChampionDto
{
    public int Id { get; set; }
    public string RiotChampionId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Title { get; set; }
    public string? ImageUrl { get; set; }
    public string? Description { get; set; }
    public int? PrimaryRoleId { get; set; }
    public string? PrimaryRoleName { get; set; }

    // Habilidades
    public string? QSpellId { get; set; }
    public string? QSpellName { get; set; }
    public string? QSpellIcon { get; set; }

    public string? WSpellId { get; set; }
    public string? WSpellName { get; set; }
    public string? WSpellIcon { get; set; }

    public string? ESpellId { get; set; }
    public string? ESpellName { get; set; }
    public string? ESpellIcon { get; set; }

    public string? RSpellId { get; set; }
    public string? RSpellName { get; set; }
    public string? RSpellIcon { get; set; }
}
