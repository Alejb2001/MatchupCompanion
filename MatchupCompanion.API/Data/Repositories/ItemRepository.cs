using MatchupCompanion.API.Data.Repositories.Interfaces;
using MatchupCompanion.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace MatchupCompanion.API.Data.Repositories;

/// <summary>
/// Implementación del repositorio de items
/// </summary>
public class ItemRepository : IItemRepository
{
    private readonly ApplicationDbContext _context;

    public ItemRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Item>> GetAllAsync()
    {
        return await _context.Items
            .Where(i => i.IsPurchasable)
            .OrderBy(i => i.Name)
            .ToListAsync();
    }

    public async Task<Item?> GetByIdAsync(int id)
    {
        return await _context.Items.FindAsync(id);
    }

    public async Task<Item?> GetByRiotIdAsync(int riotItemId)
    {
        return await _context.Items
            .FirstOrDefaultAsync(i => i.RiotItemId == riotItemId);
    }

    public async Task<IEnumerable<Item>> GetCompletedItemsAsync()
    {
        return await _context.Items
            .Where(i => i.IsCompleted && i.IsPurchasable)
            .OrderBy(i => i.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<Item>> GetPurchasableItemsAsync()
    {
        return await _context.Items
            .Where(i => i.IsPurchasable)
            .OrderBy(i => i.TotalGold)
            .ThenBy(i => i.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<Item>> SearchByNameAsync(string searchTerm)
    {
        return await _context.Items
            .Where(i => i.IsPurchasable && i.Name.Contains(searchTerm))
            .OrderBy(i => i.Name)
            .ToListAsync();
    }

    public async Task<Item> CreateAsync(Item item)
    {
        _context.Items.Add(item);
        await _context.SaveChangesAsync();
        return item;
    }

    public async Task UpdateAsync(Item item)
    {
        item.UpdatedAt = DateTime.UtcNow;
        _context.Entry(item).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var item = await _context.Items.FindAsync(id);
        if (item != null)
        {
            _context.Items.Remove(item);
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteAllAsync()
    {
        _context.Items.RemoveRange(_context.Items);
        await _context.SaveChangesAsync();
    }
}
