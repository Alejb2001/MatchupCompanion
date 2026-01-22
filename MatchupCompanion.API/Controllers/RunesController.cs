using MatchupCompanion.API.Data.Repositories.Interfaces;
using MatchupCompanion.Shared.Models;
using Microsoft.AspNetCore.Mvc;

namespace MatchupCompanion.API.Controllers;

/// <summary>
/// Controlador para gestionar runas
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class RunesController : ControllerBase
{
    private readonly IRuneRepository _runeRepository;
    private readonly ILogger<RunesController> _logger;
    private const string DataDragonBaseUrl = "https://ddragon.leagueoflegends.com";

    public RunesController(IRuneRepository runeRepository, ILogger<RunesController> logger)
    {
        _runeRepository = runeRepository;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene todas las runas
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllRunes()
    {
        var runes = await _runeRepository.GetAllAsync();
        var runeDtos = runes.Select(MapToDto).ToList();
        return Ok(runeDtos);
    }

    /// <summary>
    /// Obtiene una runa por su ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetRuneById(int id)
    {
        var rune = await _runeRepository.GetByIdAsync(id);
        if (rune == null)
            return NotFound(new { message = $"Runa con ID {id} no encontrada" });

        return Ok(MapToDto(rune));
    }

    /// <summary>
    /// Obtiene runas por árbol
    /// </summary>
    [HttpGet("tree/{treeId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRunesByTree(int treeId)
    {
        var runes = await _runeRepository.GetByTreeIdAsync(treeId);
        var runeDtos = runes.Select(MapToDto).ToList();
        return Ok(runeDtos);
    }

    /// <summary>
    /// Obtiene runas por árbol y slot
    /// </summary>
    [HttpGet("tree/{treeId}/slot/{slotIndex}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRunesByTreeAndSlot(int treeId, int slotIndex)
    {
        var runes = await _runeRepository.GetByTreeAndSlotAsync(treeId, slotIndex);
        var runeDtos = runes.Select(MapToDto).ToList();
        return Ok(runeDtos);
    }

    /// <summary>
    /// Obtiene todas las keystones
    /// </summary>
    [HttpGet("keystones")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetKeystones()
    {
        var runes = await _runeRepository.GetKeystonesAsync();
        var runeDtos = runes.Select(MapToDto).ToList();
        return Ok(runeDtos);
    }

    /// <summary>
    /// Obtiene todos los árboles de runas con sus runas agrupadas
    /// </summary>
    [HttpGet("trees")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRuneTrees()
    {
        var allRunes = await _runeRepository.GetAllAsync();
        var runesList = allRunes.ToList();

        var trees = runesList
            .GroupBy(r => new { r.TreeId, r.TreeName })
            .Select(g => new RuneTreeDto
            {
                TreeId = g.Key.TreeId,
                TreeName = g.Key.TreeName,
                TreeIconUrl = $"{DataDragonBaseUrl}/cdn/img/perk-images/Styles/{g.Key.TreeId}_{g.Key.TreeName}.png",
                Keystones = g.Where(r => r.SlotIndex == 0).Select(MapToDto).ToList(),
                Slot1Runes = g.Where(r => r.SlotIndex == 1).Select(MapToDto).ToList(),
                Slot2Runes = g.Where(r => r.SlotIndex == 2).Select(MapToDto).ToList(),
                Slot3Runes = g.Where(r => r.SlotIndex == 3).Select(MapToDto).ToList()
            })
            .OrderBy(t => t.TreeName)
            .ToList();

        return Ok(trees);
    }

    private RuneDto MapToDto(Models.Entities.Rune rune)
    {
        return new RuneDto
        {
            Id = rune.Id,
            RiotRuneId = rune.RiotRuneId,
            Key = rune.Key,
            Name = rune.Name,
            IconPath = rune.IconPath,
            IconUrl = !string.IsNullOrEmpty(rune.IconPath)
                ? $"{DataDragonBaseUrl}/cdn/img/{rune.IconPath}"
                : null,
            TreeName = rune.TreeName,
            TreeId = rune.TreeId,
            SlotIndex = rune.SlotIndex,
            ShortDescription = rune.ShortDescription
        };
    }
}
