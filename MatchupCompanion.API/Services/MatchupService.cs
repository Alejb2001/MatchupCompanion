using MatchupCompanion.API.Data.Repositories.Interfaces;
using MatchupCompanion.API.Models.DTOs;
using MatchupCompanion.API.Models.Entities;
using MatchupCompanion.API.Services.Interfaces;

namespace MatchupCompanion.API.Services;

/// <summary>
/// Servicio de lógica de negocio para Matchups
/// </summary>
public class MatchupService : IMatchupService
{
    private readonly IMatchupRepository _matchupRepository;
    private readonly IMatchupTipRepository _tipRepository;
    private readonly IChampionRepository _championRepository;
    private readonly ILogger<MatchupService> _logger;

    public MatchupService(
        IMatchupRepository matchupRepository,
        IMatchupTipRepository tipRepository,
        IChampionRepository championRepository,
        ILogger<MatchupService> logger)
    {
        _matchupRepository = matchupRepository;
        _tipRepository = tipRepository;
        _championRepository = championRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<MatchupDto>> GetAllMatchupsAsync()
    {
        var matchups = await _matchupRepository.GetAllAsync();
        return matchups.Select(MapToDto);
    }

    public async Task<MatchupDto?> GetMatchupByIdAsync(int id)
    {
        var matchup = await _matchupRepository.GetByIdAsync(id);
        return matchup != null ? MapToDto(matchup) : null;
    }

    public async Task<MatchupDto?> GetMatchupByChampionsAndRoleAsync(int playerChampionId, int enemyChampionId, int roleId)
    {
        var matchup = await _matchupRepository.GetByChampionsAndRoleAsync(playerChampionId, enemyChampionId, roleId);
        return matchup != null ? MapToDto(matchup) : null;
    }

    public async Task<IEnumerable<MatchupDto>> GetMatchupsByPlayerChampionAsync(int playerChampionId)
    {
        var matchups = await _matchupRepository.GetByPlayerChampionAsync(playerChampionId);
        return matchups.Select(MapToDto);
    }

    public async Task<MatchupDto> CreateMatchupAsync(CreateMatchupRequest request)
    {
        // Validar que los campeones existan
        var playerChampion = await _championRepository.GetByIdAsync(request.PlayerChampionId);
        if (playerChampion == null)
            throw new ArgumentException($"Champion con ID {request.PlayerChampionId} no encontrado");

        var enemyChampion = await _championRepository.GetByIdAsync(request.EnemyChampionId);
        if (enemyChampion == null)
            throw new ArgumentException($"Champion con ID {request.EnemyChampionId} no encontrado");

        // Validar que no exista ya este matchup
        var existingMatchup = await _matchupRepository.GetByChampionsAndRoleAsync(
            request.PlayerChampionId,
            request.EnemyChampionId,
            request.RoleId);

        if (existingMatchup != null)
            throw new InvalidOperationException("Este matchup ya existe");

        var matchup = new Matchup
        {
            PlayerChampionId = request.PlayerChampionId,
            EnemyChampionId = request.EnemyChampionId,
            RoleId = request.RoleId,
            Difficulty = request.Difficulty,
            GeneralAdvice = request.GeneralAdvice
        };

        var createdMatchup = await _matchupRepository.CreateAsync(matchup);
        _logger.LogInformation("Matchup creado: {PlayerChampionId} vs {EnemyChampionId} en {RoleId}",
            request.PlayerChampionId, request.EnemyChampionId, request.RoleId);

        // Recargar con las relaciones
        var fullMatchup = await _matchupRepository.GetByIdAsync(createdMatchup.Id);
        return MapToDto(fullMatchup!);
    }

    public async Task<MatchupDto> AddTipToMatchupAsync(CreateMatchupTipRequest request)
    {
        // Validar que el matchup exista
        var matchup = await _matchupRepository.GetByIdAsync(request.MatchupId);
        if (matchup == null)
            throw new ArgumentException($"Matchup con ID {request.MatchupId} no encontrado");

        var tip = new MatchupTip
        {
            MatchupId = request.MatchupId,
            Category = request.Category,
            Content = request.Content,
            Priority = request.Priority,
            AuthorName = request.AuthorName
        };

        await _tipRepository.CreateAsync(tip);
        _logger.LogInformation("Tip agregado al matchup {MatchupId}", request.MatchupId);

        // Recargar el matchup completo con el nuevo tip
        var updatedMatchup = await _matchupRepository.GetByIdAsync(request.MatchupId);
        return MapToDto(updatedMatchup!);
    }

    public async Task DeleteMatchupAsync(int id)
    {
        await _matchupRepository.DeleteAsync(id);
        _logger.LogInformation("Matchup {MatchupId} eliminado", id);
    }

    /// <summary>
    /// Mapea una entidad Matchup a un DTO
    /// </summary>
    private static MatchupDto MapToDto(Matchup matchup)
    {
        return new MatchupDto
        {
            Id = matchup.Id,
            PlayerChampion = new ChampionDto
            {
                Id = matchup.PlayerChampion.Id,
                RiotChampionId = matchup.PlayerChampion.RiotChampionId,
                Name = matchup.PlayerChampion.Name,
                Title = matchup.PlayerChampion.Title,
                ImageUrl = matchup.PlayerChampion.ImageUrl,
                Description = matchup.PlayerChampion.Description,
                PrimaryRoleId = matchup.PlayerChampion.PrimaryRoleId,
                PrimaryRoleName = matchup.PlayerChampion.PrimaryRole?.Name
            },
            EnemyChampion = new ChampionDto
            {
                Id = matchup.EnemyChampion.Id,
                RiotChampionId = matchup.EnemyChampion.RiotChampionId,
                Name = matchup.EnemyChampion.Name,
                Title = matchup.EnemyChampion.Title,
                ImageUrl = matchup.EnemyChampion.ImageUrl,
                Description = matchup.EnemyChampion.Description,
                PrimaryRoleId = matchup.EnemyChampion.PrimaryRoleId,
                PrimaryRoleName = matchup.EnemyChampion.PrimaryRole?.Name
            },
            Role = new RoleDto
            {
                Id = matchup.Role.Id,
                Name = matchup.Role.Name,
                Description = matchup.Role.Description
            },
            Difficulty = matchup.Difficulty,
            GeneralAdvice = matchup.GeneralAdvice,
            Tips = matchup.Tips.Select(t => new MatchupTipDto
            {
                Id = t.Id,
                MatchupId = t.MatchupId,
                Category = t.Category,
                Content = t.Content,
                Priority = t.Priority,
                AuthorName = t.AuthorName,
                CreatedAt = t.CreatedAt
            }).ToList(),
            CreatedAt = matchup.CreatedAt,
            UpdatedAt = matchup.UpdatedAt
        };
    }
}
