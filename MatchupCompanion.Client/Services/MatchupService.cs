using System.Net.Http.Json;
using MatchupCompanion.Shared.Models;

namespace MatchupCompanion.Client.Services;

public class MatchupService : IMatchupService
{
    private readonly HttpClient _httpClient;

    public MatchupService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<MatchupDto>> GetAllMatchupsAsync()
    {
        try
        {
            var matchups = await _httpClient.GetFromJsonAsync<List<MatchupDto>>("api/Matchups");
            return matchups ?? new List<MatchupDto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al obtener matchups: {ex.Message}");
            return new List<MatchupDto>();
        }
    }

    public async Task<MatchupDto?> GetMatchupByIdAsync(int id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<MatchupDto>($"api/Matchups/{id}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al obtener matchup {id}: {ex.Message}");
            return null;
        }
    }

    public async Task<MatchupDto?> SearchMatchupAsync(int playerChampionId, int enemyChampionId, int roleId)
    {
        try
        {
            var url = $"api/Matchups/search?playerChampionId={playerChampionId}&enemyChampionId={enemyChampionId}&roleId={roleId}";
            return await _httpClient.GetFromJsonAsync<MatchupDto>(url);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al buscar matchup: {ex.Message}");
            return null;
        }
    }

    public async Task<MatchupDto?> CreateMatchupAsync(CreateMatchupDto matchup)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/Matchups", matchup);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<MatchupDto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al crear matchup: {ex.Message}");
            throw;
        }
    }

    public async Task<MatchupTipDto?> AddTipAsync(CreateMatchupTipDto tip)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/MatchupTips", tip);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<MatchupTipDto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al agregar tip: {ex.Message}");
            throw;
        }
    }
}
