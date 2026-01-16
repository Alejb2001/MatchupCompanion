namespace MatchupCompanion.Shared.Models;

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
}
