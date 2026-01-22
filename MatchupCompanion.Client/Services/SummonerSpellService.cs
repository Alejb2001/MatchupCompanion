using System.Net.Http.Json;
using MatchupCompanion.Shared.Models;

namespace MatchupCompanion.Client.Services;

/// <summary>
/// Implementación del servicio de hechizos de invocador para el cliente
/// </summary>
public class SummonerSpellService : ISummonerSpellService
{
    private readonly HttpClient _httpClient;

    public SummonerSpellService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<SummonerSpellDto>> GetAllSummonerSpellsAsync()
    {
        var response = await _httpClient.GetFromJsonAsync<List<SummonerSpellDto>>("api/SummonerSpells");
        return response ?? new List<SummonerSpellDto>();
    }

    public async Task<SummonerSpellDto?> GetSummonerSpellByIdAsync(int id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<SummonerSpellDto>($"api/SummonerSpells/{id}");
        }
        catch
        {
            return null;
        }
    }
}
