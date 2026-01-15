using MatchupCompanion.API.Data.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MatchupCompanion.API.Controllers;

/// <summary>
/// Controlador para gestionar roles/líneas
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class RolesController : ControllerBase
{
    private readonly IRoleRepository _roleRepository;
    private readonly ILogger<RolesController> _logger;

    public RolesController(IRoleRepository roleRepository, ILogger<RolesController> logger)
    {
        _roleRepository = roleRepository;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene todos los roles disponibles
    /// </summary>
    /// <returns>Lista de roles</returns>
    /// <response code="200">Retorna la lista de roles</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllRoles()
    {
        var roles = await _roleRepository.GetAllAsync();
        return Ok(roles);
    }

    /// <summary>
    /// Obtiene un rol por su ID
    /// </summary>
    /// <param name="id">ID del rol</param>
    /// <returns>Rol solicitado</returns>
    /// <response code="200">Retorna el rol solicitado</response>
    /// <response code="404">Si el rol no existe</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetRoleById(int id)
    {
        var role = await _roleRepository.GetByIdAsync(id);

        if (role == null)
        {
            _logger.LogWarning("Rol con ID {RoleId} no encontrado", id);
            return NotFound(new { message = $"Rol con ID {id} no encontrado" });
        }

        return Ok(role);
    }
}
