using MatchupCompanion.API.ExternalServices;
using Microsoft.AspNetCore.Mvc;

namespace MatchupCompanion.API.Controllers;

/// <summary>
/// Controlador para sincronizar datos desde la API de Riot Games
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class RiotSyncController : ControllerBase
{
    private readonly RiotApiService _riotApiService;
    private readonly ILogger<RiotSyncController> _logger;

    public RiotSyncController(RiotApiService riotApiService, ILogger<RiotSyncController> logger)
    {
        _riotApiService = riotApiService;
        _logger = logger;
    }

    /// <summary>
    /// Sincroniza los campeones desde la API de Riot Games (Data Dragon)
    /// </summary>
    /// <param name="language">Idioma (por defecto: en_US, español: es_MX)</param>
    /// <returns>Número de campeones sincronizados</returns>
    /// <response code="200">Sincronización completada exitosamente</response>
    /// <response code="500">Error durante la sincronización</response>
    [HttpPost("sync-champions")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SyncChampions([FromQuery] string language = "en_US")
    {
        try
        {
            _logger.LogInformation("Iniciando sincronización de campeones desde Riot API");
            var count = await _riotApiService.SyncChampionsFromRiotAsync(language);

            return Ok(new
            {
                message = "Sincronización completada exitosamente",
                championsSynced = count,
                language
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al sincronizar campeones desde Riot API");
            return StatusCode(500, new
            {
                message = "Error al sincronizar campeones",
                error = ex.Message
            });
        }
    }

    /// <summary>
    /// Obtiene la versión actual de Data Dragon
    /// </summary>
    /// <returns>Versión actual</returns>
    /// <response code="200">Retorna la versión actual</response>
    [HttpGet("version")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCurrentVersion()
    {
        var version = await _riotApiService.GetLatestVersionAsync();
        return Ok(new { version });
    }

    /// <summary>
    /// Sincroniza las runas desde la API de Riot Games (Data Dragon)
    /// </summary>
    /// <param name="language">Idioma (por defecto: en_US)</param>
    /// <returns>Número de runas sincronizadas</returns>
    [HttpPost("sync-runes")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SyncRunes([FromQuery] string language = "en_US")
    {
        try
        {
            _logger.LogInformation("Iniciando sincronización de runas desde Riot API");
            var count = await _riotApiService.SyncRunesFromRiotAsync(language);

            return Ok(new
            {
                message = "Sincronización de runas completada exitosamente",
                runesSynced = count,
                language
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al sincronizar runas desde Riot API");
            return StatusCode(500, new
            {
                message = "Error al sincronizar runas",
                error = ex.Message
            });
        }
    }

    /// <summary>
    /// Sincroniza los items desde la API de Riot Games (Data Dragon)
    /// </summary>
    /// <param name="language">Idioma (por defecto: en_US)</param>
    /// <returns>Número de items sincronizados</returns>
    [HttpPost("sync-items")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SyncItems([FromQuery] string language = "en_US")
    {
        try
        {
            _logger.LogInformation("Iniciando sincronización de items desde Riot API");
            var count = await _riotApiService.SyncItemsFromRiotAsync(language);

            return Ok(new
            {
                message = "Sincronización de items completada exitosamente",
                itemsSynced = count,
                language
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al sincronizar items desde Riot API");
            return StatusCode(500, new
            {
                message = "Error al sincronizar items",
                error = ex.Message
            });
        }
    }

    /// <summary>
    /// Sincroniza todo: campeones, runas e items desde Data Dragon
    /// </summary>
    /// <param name="language">Idioma (por defecto: en_US)</param>
    /// <returns>Conteo de elementos sincronizados</returns>
    [HttpPost("sync-all")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SyncAll([FromQuery] string language = "en_US")
    {
        try
        {
            _logger.LogInformation("Iniciando sincronización completa desde Riot API");
            var (champions, runes, items) = await _riotApiService.SyncAllFromRiotAsync(language);

            return Ok(new
            {
                message = "Sincronización completa exitosa",
                championsSynced = champions,
                runesSynced = runes,
                itemsSynced = items,
                language
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al sincronizar desde Riot API");
            var innerMessage = ex.InnerException?.Message ?? "No inner exception";
            return StatusCode(500, new
            {
                message = "Error durante la sincronización",
                error = ex.Message,
                innerError = innerMessage,
                stackTrace = ex.StackTrace
            });
        }
    }
}
