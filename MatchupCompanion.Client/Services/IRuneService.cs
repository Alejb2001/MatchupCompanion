using MatchupCompanion.Shared.Models;

namespace MatchupCompanion.Client.Services;

/// <summary>
/// Interfaz del servicio de runas para el cliente
/// </summary>
public interface IRuneService
{
    Task<List<RuneDto>> GetAllRunesAsync();
    Task<List<RuneTreeDto>> GetRuneTreesAsync();
    Task<List<RuneDto>> GetRunesByTreeAsync(int treeId);
    Task<List<RuneDto>> GetRunesByTreeAndSlotAsync(int treeId, int slotIndex);
    Task<List<RuneDto>> GetKeystonesAsync();
}
