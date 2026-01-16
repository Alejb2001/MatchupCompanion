using System.Text.Json;
using System.Text.Json.Serialization;
using MatchupCompanion.API.Data.Repositories.Interfaces;
using MatchupCompanion.API.Models.Entities;

namespace MatchupCompanion.API.ExternalServices;

/// <summary>
/// Servicio para interactuar con la API de Riot Games
/// Documentación: https://developer.riotgames.com/
/// </summary>
public class RiotApiService
{
    private readonly HttpClient _httpClient;
    private readonly IChampionRepository _championRepository;
    private readonly ILogger<RiotApiService> _logger;
    private readonly string _apiKey;

    // URL base para la API de Data Dragon (datos estáticos de campeones)
    // No requiere API key y es más fácil de usar para obtener información básica
    private const string DataDragonBaseUrl = "https://ddragon.leagueoflegends.com";

    public RiotApiService(
        HttpClient httpClient,
        IConfiguration configuration,
        IChampionRepository championRepository,
        ILogger<RiotApiService> logger)
    {
        _httpClient = httpClient;
        _championRepository = championRepository;
        _logger = logger;
        _apiKey = configuration["RiotApi:ApiKey"] ?? string.Empty;
    }

    /// <summary>
    /// Obtiene la versión más reciente de Data Dragon
    /// </summary>
    public async Task<string> GetLatestVersionAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync($"{DataDragonBaseUrl}/api/versions.json");
            response.EnsureSuccessStatusCode();

            var versions = await response.Content.ReadFromJsonAsync<List<string>>();
            return versions?.FirstOrDefault() ?? "14.1.1";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener la versión más reciente de Data Dragon");
            return "14.1.1"; // Versión fallback
        }
    }

    /// <summary>
    /// Obtiene todos los campeones desde Data Dragon y los sincroniza con la base de datos
    /// </summary>
    /// <param name="language">Idioma (ej: "es_MX", "en_US")</param>
    public async Task<int> SyncChampionsFromRiotAsync(string language = "en_US")
    {
        try
        {
            var version = await GetLatestVersionAsync();
            var url = $"{DataDragonBaseUrl}/cdn/{version}/data/{language}/champion.json";

            _logger.LogInformation("Obteniendo campeones desde {Url}", url);

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var jsonString = await response.Content.ReadAsStringAsync();
            var championData = JsonSerializer.Deserialize<RiotChampionResponse>(jsonString);

            if (championData?.Data == null)
            {
                _logger.LogWarning("No se encontraron datos de campeones");
                return 0;
            }

            int syncedCount = 0;

            foreach (var (key, champion) in championData.Data)
            {
                // Verificar si el campeón ya existe en nuestra base de datos
                var existingChampion = await _championRepository.GetByRiotIdAsync(champion.Key);

                if (existingChampion == null)
                {
                    // Crear nuevo campeón
                    var newChampion = new Champion
                    {
                        RiotChampionId = champion.Key,
                        Name = champion.Name,
                        Title = champion.Title,
                        Description = champion.Blurb,
                        ImageUrl = $"{DataDragonBaseUrl}/cdn/{version}/img/champion/{champion.Image.Full}",
                        // Mapear roles (esto es una simplificación, puede mejorarse)
                        PrimaryRoleId = MapRoleToId(champion.Tags.FirstOrDefault())
                    };

                    await _championRepository.CreateAsync(newChampion);
                    syncedCount++;
                    _logger.LogInformation("Campeón creado: {ChampionName}", newChampion.Name);
                }
                else
                {
                    // Actualizar campeón existente
                    existingChampion.Name = champion.Name;
                    existingChampion.Title = champion.Title;
                    existingChampion.Description = champion.Blurb;
                    existingChampion.ImageUrl = $"{DataDragonBaseUrl}/cdn/{version}/img/champion/{champion.Image.Full}";

                    await _championRepository.UpdateAsync(existingChampion);
                    syncedCount++;
                    _logger.LogInformation("Campeón actualizado: {ChampionName}", existingChampion.Name);
                }
            }

            _logger.LogInformation("Sincronización completada. {Count} campeones sincronizados", syncedCount);
            return syncedCount;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al sincronizar campeones desde Riot API");
            throw;
        }
    }

    /// <summary>
    /// Mapea un tag de rol de Riot a nuestro ID de rol interno
    /// </summary>
    private int? MapRoleToId(string? riotRole)
    {
        return riotRole?.ToLower() switch
        {
            "fighter" => 1,      // Top
            "tank" => 1,         // Top
            "assassin" => 3,     // Mid
            "mage" => 3,         // Mid
            "marksman" => 4,     // ADC
            "support" => 5,      // Support
            _ => null
        };
    }

    #region Clases de respuesta de la API de Riot (Data Dragon)

    private class RiotChampionResponse
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        [JsonPropertyName("format")]
        public string Format { get; set; } = string.Empty;

        [JsonPropertyName("version")]
        public string Version { get; set; } = string.Empty;

        [JsonPropertyName("data")]
        public Dictionary<string, RiotChampion> Data { get; set; } = new();
    }

    private class RiotChampion
    {
        [JsonPropertyName("version")]
        public string Version { get; set; } = string.Empty;

        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("key")]
        public string Key { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("blurb")]
        public string Blurb { get; set; } = string.Empty;

        [JsonPropertyName("image")]
        public RiotChampionImage Image { get; set; } = new();

        [JsonPropertyName("tags")]
        public List<string> Tags { get; set; } = new();

        [JsonPropertyName("partype")]
        public string Partype { get; set; } = string.Empty;
    }

    private class RiotChampionImage
    {
        [JsonPropertyName("full")]
        public string Full { get; set; } = string.Empty;

        [JsonPropertyName("sprite")]
        public string Sprite { get; set; } = string.Empty;

        [JsonPropertyName("group")]
        public string Group { get; set; } = string.Empty;

        [JsonPropertyName("x")]
        public int X { get; set; }

        [JsonPropertyName("y")]
        public int Y { get; set; }

        [JsonPropertyName("w")]
        public int W { get; set; }

        [JsonPropertyName("h")]
        public int H { get; set; }
    }

    #endregion
}
