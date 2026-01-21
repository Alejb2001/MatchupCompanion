using MatchupCompanion.API.Data.Repositories;
using MatchupCompanion.Shared.Models;

namespace MatchupCompanion.API.Services;

/// <summary>
/// Servicio para gestionar hechizos de invocador
/// </summary>
public class SummonerSpellService
{
    private readonly SummonerSpellRepository _repository;

    public SummonerSpellService(SummonerSpellRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Obtiene todos los hechizos de invocador
    /// </summary>
    public async Task<List<SummonerSpellDto>> GetAllSummonerSpellsAsync()
    {
        var spells = await _repository.GetAllAsync();
        return spells.Select(MapToDto).ToList();
    }

    /// <summary>
    /// Obtiene un hechizo por ID
    /// </summary>
    public async Task<SummonerSpellDto?> GetSummonerSpellByIdAsync(int id)
    {
        var spell = await _repository.GetByIdAsync(id);
        return spell != null ? MapToDto(spell) : null;
    }

    /// <summary>
    /// Mapea una entidad SummonerSpell a DTO
    /// </summary>
    private static SummonerSpellDto MapToDto(Models.Entities.SummonerSpell spell)
    {
        return new SummonerSpellDto
        {
            Id = spell.Id,
            RiotSpellId = spell.RiotSpellId,
            Name = spell.Name,
            Description = spell.Description,
            ImageUrl = spell.ImageUrl,
            Cooldown = spell.Cooldown
        };
    }
}
