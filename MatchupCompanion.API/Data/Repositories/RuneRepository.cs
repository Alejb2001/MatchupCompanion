using MatchupCompanion.API.Data.Repositories.Interfaces;
using MatchupCompanion.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace MatchupCompanion.API.Data.Repositories;

/// <summary>
/// Implementación del repositorio de runas
/// </summary>
public class RuneRepository : IRuneRepository
{
    private readonly ApplicationDbContext _context;

    public RuneRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Rune>> GetAllAsync()
    {
        return await _context.Runes
            .OrderBy(r => r.TreeId)
            .ThenBy(r => r.SlotIndex)
            .ThenBy(r => r.Name)
            .ToListAsync();
    }

    public async Task<Rune?> GetByIdAsync(int id)
    {
        return await _context.Runes.FindAsync(id);
    }

    public async Task<Rune?> GetByRiotIdAsync(int riotRuneId)
    {
        return await _context.Runes
            .FirstOrDefaultAsync(r => r.RiotRuneId == riotRuneId);
    }

    public async Task<IEnumerable<Rune>> GetByTreeIdAsync(int treeId)
    {
        return await _context.Runes
            .Where(r => r.TreeId == treeId)
            .OrderBy(r => r.SlotIndex)
            .ThenBy(r => r.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<Rune>> GetByTreeAndSlotAsync(int treeId, int slotIndex)
    {
        return await _context.Runes
            .Where(r => r.TreeId == treeId && r.SlotIndex == slotIndex)
            .OrderBy(r => r.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<Rune>> GetKeystonesAsync()
    {
        return await _context.Runes
            .Where(r => r.SlotIndex == 0)
            .OrderBy(r => r.TreeName)
            .ThenBy(r => r.Name)
            .ToListAsync();
    }

    public async Task<Rune> CreateAsync(Rune rune)
    {
        _context.Runes.Add(rune);
        await _context.SaveChangesAsync();
        return rune;
    }

    public async Task UpdateAsync(Rune rune)
    {
        rune.UpdatedAt = DateTime.UtcNow;
        _context.Entry(rune).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var rune = await _context.Runes.FindAsync(id);
        if (rune != null)
        {
            _context.Runes.Remove(rune);
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteAllAsync()
    {
        _context.Runes.RemoveRange(_context.Runes);
        await _context.SaveChangesAsync();
    }
}
