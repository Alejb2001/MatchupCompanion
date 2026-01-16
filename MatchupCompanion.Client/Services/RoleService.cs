using System.Net.Http.Json;
using MatchupCompanion.Shared.Models;

namespace MatchupCompanion.Client.Services;

public class RoleService : IRoleService
{
    private readonly HttpClient _httpClient;

    public RoleService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<RoleDto>> GetAllRolesAsync()
    {
        try
        {
            var roles = await _httpClient.GetFromJsonAsync<List<RoleDto>>("api/Roles");
            return roles ?? new List<RoleDto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al obtener roles: {ex.Message}");
            return new List<RoleDto>();
        }
    }

    public async Task<RoleDto?> GetRoleByIdAsync(int id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<RoleDto>($"api/Roles/{id}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al obtener rol {id}: {ex.Message}");
            return null;
        }
    }
}
