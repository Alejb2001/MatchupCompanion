using MatchupCompanion.API.Data.Repositories.Interfaces;
using MatchupCompanion.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace MatchupCompanion.API.Data.Repositories;

/// <summary>
/// Implementación del repositorio de tips de matchup
/// </summary>
public class MatchupTipRepository : IMatchupTipRepository
{
    private readonly ApplicationDbContext _context;

    public MatchupTipRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<MatchupTip>> GetByMatchupIdAsync(int matchupId)
    {
        return await _context.MatchupTips
            .Where(t => t.MatchupId == matchupId)
            .OrderBy(t => t.Priority)
            .ThenByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<MatchupTip?> GetByIdAsync(int id)
    {
        return await _context.MatchupTips
            .Include(t => t.Matchup)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<MatchupTip> CreateAsync(MatchupTip tip)
    {
        tip.CreatedAt = DateTime.UtcNow;
        tip.UpdatedAt = DateTime.UtcNow;

        _context.MatchupTips.Add(tip);
        await _context.SaveChangesAsync();
        return tip;
    }

    public async Task UpdateAsync(MatchupTip tip)
    {
        tip.UpdatedAt = DateTime.UtcNow;
        _context.MatchupTips.Update(tip);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var tip = await _context.MatchupTips.FindAsync(id);
        if (tip != null)
        {
            _context.MatchupTips.Remove(tip);
            await _context.SaveChangesAsync();
        }
    }
}
