using MatchupCompanion.API.Models.Entities;

namespace MatchupCompanion.API.Data.Repositories.Interfaces;

/// <summary>
/// Interfaz del repositorio de roles
/// </summary>
public interface IRoleRepository
{
    Task<IEnumerable<Role>> GetAllAsync();
    Task<Role?> GetByIdAsync(int id);
    Task<Role?> GetByNameAsync(string name);
}
