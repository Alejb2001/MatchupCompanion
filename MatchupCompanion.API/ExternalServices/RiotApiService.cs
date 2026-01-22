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
    private readonly IRuneRepository _runeRepository;
    private readonly IItemRepository _itemRepository;
    private readonly ILogger<RiotApiService> _logger;
    private readonly string _apiKey;

    // URL base para la API de Data Dragon (datos estáticos de campeones)
    // No requiere API key y es más fácil de usar para obtener información básica
    private const string DataDragonBaseUrl = "https://ddragon.leagueoflegends.com";

    public RiotApiService(
        HttpClient httpClient,
        IConfiguration configuration,
        IChampionRepository championRepository,
        IRuneRepository runeRepository,
        IItemRepository itemRepository,
        ILogger<RiotApiService> logger)
    {
        _httpClient = httpClient;
        _championRepository = championRepository;
        _runeRepository = runeRepository;
        _itemRepository = itemRepository;
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
                // Obtener datos detallados del campeón (incluyendo habilidades)
                var detailedChampion = await GetChampionDetailsAsync(champion.Id, language, version);

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
                        PrimaryRoleId = MapRoleToId(champion.Tags.FirstOrDefault()),
                        // Habilidades
                        QSpellId = detailedChampion?.Spells.ElementAtOrDefault(0)?.Id,
                        QSpellName = detailedChampion?.Spells.ElementAtOrDefault(0)?.Name,
                        QSpellIcon = detailedChampion?.Spells.ElementAtOrDefault(0)?.Image.Full != null
                            ? $"{DataDragonBaseUrl}/cdn/{version}/img/spell/{detailedChampion.Spells[0].Image.Full}"
                            : null,
                        WSpellId = detailedChampion?.Spells.ElementAtOrDefault(1)?.Id,
                        WSpellName = detailedChampion?.Spells.ElementAtOrDefault(1)?.Name,
                        WSpellIcon = detailedChampion?.Spells.ElementAtOrDefault(1)?.Image.Full != null
                            ? $"{DataDragonBaseUrl}/cdn/{version}/img/spell/{detailedChampion.Spells[1].Image.Full}"
                            : null,
                        ESpellId = detailedChampion?.Spells.ElementAtOrDefault(2)?.Id,
                        ESpellName = detailedChampion?.Spells.ElementAtOrDefault(2)?.Name,
                        ESpellIcon = detailedChampion?.Spells.ElementAtOrDefault(2)?.Image.Full != null
                            ? $"{DataDragonBaseUrl}/cdn/{version}/img/spell/{detailedChampion.Spells[2].Image.Full}"
                            : null,
                        RSpellId = detailedChampion?.Passive?.Image.Full != null
                            ? detailedChampion.Passive.Name
                            : null,
                        RSpellName = detailedChampion?.Spells.ElementAtOrDefault(3)?.Name,
                        RSpellIcon = detailedChampion?.Spells.ElementAtOrDefault(3)?.Image.Full != null
                            ? $"{DataDragonBaseUrl}/cdn/{version}/img/spell/{detailedChampion.Spells[3].Image.Full}"
                            : null
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

                    // Actualizar habilidades
                    existingChampion.QSpellId = detailedChampion?.Spells.ElementAtOrDefault(0)?.Id;
                    existingChampion.QSpellName = detailedChampion?.Spells.ElementAtOrDefault(0)?.Name;
                    existingChampion.QSpellIcon = detailedChampion?.Spells.ElementAtOrDefault(0)?.Image.Full != null
                        ? $"{DataDragonBaseUrl}/cdn/{version}/img/spell/{detailedChampion.Spells[0].Image.Full}"
                        : null;
                    existingChampion.WSpellId = detailedChampion?.Spells.ElementAtOrDefault(1)?.Id;
                    existingChampion.WSpellName = detailedChampion?.Spells.ElementAtOrDefault(1)?.Name;
                    existingChampion.WSpellIcon = detailedChampion?.Spells.ElementAtOrDefault(1)?.Image.Full != null
                        ? $"{DataDragonBaseUrl}/cdn/{version}/img/spell/{detailedChampion.Spells[1].Image.Full}"
                        : null;
                    existingChampion.ESpellId = detailedChampion?.Spells.ElementAtOrDefault(2)?.Id;
                    existingChampion.ESpellName = detailedChampion?.Spells.ElementAtOrDefault(2)?.Name;
                    existingChampion.ESpellIcon = detailedChampion?.Spells.ElementAtOrDefault(2)?.Image.Full != null
                        ? $"{DataDragonBaseUrl}/cdn/{version}/img/spell/{detailedChampion.Spells[2].Image.Full}"
                        : null;
                    existingChampion.RSpellId = detailedChampion?.Spells.ElementAtOrDefault(3)?.Id;
                    existingChampion.RSpellName = detailedChampion?.Spells.ElementAtOrDefault(3)?.Name;
                    existingChampion.RSpellIcon = detailedChampion?.Spells.ElementAtOrDefault(3)?.Image.Full != null
                        ? $"{DataDragonBaseUrl}/cdn/{version}/img/spell/{detailedChampion.Spells[3].Image.Full}"
                        : null;

                    await _championRepository.UpdateAsync(existingChampion);
                    syncedCount++;
                    _logger.LogInformation("Campeón actualizado: {ChampionName}", existingChampion.Name);
                }

                // Pequeña pausa para no saturar la API
                await Task.Delay(100);
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
    /// Obtiene los detalles completos de un campeón específico (incluyendo habilidades)
    /// </summary>
    private async Task<RiotChampionDetailed?> GetChampionDetailsAsync(string championId, string language, string version)
    {
        try
        {
            var url = $"{DataDragonBaseUrl}/cdn/{version}/data/{language}/champion/{championId}.json";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var jsonString = await response.Content.ReadAsStringAsync();
            var detailedData = JsonSerializer.Deserialize<RiotChampionDetailedResponse>(jsonString);

            return detailedData?.Data?.Values.FirstOrDefault();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "No se pudieron obtener detalles del campeón {ChampionId}", championId);
            return null;
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

    /// <summary>
    /// Elimina tags HTML de un string
    /// </summary>
    private static string StripHtmlTags(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        // Usar regex para eliminar todos los tags HTML
        return System.Text.RegularExpressions.Regex.Replace(input, "<[^>]*>", "").Trim();
    }

    /// <summary>
    /// Sincroniza todas las runas desde Data Dragon
    /// </summary>
    public async Task<int> SyncRunesFromRiotAsync(string language = "en_US")
    {
        try
        {
            var version = await GetLatestVersionAsync();
            var url = $"{DataDragonBaseUrl}/cdn/{version}/data/{language}/runesReforged.json";

            _logger.LogInformation("Obteniendo runas desde {Url}", url);

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var jsonString = await response.Content.ReadAsStringAsync();
            var runeTrees = JsonSerializer.Deserialize<List<RiotRuneTree>>(jsonString);

            if (runeTrees == null || !runeTrees.Any())
            {
                _logger.LogWarning("No se encontraron datos de runas");
                return 0;
            }

            int syncedCount = 0;

            foreach (var tree in runeTrees)
            {
                int slotIndex = 0;
                foreach (var slot in tree.Slots)
                {
                    foreach (var rune in slot.Runes)
                    {
                        var existingRune = await _runeRepository.GetByRiotIdAsync(rune.Id);

                        if (existingRune == null)
                        {
                            var newRune = new Rune
                            {
                                RiotRuneId = rune.Id,
                                Key = rune.Key,
                                Name = rune.Name,
                                IconPath = rune.Icon,
                                TreeName = tree.Name,
                                TreeId = tree.Id,
                                SlotIndex = slotIndex,
                                ShortDescription = rune.ShortDesc
                            };

                            await _runeRepository.CreateAsync(newRune);
                            syncedCount++;
                            _logger.LogDebug("Runa creada: {RuneName} ({TreeName})", newRune.Name, tree.Name);
                        }
                        else
                        {
                            existingRune.Key = rune.Key;
                            existingRune.Name = rune.Name;
                            existingRune.IconPath = rune.Icon;
                            existingRune.TreeName = tree.Name;
                            existingRune.TreeId = tree.Id;
                            existingRune.SlotIndex = slotIndex;
                            existingRune.ShortDescription = rune.ShortDesc;

                            await _runeRepository.UpdateAsync(existingRune);
                            syncedCount++;
                        }
                    }
                    slotIndex++;
                }
            }

            _logger.LogInformation("Sincronización de runas completada. {Count} runas sincronizadas", syncedCount);
            return syncedCount;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al sincronizar runas desde Riot API");
            throw;
        }
    }

    /// <summary>
    /// Sincroniza todos los items desde Data Dragon
    /// </summary>
    public async Task<int> SyncItemsFromRiotAsync(string language = "en_US")
    {
        try
        {
            var version = await GetLatestVersionAsync();
            var url = $"{DataDragonBaseUrl}/cdn/{version}/data/{language}/item.json";

            _logger.LogInformation("Obteniendo items desde {Url}", url);

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var jsonString = await response.Content.ReadAsStringAsync();
            var itemData = JsonSerializer.Deserialize<RiotItemResponse>(jsonString);

            if (itemData?.Data == null || !itemData.Data.Any())
            {
                _logger.LogWarning("No se encontraron datos de items");
                return 0;
            }

            int syncedCount = 0;

            foreach (var (itemId, item) in itemData.Data)
            {
                if (!int.TryParse(itemId, out int riotItemId))
                    continue;

                // Filtrar items que no son para Summoner's Rift (map 11)
                if (item.Maps != null && item.Maps.TryGetValue("11", out bool isAvailable) && !isAvailable)
                    continue;

                var existingItem = await _itemRepository.GetByRiotIdAsync(riotItemId);

                var tags = item.Tags != null ? JsonSerializer.Serialize(item.Tags) : null;
                var buildsFrom = item.From != null ? string.Join(",", item.From) : null;
                var buildsInto = item.Into != null ? string.Join(",", item.Into) : null;

                // Limpiar HTML del nombre (algunos items tienen tags HTML en el nombre)
                var cleanName = StripHtmlTags(item.Name);

                if (existingItem == null)
                {
                    var newItem = new Item
                    {
                        RiotItemId = riotItemId,
                        Name = cleanName,
                        Description = item.Description,
                        IconPath = $"{riotItemId}.png",
                        TotalGold = item.Gold?.Total ?? 0,
                        Tags = tags,
                        IsPurchasable = item.Gold?.Purchasable ?? false,
                        IsCompleted = (item.Depth ?? 1) >= 3,
                        BuildsFrom = buildsFrom,
                        BuildsInto = buildsInto
                    };

                    await _itemRepository.CreateAsync(newItem);
                    syncedCount++;
                    _logger.LogDebug("Item creado: {ItemName}", newItem.Name);
                }
                else
                {
                    existingItem.Name = cleanName;
                    existingItem.Description = item.Description;
                    existingItem.IconPath = $"{riotItemId}.png";
                    existingItem.TotalGold = item.Gold?.Total ?? 0;
                    existingItem.Tags = tags;
                    existingItem.IsPurchasable = item.Gold?.Purchasable ?? false;
                    existingItem.IsCompleted = (item.Depth ?? 1) >= 3;
                    existingItem.BuildsFrom = buildsFrom;
                    existingItem.BuildsInto = buildsInto;

                    await _itemRepository.UpdateAsync(existingItem);
                    syncedCount++;
                }
            }

            _logger.LogInformation("Sincronización de items completada. {Count} items sincronizados", syncedCount);
            return syncedCount;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al sincronizar items desde Riot API");
            throw;
        }
    }

    /// <summary>
    /// Sincroniza todo: campeones, runas e items
    /// </summary>
    public async Task<(int champions, int runes, int items)> SyncAllFromRiotAsync(string language = "en_US")
    {
        var champions = await SyncChampionsFromRiotAsync(language);
        var runes = await SyncRunesFromRiotAsync(language);
        var items = await SyncItemsFromRiotAsync(language);

        return (champions, runes, items);
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

    // ============================================
    // Clases para Runas (runesReforged.json)
    // ============================================

    private class RiotRuneTree
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("key")]
        public string Key { get; set; } = string.Empty;

        [JsonPropertyName("icon")]
        public string Icon { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("slots")]
        public List<RiotRuneSlot> Slots { get; set; } = new();
    }

    private class RiotRuneSlot
    {
        [JsonPropertyName("runes")]
        public List<RiotRune> Runes { get; set; } = new();
    }

    private class RiotRune
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("key")]
        public string Key { get; set; } = string.Empty;

        [JsonPropertyName("icon")]
        public string Icon { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("shortDesc")]
        public string ShortDesc { get; set; } = string.Empty;

        [JsonPropertyName("longDesc")]
        public string LongDesc { get; set; } = string.Empty;
    }

    // ============================================
    // Clases para Items (item.json)
    // ============================================

    private class RiotItemResponse
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        [JsonPropertyName("version")]
        public string Version { get; set; } = string.Empty;

        [JsonPropertyName("data")]
        public Dictionary<string, RiotItem> Data { get; set; } = new();
    }

    private class RiotItem
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        [JsonPropertyName("gold")]
        public RiotItemGold? Gold { get; set; }

        [JsonPropertyName("tags")]
        public List<string>? Tags { get; set; }

        [JsonPropertyName("maps")]
        public Dictionary<string, bool>? Maps { get; set; }

        [JsonPropertyName("from")]
        public List<string>? From { get; set; }

        [JsonPropertyName("into")]
        public List<string>? Into { get; set; }

        [JsonPropertyName("depth")]
        public int? Depth { get; set; }
    }

    private class RiotItemGold
    {
        [JsonPropertyName("base")]
        public int Base { get; set; }

        [JsonPropertyName("total")]
        public int Total { get; set; }

        [JsonPropertyName("sell")]
        public int Sell { get; set; }

        [JsonPropertyName("purchasable")]
        public bool Purchasable { get; set; }
    }

    // ============================================
    // Clases para Detalles de Campeón (champion/{championId}.json)
    // ============================================

    private class RiotChampionDetailedResponse
    {
        [JsonPropertyName("data")]
        public Dictionary<string, RiotChampionDetailed>? Data { get; set; }
    }

    private class RiotChampionDetailed
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("passive")]
        public RiotSpellPassive Passive { get; set; } = new();

        [JsonPropertyName("spells")]
        public List<RiotSpell> Spells { get; set; } = new();
    }

    private class RiotSpellPassive
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        [JsonPropertyName("image")]
        public RiotChampionImage Image { get; set; } = new();
    }

    private class RiotSpell
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        [JsonPropertyName("image")]
        public RiotChampionImage Image { get; set; } = new();
    }

    #endregion
}
