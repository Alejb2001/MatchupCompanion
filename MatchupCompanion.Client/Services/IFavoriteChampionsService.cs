namespace MatchupCompanion.Client.Services;

/// <summary>
/// Servicio para gestionar los campeones favoritos del usuario
/// </summary>
public interface IFavoriteChampionsService
{
    /// <summary>
    /// Obtiene la lista de IDs de campeones favoritos
    /// </summary>
    Task<List<int>> GetFavoriteChampionIdsAsync();

    /// <summary>
    /// Agrega un campeón a favoritos
    /// </summary>
    Task AddFavoriteAsync(int championId);

    /// <summary>
    /// Elimina un campeón de favoritos
    /// </summary>
    Task RemoveFavoriteAsync(int championId);

    /// <summary>
    /// Verifica si un campeón es favorito
    /// </summary>
    Task<bool> IsFavoriteAsync(int championId);

    /// <summary>
    /// Alterna el estado de favorito de un campeón
    /// </summary>
    Task ToggleFavoriteAsync(int championId);
}
