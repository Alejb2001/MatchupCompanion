using System.Text.Json;
using MatchupCompanion.API.Data.Repositories.Interfaces;
using MatchupCompanion.Shared.Models;
using Microsoft.AspNetCore.Mvc;

namespace MatchupCompanion.API.Controllers;

/// <summary>
/// Controlador para gestionar items
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ItemsController : ControllerBase
{
    private readonly IItemRepository _itemRepository;
    private readonly ILogger<ItemsController> _logger;
    private const string DataDragonBaseUrl = "https://ddragon.leagueoflegends.com";
    private const string CurrentVersion = "14.24.1"; // TODO: Obtener dinámicamente

    public ItemsController(IItemRepository itemRepository, ILogger<ItemsController> logger)
    {
        _itemRepository = itemRepository;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene todos los items comprables
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllItems()
    {
        var items = await _itemRepository.GetAllAsync();
        var itemDtos = items.Select(MapToDto).ToList();
        return Ok(itemDtos);
    }

    /// <summary>
    /// Obtiene un item por su ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetItemById(int id)
    {
        var item = await _itemRepository.GetByIdAsync(id);
        if (item == null)
            return NotFound(new { message = $"Item con ID {id} no encontrado" });

        return Ok(MapToDto(item));
    }

    /// <summary>
    /// Obtiene un item por su ID de Riot
    /// </summary>
    [HttpGet("riot/{riotItemId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetItemByRiotId(int riotItemId)
    {
        var item = await _itemRepository.GetByRiotIdAsync(riotItemId);
        if (item == null)
            return NotFound(new { message = $"Item con Riot ID {riotItemId} no encontrado" });

        return Ok(MapToDto(item));
    }

    /// <summary>
    /// Obtiene solo los items completos (legendarios/míticos)
    /// </summary>
    [HttpGet("completed")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCompletedItems()
    {
        var items = await _itemRepository.GetCompletedItemsAsync();
        var itemDtos = items.Select(MapToDto).ToList();
        return Ok(itemDtos);
    }

    /// <summary>
    /// Busca items por nombre
    /// </summary>
    [HttpGet("search")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchItems([FromQuery] string q)
    {
        if (string.IsNullOrWhiteSpace(q))
            return Ok(new List<ItemDto>());

        var items = await _itemRepository.SearchByNameAsync(q);
        var itemDtos = items.Select(MapToDto).ToList();
        return Ok(itemDtos);
    }

    /// <summary>
    /// Obtiene múltiples items por sus IDs de Riot (separados por coma)
    /// </summary>
    [HttpGet("by-riot-ids")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetItemsByRiotIds([FromQuery] string ids)
    {
        if (string.IsNullOrWhiteSpace(ids))
            return Ok(new List<ItemDto>());

        var riotIds = ids.Split(',')
            .Select(id => int.TryParse(id.Trim(), out int parsed) ? parsed : (int?)null)
            .Where(id => id.HasValue)
            .Select(id => id!.Value)
            .ToList();

        var items = new List<ItemDto>();
        foreach (var riotId in riotIds)
        {
            var item = await _itemRepository.GetByRiotIdAsync(riotId);
            if (item != null)
                items.Add(MapToDto(item));
        }

        return Ok(items);
    }

    private ItemDto MapToDto(Models.Entities.Item item)
    {
        List<string>? tags = null;
        if (!string.IsNullOrEmpty(item.Tags))
        {
            try
            {
                tags = JsonSerializer.Deserialize<List<string>>(item.Tags);
            }
            catch
            {
                tags = null;
            }
        }

        return new ItemDto
        {
            Id = item.Id,
            RiotItemId = item.RiotItemId,
            Name = item.Name,
            Description = item.Description,
            IconPath = item.IconPath,
            IconUrl = !string.IsNullOrEmpty(item.IconPath)
                ? $"{DataDragonBaseUrl}/cdn/{CurrentVersion}/img/item/{item.IconPath}"
                : null,
            TotalGold = item.TotalGold,
            Tags = tags,
            IsPurchasable = item.IsPurchasable,
            IsCompleted = item.IsCompleted
        };
    }
}
