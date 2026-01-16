using System.Net.Http.Json;
using MatchupCompanion.Shared.Models;

namespace MatchupCompanion.Client.Services;

public class ChampionService : IChampionService
{
    private readonly HttpClient _httpClient;

    public ChampionService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<ChampionDto>> GetAllChampionsAsync()
    {
        try
        {
            var champions = await _httpClient.GetFromJsonAsync<List<ChampionDto>>("api/Champions");
            return champions ?? new List<ChampionDto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al obtener campeones: {ex.Message}");
            return new List<ChampionDto>();
        }
    }

    public async Task<ChampionDto?> GetChampionByIdAsync(int id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<ChampionDto>($"api/Champions/{id}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al obtener campeón {id}: {ex.Message}");
            return null;
        }
    }
}
