using MatchupCompanion.API.Models.DTOs;
using MatchupCompanion.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MatchupCompanion.API.Controllers;

/// <summary>
/// Controlador para gestionar matchups y sus consejos
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class MatchupsController : ControllerBase
{
    private readonly IMatchupService _matchupService;
    private readonly ILogger<MatchupsController> _logger;

    public MatchupsController(IMatchupService matchupService, ILogger<MatchupsController> logger)
    {
        _matchupService = matchupService;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene todos los matchups
    /// </summary>
    /// <returns>Lista de matchups</returns>
    /// <response code="200">Retorna la lista de matchups</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllMatchups()
    {
        var matchups = await _matchupService.GetAllMatchupsAsync();
        return Ok(matchups);
    }

    /// <summary>
    /// Obtiene un matchup por su ID
    /// </summary>
    /// <param name="id">ID del matchup</param>
    /// <returns>Matchup con todos sus consejos</returns>
    /// <response code="200">Retorna el matchup solicitado</response>
    /// <response code="404">Si el matchup no existe</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMatchupById(int id)
    {
        var matchup = await _matchupService.GetMatchupByIdAsync(id);

        if (matchup == null)
        {
            _logger.LogWarning("Matchup con ID {MatchupId} no encontrado", id);
            return NotFound(new { message = $"Matchup con ID {id} no encontrado" });
        }

        return Ok(matchup);
    }

    /// <summary>
    /// Busca un matchup específico por campeones y rol
    /// </summary>
    /// <param name="playerChampionId">ID del campeón del jugador</param>
    /// <param name="enemyChampionId">ID del campeón enemigo</param>
    /// <param name="roleId">ID del rol</param>
    /// <returns>Matchup si existe</returns>
    /// <response code="200">Retorna el matchup encontrado</response>
    /// <response code="404">Si el matchup no existe</response>
    [HttpGet("search")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SearchMatchup(
        [FromQuery] int playerChampionId,
        [FromQuery] int enemyChampionId,
        [FromQuery] int roleId)
    {
        var matchup = await _matchupService.GetMatchupByChampionsAndRoleAsync(
            playerChampionId,
            enemyChampionId,
            roleId);

        if (matchup == null)
        {
            return NotFound(new
            {
                message = "No se encontró información para este matchup",
                playerChampionId,
                enemyChampionId,
                roleId
            });
        }

        return Ok(matchup);
    }

    /// <summary>
    /// Obtiene todos los matchups para un campeón específico
    /// </summary>
    /// <param name="championId">ID del campeón</param>
    /// <returns>Lista de matchups del campeón</returns>
    /// <response code="200">Retorna los matchups del campeón</response>
    [HttpGet("champion/{championId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMatchupsByChampion(int championId)
    {
        var matchups = await _matchupService.GetMatchupsByPlayerChampionAsync(championId);
        return Ok(matchups);
    }

    /// <summary>
    /// Crea un nuevo matchup
    /// </summary>
    /// <param name="request">Datos del matchup a crear</param>
    /// <returns>Matchup creado</returns>
    /// <response code="201">Retorna el matchup creado</response>
    /// <response code="400">Si los datos son inválidos</response>
    /// <response code="409">Si el matchup ya existe</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateMatchup([FromBody] CreateMatchupRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var matchup = await _matchupService.CreateMatchupAsync(request);
            return CreatedAtAction(
                nameof(GetMatchupById),
                new { id = matchup.Id },
                matchup);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Error de validación al crear matchup");
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Matchup duplicado");
            return Conflict(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Agrega un consejo a un matchup existente
    /// </summary>
    /// <param name="request">Datos del consejo a agregar</param>
    /// <returns>Matchup actualizado con el nuevo consejo</returns>
    /// <response code="200">Retorna el matchup actualizado</response>
    /// <response code="400">Si los datos son inválidos</response>
    /// <response code="404">Si el matchup no existe</response>
    [HttpPost("tips")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddTipToMatchup([FromBody] CreateMatchupTipRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var matchup = await _matchupService.AddTipToMatchupAsync(request);
            return Ok(matchup);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Error al agregar tip al matchup");
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Elimina un matchup
    /// </summary>
    /// <param name="id">ID del matchup a eliminar</param>
    /// <returns>No content</returns>
    /// <response code="204">Matchup eliminado exitosamente</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteMatchup(int id)
    {
        await _matchupService.DeleteMatchupAsync(id);
        return NoContent();
    }
}
