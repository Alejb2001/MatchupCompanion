using MatchupCompanion.Shared.Models;

namespace MatchupCompanion.Client.Services;

public interface IChampionService
{
    Task<List<ChampionDto>> GetAllChampionsAsync();
    Task<ChampionDto?> GetChampionByIdAsync(int id);
}
