using MatchupCompanion.API.Data.Repositories.Interfaces;
using MatchupCompanion.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace MatchupCompanion.API.Data.Repositories;

/// <summary>
/// Implementación del repositorio de roles
/// </summary>
public class RoleRepository : IRoleRepository
{
    private readonly ApplicationDbContext _context;

    public RoleRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Role>> GetAllAsync()
    {
        return await _context.Roles
            .OrderBy(r => r.Name)
            .ToListAsync();
    }

    public async Task<Role?> GetByIdAsync(int id)
    {
        return await _context.Roles.FindAsync(id);
    }

    public async Task<Role?> GetByNameAsync(string name)
    {
        return await _context.Roles
            .FirstOrDefaultAsync(r => r.Name.ToLower() == name.ToLower());
    }
}
