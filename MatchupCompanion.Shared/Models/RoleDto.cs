namespace MatchupCompanion.Shared.Models;

/// <summary>
/// DTO para transferir información de un rol/línea
/// </summary>
public class RoleDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}
