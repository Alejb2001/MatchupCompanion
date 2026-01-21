using Blazored.LocalStorage;
using System.Net.Http.Headers;

namespace MatchupCompanion.Client.Handlers;

public class AuthorizationMessageHandler : DelegatingHandler
{
    private readonly ILocalStorageService _localStorage;
    private const string TOKEN_KEY = "authToken";

    public AuthorizationMessageHandler(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        // Obtener el token del local storage
        var token = await _localStorage.GetItemAsync<string>(TOKEN_KEY);

        // Si existe un token, añadirlo al header de autorización
        if (!string.IsNullOrEmpty(token))
        {
            // Limpiar comillas si existen (GetItemAsStringAsync puede devolver "token" en lugar de token)
            token = token.Trim('"');
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            Console.WriteLine($"Adding Authorization header to: {request.RequestUri}");
        }
        else
        {
            Console.WriteLine($"No token found for request to: {request.RequestUri}");
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
