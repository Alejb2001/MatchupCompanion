using MatchupCompanion.API.Data.Repositories.Interfaces;
using MatchupCompanion.API.Models.DTOs;
using MatchupCompanion.API.Services.Interfaces;

namespace MatchupCompanion.API.Services;

/// <summary>
/// Servicio de lógica de negocio para Campeones
/// </summary>
public class ChampionService : IChampionService
{
    private readonly IChampionRepository _championRepository;
    private readonly ILogger<ChampionService> _logger;

    public ChampionService(IChampionRepository championRepository, ILogger<ChampionService> logger)
    {
        _championRepository = championRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<ChampionDto>> GetAllChampionsAsync()
    {
        var champions = await _championRepository.GetAllAsync();
        return champions.Select(c => new ChampionDto
        {
            Id = c.Id,
            RiotChampionId = c.RiotChampionId,
            Name = c.Name,
            Title = c.Title,
            ImageUrl = c.ImageUrl,
            Description = c.Description,
            PrimaryRoleId = c.PrimaryRoleId,
            PrimaryRoleName = c.PrimaryRole?.Name
        });
    }

    public async Task<ChampionDto?> GetChampionByIdAsync(int id)
    {
        var champion = await _championRepository.GetByIdAsync(id);
        if (champion == null)
            return null;

        return new ChampionDto
        {
            Id = champion.Id,
            RiotChampionId = champion.RiotChampionId,
            Name = champion.Name,
            Title = champion.Title,
            ImageUrl = champion.ImageUrl,
            Description = champion.Description,
            PrimaryRoleId = champion.PrimaryRoleId,
            PrimaryRoleName = champion.PrimaryRole?.Name
        };
    }

    public async Task<IEnumerable<ChampionDto>> GetChampionsByRoleAsync(int roleId)
    {
        var champions = await _championRepository.GetByRoleAsync(roleId);
        return champions.Select(c => new ChampionDto
        {
            Id = c.Id,
            RiotChampionId = c.RiotChampionId,
            Name = c.Name,
            Title = c.Title,
            ImageUrl = c.ImageUrl,
            Description = c.Description,
            PrimaryRoleId = c.PrimaryRoleId,
            PrimaryRoleName = c.PrimaryRole?.Name
        });
    }
}
