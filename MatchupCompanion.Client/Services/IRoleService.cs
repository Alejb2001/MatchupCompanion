using MatchupCompanion.Shared.Models;

namespace MatchupCompanion.Client.Services;

public interface IRoleService
{
    Task<List<RoleDto>> GetAllRolesAsync();
    Task<RoleDto?> GetRoleByIdAsync(int id);
}
