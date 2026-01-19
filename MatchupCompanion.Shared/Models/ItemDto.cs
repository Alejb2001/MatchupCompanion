namespace MatchupCompanion.Shared.Models;

/// <summary>
/// DTO para representar un item
/// </summary>
public class ItemDto
{
    public int Id { get; set; }
    public int RiotItemId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? IconPath { get; set; }
    public string? IconUrl { get; set; }
    public int TotalGold { get; set; }
    public List<string>? Tags { get; set; }
    public bool IsPurchasable { get; set; }
    public bool IsCompleted { get; set; }
}
