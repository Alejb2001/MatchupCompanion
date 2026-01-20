using MatchupCompanion.Shared.Models.Auth;

namespace MatchupCompanion.Client.Services.Auth;

public interface IAuthenticationService
{
    Task<AuthResponse?> LoginAsync(LoginRequest request);
    Task<AuthResponse?> RegisterAsync(RegisterRequest request);
    Task<AuthResponse?> LoginAsGuestAsync();
    Task LogoutAsync();
    Task<UserDto?> GetCurrentUserAsync();
    Task<bool> IsAuthenticatedAsync();
    Task<bool> IsGuestAsync();
    string? GetToken();
}
