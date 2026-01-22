namespace MatchupCompanion.Shared.Models;

/// <summary>
/// DTO para representar una runa
/// </summary>
public class RuneDto
{
    public int Id { get; set; }
    public int RiotRuneId { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? IconPath { get; set; }
    public string? IconUrl { get; set; }
    public string TreeName { get; set; } = string.Empty;
    public int TreeId { get; set; }
    public int SlotIndex { get; set; }
    public string? ShortDescription { get; set; }
}

/// <summary>
/// DTO para representar un árbol de runas con todas sus runas agrupadas
/// </summary>
public class RuneTreeDto
{
    public int TreeId { get; set; }
    public string TreeName { get; set; } = string.Empty;
    public string? TreeIconUrl { get; set; }
    public List<RuneDto> Keystones { get; set; } = new();
    public List<RuneDto> Slot1Runes { get; set; } = new();
    public List<RuneDto> Slot2Runes { get; set; } = new();
    public List<RuneDto> Slot3Runes { get; set; } = new();
}
