namespace MatchupCompanion.API.Models.DTOs;

/// <summary>
/// DTO para transferir información de un tip de matchup
/// </summary>
public class MatchupTipDto
{
    public int Id { get; set; }
    public int MatchupId { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public int Priority { get; set; }
    public string? AuthorName { get; set; }
    public DateTime CreatedAt { get; set; }
}
