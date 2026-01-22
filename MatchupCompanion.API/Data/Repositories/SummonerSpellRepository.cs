using MatchupCompanion.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace MatchupCompanion.API.Data.Repositories;

/// <summary>
/// Repositorio para operaciones de base de datos con hechizos de invocador
/// </summary>
public class SummonerSpellRepository
{
    private readonly ApplicationDbContext _context;

    public SummonerSpellRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Obtiene todos los hechizos de invocador
    /// </summary>
    public async Task<List<SummonerSpell>> GetAllAsync()
    {
        return await _context.SummonerSpells
            .OrderBy(s => s.Name)
            .ToListAsync();
    }

    /// <summary>
    /// Obtiene un hechizo por su ID
    /// </summary>
    public async Task<SummonerSpell?> GetByIdAsync(int id)
    {
        return await _context.SummonerSpells
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    /// <summary>
    /// Obtiene un hechizo por su RiotSpellId
    /// </summary>
    public async Task<SummonerSpell?> GetByRiotSpellIdAsync(int riotSpellId)
    {
        return await _context.SummonerSpells
            .FirstOrDefaultAsync(s => s.RiotSpellId == riotSpellId);
    }
}
