using MatchupCompanion.API.Models.Entities;

namespace MatchupCompanion.API.Data.Repositories.Interfaces;

/// <summary>
/// Interfaz del repositorio de campeones
/// </summary>
public interface IChampionRepository
{
    Task<IEnumerable<Champion>> GetAllAsync();
    Task<Champion?> GetByIdAsync(int id);
    Task<Champion?> GetByRiotIdAsync(string riotChampionId);
    Task<IEnumerable<Champion>> GetByRoleAsync(int roleId);
    Task<Champion> CreateAsync(Champion champion);
    Task UpdateAsync(Champion champion);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}
