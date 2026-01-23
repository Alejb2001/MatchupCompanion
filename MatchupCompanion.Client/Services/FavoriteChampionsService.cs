using Blazored.LocalStorage;

namespace MatchupCompanion.Client.Services;

/// <summary>
/// Implementación del servicio de campeones favoritos usando localStorage
/// </summary>
public class FavoriteChampionsService : IFavoriteChampionsService
{
    private readonly ILocalStorageService _localStorage;
    private const string STORAGE_KEY = "favoriteChampions";

    public FavoriteChampionsService(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    public async Task<List<int>> GetFavoriteChampionIdsAsync()
    {
        try
        {
            var favorites = await _localStorage.GetItemAsync<List<int>>(STORAGE_KEY);
            return favorites ?? new List<int>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al obtener favoritos: {ex.Message}");
            return new List<int>();
        }
    }

    public async Task AddFavoriteAsync(int championId)
    {
        var favorites = await GetFavoriteChampionIdsAsync();

        if (!favorites.Contains(championId))
        {
            favorites.Add(championId);
            await _localStorage.SetItemAsync(STORAGE_KEY, favorites);
        }
    }

    public async Task RemoveFavoriteAsync(int championId)
    {
        var favorites = await GetFavoriteChampionIdsAsync();

        if (favorites.Contains(championId))
        {
            favorites.Remove(championId);
            await _localStorage.SetItemAsync(STORAGE_KEY, favorites);
        }
    }

    public async Task<bool> IsFavoriteAsync(int championId)
    {
        var favorites = await GetFavoriteChampionIdsAsync();
        return favorites.Contains(championId);
    }

    public async Task ToggleFavoriteAsync(int championId)
    {
        if (await IsFavoriteAsync(championId))
        {
            await RemoveFavoriteAsync(championId);
        }
        else
        {
            await AddFavoriteAsync(championId);
        }
    }
}
