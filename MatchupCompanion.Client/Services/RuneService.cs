using System.Net.Http.Json;
using MatchupCompanion.Shared.Models;

namespace MatchupCompanion.Client.Services;

/// <summary>
/// Implementación del servicio de runas para el cliente
/// </summary>
public class RuneService : IRuneService
{
    private readonly HttpClient _httpClient;

    public RuneService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<RuneDto>> GetAllRunesAsync()
    {
        var response = await _httpClient.GetFromJsonAsync<List<RuneDto>>("api/Runes");
        return response ?? new List<RuneDto>();
    }

    public async Task<List<RuneTreeDto>> GetRuneTreesAsync()
    {
        var response = await _httpClient.GetFromJsonAsync<List<RuneTreeDto>>("api/Runes/trees");
        return response ?? new List<RuneTreeDto>();
    }

    public async Task<List<RuneDto>> GetRunesByTreeAsync(int treeId)
    {
        var response = await _httpClient.GetFromJsonAsync<List<RuneDto>>($"api/Runes/tree/{treeId}");
        return response ?? new List<RuneDto>();
    }

    public async Task<List<RuneDto>> GetRunesByTreeAndSlotAsync(int treeId, int slotIndex)
    {
        var response = await _httpClient.GetFromJsonAsync<List<RuneDto>>($"api/Runes/tree/{treeId}/slot/{slotIndex}");
        return response ?? new List<RuneDto>();
    }

    public async Task<List<RuneDto>> GetKeystonesAsync()
    {
        var response = await _httpClient.GetFromJsonAsync<List<RuneDto>>("api/Runes/keystones");
        return response ?? new List<RuneDto>();
    }
}
