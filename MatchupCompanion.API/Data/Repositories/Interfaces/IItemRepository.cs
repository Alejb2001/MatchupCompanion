using MatchupCompanion.API.Models.Entities;

namespace MatchupCompanion.API.Data.Repositories.Interfaces;

/// <summary>
/// Interfaz para el repositorio de items
/// </summary>
public interface IItemRepository
{
    Task<IEnumerable<Item>> GetAllAsync();
    Task<Item?> GetByIdAsync(int id);
    Task<Item?> GetByRiotIdAsync(int riotItemId);
    Task<IEnumerable<Item>> GetCompletedItemsAsync();
    Task<IEnumerable<Item>> GetPurchasableItemsAsync();
    Task<IEnumerable<Item>> SearchByNameAsync(string searchTerm);
    Task<Item> CreateAsync(Item item);
    Task UpdateAsync(Item item);
    Task DeleteAsync(int id);
    Task DeleteAllAsync();
}
