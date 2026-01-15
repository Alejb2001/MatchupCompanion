using MatchupCompanion.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MatchupCompanion.API.Controllers;

/// <summary>
/// Controlador para gestionar campeones
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ChampionsController : ControllerBase
{
    private readonly IChampionService _championService;
    private readonly ILogger<ChampionsController> _logger;

    public ChampionsController(IChampionService championService, ILogger<ChampionsController> logger)
    {
        _championService = championService;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene todos los campeones
    /// </summary>
    /// <returns>Lista de campeones</returns>
    /// <response code="200">Retorna la lista de campeones</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllChampions()
    {
        var champions = await _championService.GetAllChampionsAsync();
        return Ok(champions);
    }

    /// <summary>
    /// Obtiene un campeón por su ID
    /// </summary>
    /// <param name="id">ID del campeón</param>
    /// <returns>Campeón solicitado</returns>
    /// <response code="200">Retorna el campeón solicitado</response>
    /// <response code="404">Si el campeón no existe</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetChampionById(int id)
    {
        var champion = await _championService.GetChampionByIdAsync(id);

        if (champion == null)
        {
            _logger.LogWarning("Campeón con ID {ChampionId} no encontrado", id);
            return NotFound(new { message = $"Campeón con ID {id} no encontrado" });
        }

        return Ok(champion);
    }

    /// <summary>
    /// Obtiene campeones por rol
    /// </summary>
    /// <param name="roleId">ID del rol</param>
    /// <returns>Lista de campeones del rol especificado</returns>
    /// <response code="200">Retorna los campeones del rol</response>
    [HttpGet("role/{roleId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetChampionsByRole(int roleId)
    {
        var champions = await _championService.GetChampionsByRoleAsync(roleId);
        return Ok(champions);
    }
}
