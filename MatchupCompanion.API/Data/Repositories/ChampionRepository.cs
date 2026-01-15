using MatchupCompanion.API.Data.Repositories.Interfaces;
using MatchupCompanion.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace MatchupCompanion.API.Data.Repositories;

/// <summary>
/// Implementación del repositorio de campeones
/// </summary>
public class ChampionRepository : IChampionRepository
{
    private readonly ApplicationDbContext _context;

    public ChampionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Champion>> GetAllAsync()
    {
        return await _context.Champions
            .Include(c => c.PrimaryRole)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<Champion?> GetByIdAsync(int id)
    {
        return await _context.Champions
            .Include(c => c.PrimaryRole)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Champion?> GetByRiotIdAsync(string riotChampionId)
    {
        return await _context.Champions
            .Include(c => c.PrimaryRole)
            .FirstOrDefaultAsync(c => c.RiotChampionId == riotChampionId);
    }

    public async Task<IEnumerable<Champion>> GetByRoleAsync(int roleId)
    {
        return await _context.Champions
            .Include(c => c.PrimaryRole)
            .Where(c => c.PrimaryRoleId == roleId)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<Champion> CreateAsync(Champion champion)
    {
        champion.CreatedAt = DateTime.UtcNow;
        champion.UpdatedAt = DateTime.UtcNow;

        _context.Champions.Add(champion);
        await _context.SaveChangesAsync();
        return champion;
    }

    public async Task UpdateAsync(Champion champion)
    {
        champion.UpdatedAt = DateTime.UtcNow;
        _context.Champions.Update(champion);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var champion = await _context.Champions.FindAsync(id);
        if (champion != null)
        {
            _context.Champions.Remove(champion);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Champions.AnyAsync(c => c.Id == id);
    }
}
