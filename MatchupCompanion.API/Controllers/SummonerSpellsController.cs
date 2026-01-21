using MatchupCompanion.API.Services;
using MatchupCompanion.Shared.Models;
using Microsoft.AspNetCore.Mvc;

namespace MatchupCompanion.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SummonerSpellsController : ControllerBase
{
    private readonly SummonerSpellService _service;

    public SummonerSpellsController(SummonerSpellService service)
    {
        _service = service;
    }

    /// <summary>
    /// Obtiene todos los hechizos de invocador disponibles
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<SummonerSpellDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<SummonerSpellDto>>> GetAllSummonerSpells()
    {
        var spells = await _service.GetAllSummonerSpellsAsync();
        return Ok(spells);
    }

    /// <summary>
    /// Obtiene un hechizo de invocador por ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(SummonerSpellDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SummonerSpellDto>> GetSummonerSpellById(int id)
    {
        var spell = await _service.GetSummonerSpellByIdAsync(id);

        if (spell == null)
            return NotFound($"Hechizo con ID {id} no encontrado");

        return Ok(spell);
    }
}
