using MatchupCompanion.API.Data.Repositories.Interfaces;
using MatchupCompanion.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace MatchupCompanion.API.Data.Repositories;

/// <summary>
/// Implementación del repositorio de matchups
/// </summary>
public class MatchupRepository : IMatchupRepository
{
    private readonly ApplicationDbContext _context;

    public MatchupRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Matchup>> GetAllAsync()
    {
        return await _context.Matchups
            .Include(m => m.PlayerChampion)
            .Include(m => m.EnemyChampion)
            .Include(m => m.Role)
            .Include(m => m.Tips)
            .ToListAsync();
    }

    public async Task<Matchup?> GetByIdAsync(int id)
    {
        return await _context.Matchups
            .Include(m => m.PlayerChampion)
                .ThenInclude(c => c.PrimaryRole)
            .Include(m => m.EnemyChampion)
                .ThenInclude(c => c.PrimaryRole)
            .Include(m => m.Role)
            .Include(m => m.Tips.OrderBy(t => t.Priority))
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<Matchup?> GetByChampionsAndRoleAsync(int playerChampionId, int enemyChampionId, int roleId)
    {
        return await _context.Matchups
            .Include(m => m.PlayerChampion)
            .Include(m => m.EnemyChampion)
            .Include(m => m.Role)
            .Include(m => m.Tips)
            .FirstOrDefaultAsync(m =>
                m.PlayerChampionId == playerChampionId &&
                m.EnemyChampionId == enemyChampionId &&
                m.RoleId == roleId);
    }

    public async Task<IEnumerable<Matchup>> GetByPlayerChampionAsync(int playerChampionId)
    {
        return await _context.Matchups
            .Include(m => m.PlayerChampion)
            .Include(m => m.EnemyChampion)
            .Include(m => m.Role)
            .Include(m => m.Tips)
            .Where(m => m.PlayerChampionId == playerChampionId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Matchup>> GetByEnemyChampionAsync(int enemyChampionId)
    {
        return await _context.Matchups
            .Include(m => m.PlayerChampion)
            .Include(m => m.EnemyChampion)
            .Include(m => m.Role)
            .Include(m => m.Tips)
            .Where(m => m.EnemyChampionId == enemyChampionId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Matchup>> GetByRoleAsync(int roleId)
    {
        return await _context.Matchups
            .Include(m => m.PlayerChampion)
            .Include(m => m.EnemyChampion)
            .Include(m => m.Role)
            .Include(m => m.Tips)
            .Where(m => m.RoleId == roleId)
            .ToListAsync();
    }

    public async Task<Matchup> CreateAsync(Matchup matchup)
    {
        matchup.CreatedAt = DateTime.UtcNow;
        matchup.UpdatedAt = DateTime.UtcNow;

        _context.Matchups.Add(matchup);
        await _context.SaveChangesAsync();
        return matchup;
    }

    public async Task UpdateAsync(Matchup matchup)
    {
        matchup.UpdatedAt = DateTime.UtcNow;
        _context.Matchups.Update(matchup);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var matchup = await _context.Matchups.FindAsync(id);
        if (matchup != null)
        {
            _context.Matchups.Remove(matchup);
            await _context.SaveChangesAsync();
        }
    }
}
