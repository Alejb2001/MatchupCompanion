using MatchupCompanion.API.Models.Entities;

namespace MatchupCompanion.API.Data.Repositories.Interfaces;

/// <summary>
/// Interfaz del repositorio de tips de matchup
/// </summary>
public interface IMatchupTipRepository
{
    Task<IEnumerable<MatchupTip>> GetByMatchupIdAsync(int matchupId);
    Task<MatchupTip?> GetByIdAsync(int id);
    Task<MatchupTip> CreateAsync(MatchupTip tip);
    Task UpdateAsync(MatchupTip tip);
    Task DeleteAsync(int id);
}
