namespace MatchupCompanion.Shared.Models;

/// <summary>
/// DTO para transferir información de un hechizo de invocador
/// </summary>
public class SummonerSpellDto
{
    public int Id { get; set; }
    public int RiotSpellId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public int Cooldown { get; set; }
}
