using MatchupCompanion.Shared.Models;

namespace MatchupCompanion.Client.Services;

public interface IMatchupService
{
    Task<List<MatchupDto>> GetAllMatchupsAsync();
    Task<MatchupDto?> GetMatchupByIdAsync(int id);
    Task<MatchupDto?> SearchMatchupAsync(int playerChampionId, int enemyChampionId, int roleId);
    Task<MatchupDto?> CreateMatchupAsync(CreateMatchupDto matchup);
    Task<MatchupTipDto?> AddTipAsync(CreateMatchupTipDto tip);
}
