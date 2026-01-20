using Blazored.LocalStorage;
using MatchupCompanion.Shared.Models.Auth;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Json;
using System.Net.Http.Headers;

namespace MatchupCompanion.Client.Services.Auth;

public class AuthenticationService : IAuthenticationService
{
    private readonly HttpClient _httpClient;
    private readonly ILocalStorageService _localStorage;
    private readonly AuthenticationStateProvider _authStateProvider;
    private const string TOKEN_KEY = "authToken";
    private const string USER_KEY = "currentUser";

    public AuthenticationService(
        HttpClient httpClient,
        ILocalStorageService localStorage,
        AuthenticationStateProvider authStateProvider)
    {
        _httpClient = httpClient;
        _localStorage = localStorage;
        _authStateProvider = authStateProvider;
    }

    public async Task<AuthResponse?> LoginAsync(LoginRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/login", request);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
            if (authResponse != null)
            {
                await SaveAuthDataAsync(authResponse);
            }

            return authResponse;
        }
        catch
        {
            return null;
        }
    }

    public async Task<AuthResponse?> RegisterAsync(RegisterRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/register", request);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
            if (authResponse != null)
            {
                await SaveAuthDataAsync(authResponse);
            }

            return authResponse;
        }
        catch
        {
            return null;
        }
    }

    public async Task<AuthResponse?> LoginAsGuestAsync()
    {
        try
        {
            var response = await _httpClient.PostAsync("api/auth/guest", null);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
            if (authResponse != null)
            {
                await SaveAuthDataAsync(authResponse);
            }

            return authResponse;
        }
        catch
        {
            return null;
        }
    }

    public async Task LogoutAsync()
    {
        try
        {
            // Llamar al endpoint de logout (opcional, en JWT el logout es del lado del cliente)
            await _httpClient.PostAsync("api/auth/logout", null);
        }
        catch
        {
            // Ignorar errores al hacer logout en el servidor
        }
        finally
        {
            // Limpiar datos locales
            await _localStorage.RemoveItemAsync(TOKEN_KEY);
            await _localStorage.RemoveItemAsync(USER_KEY);

            // Limpiar header de autorización
            _httpClient.DefaultRequestHeaders.Authorization = null;

            // Notificar cambio de estado de autenticación
            if (_authStateProvider is CustomAuthenticationStateProvider customProvider)
            {
                customProvider.NotifyUserLogout();
            }
        }
    }

    public async Task<UserDto?> GetCurrentUserAsync()
    {
        try
        {
            var token = await _localStorage.GetItemAsync<string>(TOKEN_KEY);
            if (string.IsNullOrEmpty(token))
            {
                return null;
            }

            // Primero intentar obtener del storage local
            var cachedUser = await _localStorage.GetItemAsync<UserDto>(USER_KEY);
            if (cachedUser != null)
            {
                return cachedUser;
            }

            // Si no está en cache, obtener del servidor
            var response = await _httpClient.GetAsync("api/auth/me");
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var user = await response.Content.ReadFromJsonAsync<UserDto>();
            if (user != null)
            {
                await _localStorage.SetItemAsync(USER_KEY, user);
            }

            return user;
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        var token = await _localStorage.GetItemAsync<string>(TOKEN_KEY);
        return !string.IsNullOrEmpty(token);
    }

    public async Task<bool> IsGuestAsync()
    {
        var user = await GetCurrentUserAsync();
        return user?.IsGuest ?? false;
    }

    public string? GetToken()
    {
        // Método síncrono para obtener el token (usado en interceptors)
        // En Blazor WASM no podemos usar métodos async en algunos contextos
        return _localStorage.GetItemAsStringAsync(TOKEN_KEY).GetAwaiter().GetResult();
    }

    private async Task SaveAuthDataAsync(AuthResponse authResponse)
    {
        // Guardar token
        await _localStorage.SetItemAsync(TOKEN_KEY, authResponse.Token);

        // Guardar datos de usuario
        var userDto = new UserDto
        {
            Id = authResponse.UserId,
            Email = authResponse.Email,
            UserName = authResponse.UserName,
            DisplayName = authResponse.DisplayName,
            IsGuest = authResponse.IsGuest,
            Roles = authResponse.Roles
        };
        await _localStorage.SetItemAsync(USER_KEY, userDto);

        // Configurar header de autorización
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", authResponse.Token);

        // Notificar cambio de estado de autenticación
        if (_authStateProvider is CustomAuthenticationStateProvider customProvider)
        {
            customProvider.NotifyUserAuthentication(authResponse.Token);
        }
    }
}
