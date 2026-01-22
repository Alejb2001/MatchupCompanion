using MatchupCompanion.API.Models.Entities;

namespace MatchupCompanion.API.Data.Repositories.Interfaces;

/// <summary>
/// Interfaz para el repositorio de runas
/// </summary>
public interface IRuneRepository
{
    Task<IEnumerable<Rune>> GetAllAsync();
    Task<Rune?> GetByIdAsync(int id);
    Task<Rune?> GetByRiotIdAsync(int riotRuneId);
    Task<IEnumerable<Rune>> GetByTreeIdAsync(int treeId);
    Task<IEnumerable<Rune>> GetByTreeAndSlotAsync(int treeId, int slotIndex);
    Task<IEnumerable<Rune>> GetKeystonesAsync();
    Task<Rune> CreateAsync(Rune rune);
    Task UpdateAsync(Rune rune);
    Task DeleteAsync(int id);
    Task DeleteAllAsync();
}
