using MatchupCompanion.Shared.Models;

namespace MatchupCompanion.Client.Services;

/// <summary>
/// Interfaz del servicio de hechizos de invocador para el cliente
/// </summary>
public interface ISummonerSpellService
{
    Task<List<SummonerSpellDto>> GetAllSummonerSpellsAsync();
    Task<SummonerSpellDto?> GetSummonerSpellByIdAsync(int id);
}
