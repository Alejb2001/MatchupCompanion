using MatchupCompanion.API.Models.DTOs;

namespace MatchupCompanion.API.Services.Interfaces;

/// <summary>
/// Interfaz del servicio de matchups - contiene la lógica de negocio
/// </summary>
public interface IMatchupService
{
    Task<IEnumerable<MatchupDto>> GetAllMatchupsAsync();
    Task<MatchupDto?> GetMatchupByIdAsync(int id);
    Task<MatchupDto?> GetMatchupByChampionsAndRoleAsync(int playerChampionId, int enemyChampionId, int roleId);
    Task<IEnumerable<MatchupDto>> GetMatchupsByPlayerChampionAsync(int playerChampionId);
    Task<MatchupDto> CreateMatchupAsync(CreateMatchupRequest request);
    Task<MatchupDto> UpdateMatchupAsync(int id, UpdateMatchupRequest request);
    Task<MatchupDto> AddTipToMatchupAsync(CreateMatchupTipRequest request);
    Task DeleteMatchupAsync(int id);

    /// <summary>
    /// Busca un matchup existente o crea uno nuevo vacío
    /// </summary>
    Task<MatchupDto> GetOrCreateMatchupAsync(int playerChampionId, int enemyChampionId, int roleId);
}
