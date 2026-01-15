using MatchupCompanion.API.Models.Entities;

namespace MatchupCompanion.API.Data.Repositories.Interfaces;

/// <summary>
/// Interfaz del repositorio de matchups
/// </summary>
public interface IMatchupRepository
{
    Task<IEnumerable<Matchup>> GetAllAsync();
    Task<Matchup?> GetByIdAsync(int id);
    Task<Matchup?> GetByChampionsAndRoleAsync(int playerChampionId, int enemyChampionId, int roleId);
    Task<IEnumerable<Matchup>> GetByPlayerChampionAsync(int playerChampionId);
    Task<IEnumerable<Matchup>> GetByEnemyChampionAsync(int enemyChampionId);
    Task<IEnumerable<Matchup>> GetByRoleAsync(int roleId);
    Task<Matchup> CreateAsync(Matchup matchup);
    Task UpdateAsync(Matchup matchup);
    Task DeleteAsync(int id);
}
