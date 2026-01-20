namespace MatchupCompanion.Shared.Models.Auth;

public class UserDto
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string? DisplayName { get; set; }
    public int? PreferredRoleId { get; set; }
    public string? PreferredRoleName { get; set; }
    public bool IsGuest { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<string> Roles { get; set; } = new();
}
