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

    public async Task<MatchupDto> CreateMatchupAsync(CreateMatchupRequest request, string userId)
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
            GeneralAdvice = request.GeneralAdvice,
            CreatedById = userId  // Asignar el usuario que crea el matchup
        };

        var createdMatchup = await _matchupRepository.CreateAsync(matchup);
        _logger.LogInformation("Matchup creado por usuario {UserId}: {PlayerChampionId} vs {EnemyChampionId} en {RoleId}",
            userId, request.PlayerChampionId, request.EnemyChampionId, request.RoleId);

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

    public async Task<MatchupDto> UpdateMatchupAsync(int id, UpdateMatchupRequest request)
    {
        var matchup = await _matchupRepository.GetByIdAsync(id);
        if (matchup == null)
            throw new ArgumentException($"Matchup con ID {id} no encontrado");

        // Actualizar campos básicos
        matchup.Difficulty = request.Difficulty;
        matchup.GeneralAdvice = request.GeneralAdvice;

        // Actualizar runas
        matchup.PrimaryTreeId = request.PrimaryTreeId;
        matchup.KeystoneId = request.KeystoneId;
        matchup.PrimaryRune1Id = request.PrimaryRune1Id;
        matchup.PrimaryRune2Id = request.PrimaryRune2Id;
        matchup.PrimaryRune3Id = request.PrimaryRune3Id;
        matchup.SecondaryTreeId = request.SecondaryTreeId;
        matchup.SecondaryRune1Id = request.SecondaryRune1Id;
        matchup.SecondaryRune2Id = request.SecondaryRune2Id;
        matchup.StatShards = request.StatShards;

        // Actualizar items
        matchup.StartingItems = request.StartingItems;
        matchup.CoreItems = request.CoreItems;
        matchup.SituationalItems = request.SituationalItems;
        matchup.FullBuildItems = request.FullBuildItems;

        // Actualizar hechizos de invocador
        matchup.SummonerSpell1Id = request.SummonerSpell1Id;
        matchup.SummonerSpell2Id = request.SummonerSpell2Id;

        // Actualizar orden de habilidades
        matchup.AbilityOrder = request.AbilityOrder;

        // Actualizar estrategia
        matchup.Strategy = request.Strategy;

        matchup.UpdatedAt = DateTime.UtcNow;

        await _matchupRepository.UpdateAsync(matchup);
        _logger.LogInformation("Matchup {MatchupId} actualizado", id);

        // Recargar con relaciones
        var updatedMatchup = await _matchupRepository.GetByIdAsync(id);
        return MapToDto(updatedMatchup!);
    }

    public async Task<MatchupDto> GetOrCreateMatchupAsync(int playerChampionId, int enemyChampionId, int roleId)
    {
        // Buscar matchup existente
        var existingMatchup = await _matchupRepository.GetByChampionsAndRoleAsync(
            playerChampionId, enemyChampionId, roleId);

        if (existingMatchup != null)
            return MapToDto(existingMatchup);

        // Validar que los campeones existan
        var playerChampion = await _championRepository.GetByIdAsync(playerChampionId);
        if (playerChampion == null)
            throw new ArgumentException($"Champion con ID {playerChampionId} no encontrado");

        var enemyChampion = await _championRepository.GetByIdAsync(enemyChampionId);
        if (enemyChampion == null)
            throw new ArgumentException($"Champion con ID {enemyChampionId} no encontrado");

        // Crear nuevo matchup vacío
        var matchup = new Matchup
        {
            PlayerChampionId = playerChampionId,
            EnemyChampionId = enemyChampionId,
            RoleId = roleId,
            Difficulty = "Medium", // Valor por defecto
            GeneralAdvice = null
        };

        var createdMatchup = await _matchupRepository.CreateAsync(matchup);
        _logger.LogInformation("Matchup creado (GetOrCreate): {PlayerChampionId} vs {EnemyChampionId} en {RoleId}",
            playerChampionId, enemyChampionId, roleId);

        // Recargar con las relaciones
        var fullMatchup = await _matchupRepository.GetByIdAsync(createdMatchup.Id);
        return MapToDto(fullMatchup!);
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
                PrimaryRoleName = matchup.PlayerChampion.PrimaryRole?.Name,
                QSpellId = matchup.PlayerChampion.QSpellId,
                QSpellName = matchup.PlayerChampion.QSpellName,
                QSpellIcon = matchup.PlayerChampion.QSpellIcon,
                WSpellId = matchup.PlayerChampion.WSpellId,
                WSpellName = matchup.PlayerChampion.WSpellName,
                WSpellIcon = matchup.PlayerChampion.WSpellIcon,
                ESpellId = matchup.PlayerChampion.ESpellId,
                ESpellName = matchup.PlayerChampion.ESpellName,
                ESpellIcon = matchup.PlayerChampion.ESpellIcon,
                RSpellId = matchup.PlayerChampion.RSpellId,
                RSpellName = matchup.PlayerChampion.RSpellName,
                RSpellIcon = matchup.PlayerChampion.RSpellIcon
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
                PrimaryRoleName = matchup.EnemyChampion.PrimaryRole?.Name,
                QSpellId = matchup.EnemyChampion.QSpellId,
                QSpellName = matchup.EnemyChampion.QSpellName,
                QSpellIcon = matchup.EnemyChampion.QSpellIcon,
                WSpellId = matchup.EnemyChampion.WSpellId,
                WSpellName = matchup.EnemyChampion.WSpellName,
                WSpellIcon = matchup.EnemyChampion.WSpellIcon,
                ESpellId = matchup.EnemyChampion.ESpellId,
                ESpellName = matchup.EnemyChampion.ESpellName,
                ESpellIcon = matchup.EnemyChampion.ESpellIcon,
                RSpellId = matchup.EnemyChampion.RSpellId,
                RSpellName = matchup.EnemyChampion.RSpellName,
                RSpellIcon = matchup.EnemyChampion.RSpellIcon
            },
            Role = new RoleDto
            {
                Id = matchup.Role.Id,
                Name = matchup.Role.Name,
                Description = matchup.Role.Description
            },
            Difficulty = matchup.Difficulty,
            GeneralAdvice = matchup.GeneralAdvice,

            // Runas
            PrimaryTreeId = matchup.PrimaryTreeId,
            KeystoneId = matchup.KeystoneId,
            PrimaryRune1Id = matchup.PrimaryRune1Id,
            PrimaryRune2Id = matchup.PrimaryRune2Id,
            PrimaryRune3Id = matchup.PrimaryRune3Id,
            SecondaryTreeId = matchup.SecondaryTreeId,
            SecondaryRune1Id = matchup.SecondaryRune1Id,
            SecondaryRune2Id = matchup.SecondaryRune2Id,
            StatShards = matchup.StatShards,

            // Items
            StartingItems = matchup.StartingItems,
            CoreItems = matchup.CoreItems,
            SituationalItems = matchup.SituationalItems,
            FullBuildItems = matchup.FullBuildItems,

            // Hechizos de invocador
            SummonerSpell1Id = matchup.SummonerSpell1Id,
            SummonerSpell2Id = matchup.SummonerSpell2Id,

            // Orden de habilidades
            AbilityOrder = matchup.AbilityOrder,

            // Estrategia
            Strategy = matchup.Strategy,

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
            CreatedById = matchup.CreatedById ?? string.Empty,
            CreatedAt = matchup.CreatedAt,
            UpdatedAt = matchup.UpdatedAt
        };
    }
}
