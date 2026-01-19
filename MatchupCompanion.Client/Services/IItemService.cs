using MatchupCompanion.Shared.Models;

namespace MatchupCompanion.Client.Services;

/// <summary>
/// Interfaz del servicio de items para el cliente
/// </summary>
public interface IItemService
{
    Task<List<ItemDto>> GetAllItemsAsync();
    Task<List<ItemDto>> GetCompletedItemsAsync();
    Task<List<ItemDto>> SearchItemsAsync(string searchTerm);
    Task<List<ItemDto>> GetItemsByRiotIdsAsync(string ids);
    Task<ItemDto?> GetItemByRiotIdAsync(int riotItemId);
}
