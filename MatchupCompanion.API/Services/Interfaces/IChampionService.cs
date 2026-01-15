using MatchupCompanion.API.Models.DTOs;

namespace MatchupCompanion.API.Services.Interfaces;

/// <summary>
/// Interfaz del servicio de campeones - contiene la lógica de negocio
/// </summary>
public interface IChampionService
{
    Task<IEnumerable<ChampionDto>> GetAllChampionsAsync();
    Task<ChampionDto?> GetChampionByIdAsync(int id);
    Task<IEnumerable<ChampionDto>> GetChampionsByRoleAsync(int roleId);
}
