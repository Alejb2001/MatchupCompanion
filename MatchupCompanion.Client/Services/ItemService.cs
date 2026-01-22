using System.Net.Http.Json;
using MatchupCompanion.Shared.Models;

namespace MatchupCompanion.Client.Services;

/// <summary>
/// Implementación del servicio de items para el cliente
/// </summary>
public class ItemService : IItemService
{
    private readonly HttpClient _httpClient;

    public ItemService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<ItemDto>> GetAllItemsAsync()
    {
        var response = await _httpClient.GetFromJsonAsync<List<ItemDto>>("api/Items");
        return response ?? new List<ItemDto>();
    }

    public async Task<List<ItemDto>> GetCompletedItemsAsync()
    {
        var response = await _httpClient.GetFromJsonAsync<List<ItemDto>>("api/Items/completed");
        return response ?? new List<ItemDto>();
    }

    public async Task<List<ItemDto>> SearchItemsAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return new List<ItemDto>();

        var response = await _httpClient.GetFromJsonAsync<List<ItemDto>>($"api/Items/search?q={Uri.EscapeDataString(searchTerm)}");
        return response ?? new List<ItemDto>();
    }

    public async Task<List<ItemDto>> GetItemsByRiotIdsAsync(string ids)
    {
        if (string.IsNullOrWhiteSpace(ids))
            return new List<ItemDto>();

        var response = await _httpClient.GetFromJsonAsync<List<ItemDto>>($"api/Items/by-riot-ids?ids={Uri.EscapeDataString(ids)}");
        return response ?? new List<ItemDto>();
    }

    public async Task<ItemDto?> GetItemByRiotIdAsync(int riotItemId)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<ItemDto>($"api/Items/riot/{riotItemId}");
        }
        catch
        {
            return null;
        }
    }
}
