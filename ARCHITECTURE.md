# Arquitectura del Sistema - Matchup Companion

## Diagrama de Arquitectura General

```
┌─────────────────────────────────────────────────────────────┐
│                    Frontend (Cliente)                        │
│                  Blazor WebAssembly SPA                      │
│              (MatchupCompanion.Client)                       │
│                                                              │
│  ┌──────────────────    PAGES    ─────────────────────┐    │
│  │  Home │ MatchupSearch │ MatchupsList               │    │
│  │  MatchupDetail │ CreateMatchup │ AddTip            │    │
│  └───────────────────────┬──────────────────────────────┘   │
│                          │ Consume                          │
│                          ▼                                   │
│  ┌──────────────────  SERVICES  ─────────────────────┐     │
│  │  IChampionService │ IRoleService                  │     │
│  │  IMatchupService                                  │     │
│  │  (HTTP communication via HttpClient)             │     │
│  └───────────────────────┬──────────────────────────────┘  │
│                          │ Uses                             │
│                          ▼                                   │
│  ┌──────────────────  SHARED DTOs  ───────────────────┐    │
│  │  ChampionDto │ MatchupDto │ CreateMatchupDto       │    │
│  │  (MatchupCompanion.Shared)                         │    │
│  └────────────────────────────────────────────────────┘    │
└────────────────────────┬────────────────────────────────────┘
                         │ HTTP/HTTPS
                         │ JSON REST API
                         ▼
┌─────────────────────────────────────────────────────────────┐
│                    ASP.NET Core Web API                      │
│                     (MatchupCompanion.API)                   │
├─────────────────────────────────────────────────────────────┤
│                                                              │
│  ┌──────────────────  CONTROLLERS  ─────────────────────┐  │
│  │  ChampionsController  │  MatchupsController          │  │
│  │  RolesController      │  RiotSyncController          │  │
│  └───────────────────────┬──────────────────────────────┘  │
│                          │ Llamadas                         │
│                          ▼                                   │
│  ┌──────────────────   SERVICES   ─────────────────────┐   │
│  │  ChampionService   │  MatchupService                │   │
│  │  (Lógica de Negocio / Validaciones)                 │   │
│  └───────────────────────┬──────────────────────────────┘  │
│                          │ Usa                              │
│                          ▼                                   │
│  ┌──────────────────  REPOSITORIES  ───────────────────┐   │
│  │  ChampionRepository    │  MatchupRepository         │   │
│  │  MatchupTipRepository  │  RoleRepository            │   │
│  │  (Acceso a Datos con Entity Framework Core)        │   │
│  └───────────────────────┬──────────────────────────────┘  │
│                          │ Lee/Escribe                      │
│                          ▼                                   │
│  ┌────────────────  ApplicationDbContext  ─────────────┐   │
│  │           Entity Framework Core ORM                  │   │
│  └───────────────────────┬──────────────────────────────┘  │
│                          │                                   │
└──────────────────────────┼───────────────────────────────────┘
                           │ SQL
                           ▼
                ┌──────────────────────┐
                │   SQL Server         │
                │   (Base de Datos)    │
                └──────────────────────┘

         ┌────────────────────────────┐
         │   Riot Games Data Dragon   │
         │   (ddragon.leagueoflegends │
         │   .com)                    │
         └──────────┬─────────────────┘
                    │ HTTPS (No API Key)
                    ▼
         ┌──────────────────────────────┐
         │  RiotApiService              │
         │  - GetLatestVersionAsync()   │
         │  - SyncChampionsFromRiot()   │
         └──────────────────────────────┘
```

## Flujo de una Request Típica

### Ejemplo: GET /api/matchups/1

```
1. Cliente hace HTTP GET /api/matchups/1

2. MatchupsController.GetMatchupById(1)
   ├─ Valida el request
   └─ Llama a MatchupService

3. MatchupService.GetMatchupByIdAsync(1)
   ├─ Aplica lógica de negocio
   └─ Llama a MatchupRepository

4. MatchupRepository.GetByIdAsync(1)
   ├─ Usa DbContext para query
   ├─ Include relacionados (Champion, Role, Tips)
   └─ Retorna entidad Matchup

5. MatchupService mapea Matchup → MatchupDto

6. MatchupsController retorna MatchupDto como JSON

7. Cliente recibe response HTTP 200 con JSON
```

## Capas y Responsabilidades

### 1. Controllers (Capa de Presentación)

**Responsabilidad:** Manejo de HTTP requests/responses

**Tareas:**
- Validar ModelState
- Llamar a servicios
- Retornar códigos HTTP apropiados
- Transformar excepciones en responses HTTP

**NO debe:**
- Contener lógica de negocio
- Acceder directamente a repositorios
- Manipular DbContext

**Ejemplo:**
```csharp
[HttpGet("{id}")]
public async Task<IActionResult> GetMatchupById(int id)
{
    var matchup = await _matchupService.GetMatchupByIdAsync(id);

    if (matchup == null)
        return NotFound();

    return Ok(matchup);
}
```

### 2. Services (Capa de Lógica de Negocio)

**Responsabilidad:** Implementar reglas de negocio

**Tareas:**
- Validaciones complejas
- Coordinar múltiples repositorios
- Transformar entidades a DTOs
- Logging de operaciones importantes

**NO debe:**
- Saber de HTTP (códigos, headers, etc.)
- Contener queries SQL directos
- Manejar conexiones de BD

**Ejemplo:**
```csharp
public async Task<MatchupDto> CreateMatchupAsync(CreateMatchupRequest request)
{
    // Validar que los campeones existan
    var player = await _championRepo.GetByIdAsync(request.PlayerChampionId);
    if (player == null)
        throw new ArgumentException("Champion not found");

    // Validar que no exista el matchup
    var existing = await _matchupRepo.GetByChampionsAndRoleAsync(...);
    if (existing != null)
        throw new InvalidOperationException("Matchup already exists");

    // Crear el matchup
    var matchup = new Matchup { ... };
    await _matchupRepo.CreateAsync(matchup);

    return MapToDto(matchup);
}
```

### 3. Repositories (Capa de Acceso a Datos)

**Responsabilidad:** Abstracción sobre Entity Framework

**Tareas:**
- Queries a base de datos
- Operaciones CRUD
- Includes y optimizaciones
- Mapeo básico de entidades

**NO debe:**
- Contener lógica de negocio
- Validar reglas de negocio
- Saber de DTOs

**Ejemplo:**
```csharp
public async Task<Matchup?> GetByIdAsync(int id)
{
    return await _context.Matchups
        .Include(m => m.PlayerChampion)
        .Include(m => m.EnemyChampion)
        .Include(m => m.Role)
        .Include(m => m.Tips)
        .FirstOrDefaultAsync(m => m.Id == id);
}
```

### 4. Data / DbContext

**Responsabilidad:** Configuración de Entity Framework

**Tareas:**
- Definir DbSets
- Configurar relaciones
- Configurar índices
- Seed data

**Ejemplo:**
```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Matchup>(entity =>
    {
        entity.HasOne(m => m.PlayerChampion)
            .WithMany(c => c.MatchupsAsPlayerChampion)
            .HasForeignKey(m => m.PlayerChampionId)
            .OnDelete(DeleteBehavior.Restrict);
    });
}
```

## Patrones de Diseño Implementados

### 1. Repository Pattern

**Ventajas:**
- Abstrae el acceso a datos
- Facilita testing (mock repositories)
- Centraliza queries complejas
- Permite cambiar ORM sin afectar servicios

**Implementación:**
```csharp
// Interfaz
public interface IChampionRepository
{
    Task<Champion?> GetByIdAsync(int id);
}

// Implementación
public class ChampionRepository : IChampionRepository
{
    private readonly ApplicationDbContext _context;

    public async Task<Champion?> GetByIdAsync(int id)
    {
        return await _context.Champions.FindAsync(id);
    }
}
```

### 2. Dependency Injection

**Ventajas:**
- Bajo acoplamiento
- Facilita testing
- Configuración centralizada
- Lifecycle management automático

**Implementación:**
```csharp
// Program.cs
builder.Services.AddScoped<IChampionRepository, ChampionRepository>();
builder.Services.AddScoped<IChampionService, ChampionService>();

// En controllers/services
public class ChampionService
{
    private readonly IChampionRepository _repo;

    public ChampionService(IChampionRepository repo)
    {
        _repo = repo;
    }
}
```

### 3. DTO Pattern

**Ventajas:**
- Desacopla API de modelo de datos
- Control sobre qué se expone
- Versionado de API más fácil
- Reduce over-posting

**Implementación:**
```csharp
// Entidad (interna)
public class Champion
{
    public int Id { get; set; }
    public string RiotChampionId { get; set; }
    // ... navegación, metadata, etc.
}

// DTO (público)
public class ChampionDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    // Solo lo necesario
}
```

### 4. Service Layer Pattern

**Ventajas:**
- Centraliza lógica de negocio
- Reutilizable entre controllers
- Más fácil de testear
- Clara separación de responsabilidades

## Modelo de Datos

### Diagrama ER

```
┌──────────────┐
│   Role       │
│──────────────│
│ Id (PK)      │
│ Name         │
│ Description  │
└──────┬───────┘
       │ 1
       │
       │ N
┌──────▼───────────────┐           ┌─────────────────────────────┐
│    Champion          │           │   Matchup                   │
│──────────────────────│           │─────────────────────────────│
│ Id (PK)              │◄─────────┤│ Id (PK)                     │
│ RiotChampionId       │ N   Player│ PlayerChampionId            │
│ Name                 │           │ EnemyChampionId             │
│ Title                │◄─────────┤│ RoleId                      │
│ ImageUrl             │ N   Enemy │ Difficulty                  │
│ PrimaryRoleId (FK)   │           │ GeneralAdvice               │
│ CreatedAt            │           │ StrategyNotes               │
│ UpdatedAt            │           │ StartingItems (JSON)        │
└──────────────────────┘           │ CoreItems (JSON)            │
                                   │ SituationalItems (JSON)     │
                                   │ RecommendedRunes (JSON)     │
                                   └────────┬────────────────────┘
                                            │ 1
                                            │ N
                                   ┌────────▼────────┐
                                   │  MatchupTip     │
                                   │─────────────────│
                                   │ Id (PK)         │
                                   │ MatchupId (FK)  │
                                   │ Category        │
                                   │ Content         │
                                   │ Priority        │
                                   │ AuthorName      │
                                   │ CreatedAt       │
                                   └─────────────────┘

┌─────────────────────┐           ┌─────────────────────┐
│   Rune              │           │   Item              │
│─────────────────────│           │─────────────────────│
│ Id (PK)             │           │ Id (PK)             │
│ RiotRuneId          │           │ RiotItemId          │
│ Name                │           │ Name                │
│ Description         │           │ Description         │
│ ImageUrl            │           │ ImageUrl            │
│ Slot                │           │ Gold (cost)         │
│ Tree                │           │ CreatedAt           │
│ CreatedAt           │           │ UpdatedAt           │
│ UpdatedAt           │           └─────────────────────┘
└─────────────────────┘
```

### Relaciones Clave

1. **Champion → Role**: Muchos a Uno (opcional)
2. **Matchup → Champion (Player)**: Muchos a Uno
3. **Matchup → Champion (Enemy)**: Muchos a Uno
4. **Matchup → Role**: Muchos a Uno
5. **Matchup → MatchupTip**: Uno a Muchos
6. **Matchup → Items**: Referencia por RiotItemId en campos JSON (StartingItems, CoreItems, SituationalItems)
7. **Matchup → Runes**: Referencia por RiotRuneId en campo JSON (RecommendedRunes)

### Índices Importantes

```sql
-- Búsqueda rápida de matchups
CREATE UNIQUE INDEX IX_Matchup_PlayerChampion_EnemyChampion_Role
ON Matchups (PlayerChampionId, EnemyChampionId, RoleId);

-- Búsqueda por nombre de campeón
CREATE INDEX IX_Champion_Name ON Champions (Name);

-- Búsqueda por RiotChampionId
CREATE UNIQUE INDEX IX_Champion_RiotChampionId
ON Champions (RiotChampionId);

-- Búsqueda por RiotItemId
CREATE UNIQUE INDEX IX_Item_RiotItemId ON Items (RiotItemId);

-- Búsqueda por RiotRuneId
CREATE UNIQUE INDEX IX_Rune_RiotRuneId ON Runes (RiotRuneId);
```

## Seguridad y Buenas Prácticas

### 1. SQL Injection

**Protección:** Entity Framework usa queries parametrizadas
- Nunca construir SQL strings manualmente
- Usar siempre LINQ o FromSqlRaw con parámetros

### 2. Over-posting

**Protección:** DTOs separados para input/output
- No aceptar entidades directamente en POST/PUT
- Usar `CreateMatchupRequest` en lugar de `Matchup`

### 3. CORS

**Configuración:** Política configurada en `Program.cs`
```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
```

**Producción:** Restringir a dominios específicos

### 4. Logging

**Implementado:** ILogger inyectado en servicios
- Información en operaciones importantes
- Warnings en validaciones fallidas
- Errors en excepciones

### 5. Async/Await

**Beneficios:**
- No bloquea threads
- Mejor escalabilidad
- Uso eficiente de recursos

**Regla:** Toda operación I/O debe ser async

## Integración con Riot Games Data Dragon

### ¿Qué es Data Dragon?

Data Dragon es un servicio CDN de Riot Games que provee:
- Información estática de campeones (nombres, títulos, stats)
- Imágenes de campeones, ítems, habilidades
- Datos de ítems, runas, y más
- **No requiere API key** (a diferencia de la Riot API oficial)
- Actualizado con cada patch de League of Legends

### RiotApiService - Arquitectura

```csharp
public class RiotApiService
{
    private readonly HttpClient _httpClient;
    private readonly IChampionRepository _championRepository;
    private readonly IRuneRepository _runeRepository;
    private readonly IItemRepository _itemRepository;
    private readonly ILogger<RiotApiService> _logger;

    // URL base de Data Dragon
    private const string DataDragonBaseUrl = "https://ddragon.leagueoflegends.com";

    public async Task<string> GetLatestVersionAsync()
    {
        // Obtiene la versión más reciente (ej: "14.24.1")
        // URL: https://ddragon.leagueoflegends.com/api/versions.json
    }

    public async Task<int> SyncChampionsFromRiotAsync(string language = "es_ES")
    {
        // Sincroniza campeones desde:
        // https://ddragon.leagueoflegends.com/cdn/{version}/data/{language}/champion.json
    }

    public async Task<int> SyncRunesFromRiotAsync(string language = "es_ES")
    {
        // Sincroniza runas desde:
        // https://ddragon.leagueoflegends.com/cdn/{version}/data/{language}/runesReforged.json
    }

    public async Task<int> SyncItemsFromRiotAsync(string language = "es_ES")
    {
        // Sincroniza items desde:
        // https://ddragon.leagueoflegends.com/cdn/{version}/data/{language}/item.json
        // Incluye limpieza de tags HTML en nombres
    }
}
```

**Sincronización automática al iniciar:**
El API sincroniza automáticamente campeones, runas e items al iniciar si la BD está vacía (Program.cs).

### Deserialización JSON - Clases Internas

**Problema resuelto**: Data Dragon retorna propiedades en minúsculas (`name`, `title`, `blurb`), pero C# usa PascalCase (`Name`, `Title`, `Blurb`).

**Solución**: Atributos `[JsonPropertyName]`

```csharp
private class RiotChampion
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("key")]
    public string Key { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("blurb")]
    public string Blurb { get; set; }

    [JsonPropertyName("image")]
    public RiotChampionImage Image { get; set; }

    [JsonPropertyName("tags")]
    public List<string> Tags { get; set; }
}
```

### Mapeo de Roles

Data Dragon usa "tags" genéricos (`Fighter`, `Mage`, `Support`), pero nuestra BD usa roles específicos de LoL (`Top`, `Jungle`, `Mid`, `ADC`, `Support`).

**Mapeo implementado**:
```csharp
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
        _ => null            // Sin rol definido
    };
}
```

**Nota**: Este mapeo es simplificado. En producción se podría:
- Permitir múltiples roles por campeón
- Usar machine learning para mapeo más preciso
- Consultar API oficial de Riot para stats de posición

### Flujo de Sincronización

```
1. Usuario ejecuta POST /api/RiotSync/sync-champions

2. RiotSyncController.SyncChampions()
   └─ Llama a RiotApiService.SyncChampionsFromRiotAsync()

3. RiotApiService
   ├─ GetLatestVersionAsync()
   │  └─ GET https://ddragon.leagueoflegends.com/api/versions.json
   │     → ["14.24.1", "14.23.1", ...]
   │
   ├─ Construye URL de campeones
   │  → https://ddragon.leagueoflegends.com/cdn/14.24.1/data/en_US/champion.json
   │
   ├─ Deserializa JSON → RiotChampionResponse
   │  └─ Dictionary<string, RiotChampion> con ~172 campeones
   │
   └─ Para cada campeón:
      ├─ ChampionRepository.GetByRiotIdAsync(champion.Key)
      ├─ Si NO existe: CreateAsync()
      │  └─ Campeón creado con imagen URL:
      │     https://ddragon.leagueoflegends.com/cdn/14.24.1/img/champion/Aatrox.png
      │
      └─ Si existe: UpdateAsync()
         └─ Actualiza nombre, título, descripción, imagen

4. Retorna cantidad de campeones sincronizados (172)
```

### URLs de Imágenes Generadas

Las imágenes de campeones se generan dinámicamente:

**Formato**:
```
https://ddragon.leagueoflegends.com/cdn/{version}/img/champion/{championId}.png
```

**Ejemplo real**:
```
https://ddragon.leagueoflegends.com/cdn/14.24.1/img/champion/Aatrox.png
https://ddragon.leagueoflegends.com/cdn/14.24.1/img/champion/Ahri.png
```

**Ventaja**: Las URLs son estables y se sirven desde CDN de Riot (alta disponibilidad).

### Manejo de Errores

```csharp
try
{
    var version = await GetLatestVersionAsync();
    // ...
}
catch (HttpRequestException ex)
{
    _logger.LogError(ex, "Error de red al contactar Data Dragon");
    throw;
}
catch (JsonException ex)
{
    _logger.LogError(ex, "Error al deserializar respuesta de Riot");
    throw;
}
catch (Exception ex)
{
    _logger.LogError(ex, "Error general en sincronización");
    throw;
}
```

### Logging de Sincronización

Durante la sincronización se generan logs:

```
info: RiotApiService[0]
      Obteniendo campeones desde https://ddragon.leagueoflegends.com/cdn/14.24.1/data/en_US/champion.json

info: RiotApiService[0]
      Campeón creado: Aatrox

info: RiotApiService[0]
      Campeón actualizado: Ahri

info: RiotApiService[0]
      Sincronización completada. 172 campeones sincronizados
```

## Arquitectura del Frontend (Blazor WebAssembly)

### Estructura del Proyecto Client

```
MatchupCompanion.Client/
├── Pages/                      # Razor pages (componentes con ruta)
│   ├── Home.razor             # Página principal
│   ├── MatchupSearch.razor    # Búsqueda de matchup
│   ├── MatchupsList.razor     # Lista de matchups
│   ├── MatchupDetail.razor    # Detalles de matchup
│   ├── CreateMatchup.razor    # Crear matchup
│   └── AddTip.razor           # Agregar tip
├── Layout/                    # Componentes de layout
│   ├── MainLayout.razor       # Layout principal
│   └── NavMenu.razor          # Menú de navegación
├── Services/                  # Servicios HTTP
│   ├── IChampionService.cs    # Interfaz de servicio
│   ├── ChampionService.cs     # Implementación
│   ├── IRoleService.cs
│   ├── RoleService.cs
│   ├── IMatchupService.cs
│   └── MatchupService.cs
├── wwwroot/                   # Archivos estáticos
│   ├── css/                   # Estilos
│   └── index.html             # HTML base
├── _Imports.razor             # Imports globales
├── App.razor                  # Componente raíz
└── Program.cs                 # Configuración de la app
```

### Patrón de Arquitectura Frontend

El frontend sigue un patrón de **Component-Service Architecture**:

```
┌──────────────────────────────────────────────────────────┐
│                   Razor Pages                            │
│  (Presentación, interacción del usuario, validación)    │
└───────────────────────┬──────────────────────────────────┘
                        │ @inject
                        │ Dependency Injection
                        ▼
┌──────────────────────────────────────────────────────────┐
│              HTTP Services (IChampionService, etc.)       │
│  (Lógica de comunicación con API, serialización)        │
└───────────────────────┬──────────────────────────────────┘
                        │ HttpClient
                        │ JSON over HTTP
                        ▼
┌──────────────────────────────────────────────────────────┐
│                 Backend API (REST)                       │
│            https://localhost:7285/api/*                  │
└──────────────────────────────────────────────────────────┘
```

### Razor Pages - Responsabilidades

**Home.razor** - Página de bienvenida
- Descripción del proyecto
- Enlaces a funcionalidades principales
- Información de uso

**MatchupSearch.razor** - Búsqueda de matchup específico
```csharp
@page "/matchup-search"
@inject IChampionService ChampionService
@inject IMatchupService MatchupService
@inject NavigationManager Navigation

// Flujo:
// 1. Cargar todos los campeones
// 2. Usuario selecciona campeón jugador
// 3. Usuario selecciona campeón enemigo
// 4. Buscar matchup específico con API
// 5. Si existe: redirigir a detalles
// 6. Si no existe: redirigir a crear matchup
```

**MatchupsList.razor** - Lista completa de matchups
```csharp
@page "/matchups"
@inject IMatchupService MatchupService

// Características:
// - Filtrado en tiempo real por nombre de campeón
// - Visualización de dificultad (Easy, Medium, Hard)
// - Enlaces a detalles de cada matchup
// - Manejo de estados: loading, error, empty, success
```

**MatchupDetail.razor** - Detalles de matchup
```csharp
@page "/matchup-detail/{id:int}"
@inject IMatchupService MatchupService
@inject NavigationManager Navigation

[Parameter]
public int Id { get; set; }

// Muestra:
// - Información del matchup (campeones, rol, dificultad)
// - Consejos generales
// - Lista de tips organizados por categoría
// - Botón para agregar nuevo tip
```

**CreateMatchup.razor** - Formulario de creación
```csharp
@page "/create-matchup"
@inject IChampionService ChampionService
@inject IRoleService RoleService
@inject IMatchupService MatchupService
@inject NavigationManager Navigation

// Validación:
// - Campeón jugador requerido
// - Campeón enemigo requerido
// - Campeones deben ser diferentes
// - Rol requerido
// - Dificultad requerida
```

**AddTip.razor** - Agregar tip a matchup
```csharp
@page "/add-tip/{matchupId:int}"
@inject IMatchupService MatchupService
@inject NavigationManager Navigation

[Parameter]
public int MatchupId { get; set; }

// Categorías disponibles:
// - Early Game, Mid Game, Late Game
// - Items, Runes, General
// Prioridad: 1 (más importante) a 5 (menos importante)
```

### HTTP Services - Implementación

Los servicios encapsulan toda la comunicación HTTP con la API:

```csharp
public interface IChampionService
{
    Task<List<ChampionDto>> GetAllChampionsAsync();
    Task<ChampionDto?> GetChampionByIdAsync(int id);
    Task<List<ChampionDto>> GetChampionsByRoleAsync(int roleId);
}

public class ChampionService : IChampionService
{
    private readonly HttpClient _httpClient;

    public ChampionService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<ChampionDto>> GetAllChampionsAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<ChampionDto>>("/api/Champions")
            ?? new List<ChampionDto>();
    }

    // Manejo de errores:
    // - Retorna null/lista vacía en caso de error
    // - El componente maneja el estado de error
}
```

**Beneficios de este patrón**:
- Separación de responsabilidades
- Fácil testing (mock de servicios)
- Reutilización entre componentes
- Centralización de lógica HTTP

### Configuración de Dependency Injection

**Program.cs** - Configuración del frontend:

```csharp
var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// HttpClient configurado con base URL
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("https://localhost:7285")
});

// Registro de servicios
builder.Services.AddScoped<IChampionService, ChampionService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IMatchupService, MatchupService>();

await builder.Build().RunAsync();
```

**Importante**:
- HttpClient se registra con `AddScoped` (una instancia por "sesión")
- Base URL apunta al backend API
- Servicios se inyectan con `@inject` en Razor pages

### Shared DTOs - Contratos de Datos

El proyecto **MatchupCompanion.Shared** contiene los DTOs compartidos entre frontend y backend:

```csharp
// ChampionDto.cs
public class ChampionDto
{
    public int Id { get; set; }
    public string RiotChampionId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
}

// CreateMatchupDto.cs
public class CreateMatchupDto
{
    [Required]
    public int PlayerChampionId { get; set; }

    [Required]
    public int EnemyChampionId { get; set; }

    [Required]
    public int RoleId { get; set; }

    [Required]
    [MaxLength(50)]
    public string Difficulty { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? GeneralAdvice { get; set; }
}
```

**Ventajas de Shared DTOs**:
- Type safety entre frontend y backend
- Evita duplicación de código
- Validaciones compartidas (DataAnnotations)
- Refactoring más seguro

### Estado y Lifecycle en Blazor

**Componente típico con estado**:

```csharp
@code {
    private List<MatchupDto> matchups = new();
    private List<MatchupDto> filteredMatchups = new();
    private bool isLoading = true;
    private string? errorMessage;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            matchups = await MatchupService.GetAllMatchupsAsync();
            filteredMatchups = matchups;
        }
        catch (Exception ex)
        {
            errorMessage = "Error al cargar matchups";
        }
        finally
        {
            isLoading = false;
        }
    }

    private void FilterMatchups()
    {
        filteredMatchups = matchups
            .Where(m => m.PlayerChampionName.Contains(filterText,
                StringComparison.OrdinalIgnoreCase))
            .ToList();
    }
}
```

**Lifecycle hooks usados**:
- `OnInitializedAsync()` - Cargar datos al inicializar componente
- `OnParametersSet()` - Reaccionar a cambios de parámetros (usado en MatchupDetail)
- `StateHasChanged()` - Forzar re-render (raramente necesario)

### Navegación entre Páginas

**NavigationManager** - Inyectado para navegación programática:

```csharp
@inject NavigationManager Navigation

private async Task SearchMatchup()
{
    var matchup = await MatchupService.GetSpecificMatchupAsync(
        selectedPlayerChampion, selectedEnemyChampion);

    if (matchup != null)
    {
        // Matchup existe, ir a detalles
        Navigation.NavigateTo($"/matchup-detail/{matchup.Id}");
    }
    else
    {
        // Matchup no existe, ir a crear
        Navigation.NavigateTo("/create-matchup");
    }
}
```

**NavLink** - Componente para links con estado activo:

```csharp
<NavLink class="nav-link" href="matchup-search">
    Buscar Matchup
</NavLink>

// Automáticamente agrega clase "active" cuando la ruta coincide
```

### Validación de Formularios

Blazor usa **EditForm** con validaciones de DataAnnotations:

```razor
<EditForm Model="@createMatchupDto" OnValidSubmit="@HandleValidSubmit">
    <DataAnnotationsValidator />
    <ValidationSummary />

    <div class="mb-3">
        <label>Campeón Jugador</label>
        <InputSelect @bind-Value="createMatchupDto.PlayerChampionId" class="form-select">
            <option value="0">Seleccionar...</option>
            @foreach (var champion in champions)
            {
                <option value="@champion.Id">@champion.Name</option>
            }
        </InputSelect>
        <ValidationMessage For="@(() => createMatchupDto.PlayerChampionId)" />
    </div>

    <button type="submit" class="btn btn-primary">Crear Matchup</button>
</EditForm>
```

**Componentes de validación**:
- `DataAnnotationsValidator` - Habilita validaciones de atributos [Required], [MaxLength], etc.
- `ValidationSummary` - Muestra resumen de errores
- `ValidationMessage` - Mensaje de error para campo específico
- `OnValidSubmit` - Solo se ejecuta si el formulario es válido

### Manejo de Errores en el Frontend

**Estrategia actual**:

```csharp
try
{
    await MatchupService.CreateMatchupAsync(createMatchupDto);
    Navigation.NavigateTo("/matchups");
}
catch (Exception ex)
{
    errorMessage = "Error al crear matchup. Por favor intenta de nuevo.";
    // Logging futuro: enviar a servicio de telemetría
}
```

**Próximas mejoras**:
- Componente de notificaciones (toasts)
- Manejo específico de errores HTTP (400, 401, 404, 500)
- Error boundary para errores no controlados
- Retry logic para requests fallidos

### Bootstrap Integration

El frontend usa **Bootstrap 5** para estilos:

```html
<!-- index.html -->
<link href="https://cdn.jsdelivr.net/npm/bootstrap@5.1.3/dist/css/bootstrap.min.css"
      rel="stylesheet">
```

**Componentes Bootstrap usados**:
- Forms (form-control, form-select)
- Buttons (btn, btn-primary, btn-danger)
- Cards (card, card-body)
- Layout (container, row, col)
- Alerts (alert, alert-danger)
- Spinners (spinner-border) para loading

### Performance Considerations

**Optimizaciones implementadas**:

1. **Virtualization** (próximo paso)
   - Para listas largas de campeones
   - Usar `<Virtualize>` component

2. **Lazy Loading**
   - Blazor ya hace code splitting automático
   - Solo carga componentes cuando se navega a ellos

3. **Caching en Services**
   ```csharp
   private List<ChampionDto>? _cachedChampions;

   public async Task<List<ChampionDto>> GetAllChampionsAsync()
   {
       if (_cachedChampions != null)
           return _cachedChampions;

       _cachedChampions = await _httpClient
           .GetFromJsonAsync<List<ChampionDto>>("/api/Champions")
           ?? new List<ChampionDto>();

       return _cachedChampions;
   }
   ```

4. **Debouncing en filtros**
   - Evitar llamadas excesivas al filtrar
   - Usar `System.Timers.Timer` o librería externa

### Seguridad en el Frontend

**Consideraciones actuales**:

1. **No hay autenticación** (próximo paso)
   - Implementar ASP.NET Core Identity
   - JWT tokens en headers
   - Redirección a login si no autenticado

2. **XSS Protection**
   - Blazor escapa HTML automáticamente
   - Solo usar `@((MarkupString)html)` con datos confiables

3. **CORS**
   - Backend configurado para aceptar requests del frontend
   - Producción: restringir a dominio específico

4. **Validación client-side y server-side**
   - DataAnnotations en DTOs compartidos
   - Backend valida nuevamente (no confiar en frontend)

## Testing (Próximos Pasos)

### Unit Tests

```csharp
public class MatchupServiceTests
{
    [Fact]
    public async Task CreateMatchup_ValidData_ReturnsMatchupDto()
    {
        // Arrange
        var mockRepo = new Mock<IMatchupRepository>();
        var service = new MatchupService(mockRepo.Object, ...);

        // Act
        var result = await service.CreateMatchupAsync(request);

        // Assert
        Assert.NotNull(result);
    }
}
```

### Integration Tests

```csharp
public class MatchupsControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task GetMatchup_ExistingId_ReturnsOk()
    {
        // Test completo con base de datos en memoria
    }
}
```

## Performance Considerations

### 1. N+1 Query Problem

**Evitado con:** Eager loading usando `.Include()`

```csharp
// Mal - N+1 queries
var matchups = await _context.Matchups.ToListAsync();
foreach (var m in matchups)
{
    var player = m.PlayerChampion; // Query por cada matchup
}

// Bien - 1 query
var matchups = await _context.Matchups
    .Include(m => m.PlayerChampion)
    .Include(m => m.EnemyChampion)
    .ToListAsync();
```

### 2. Paginación

**Próximo paso:** Implementar en endpoints de listado

```csharp
public async Task<PagedResult<MatchupDto>> GetAllMatchupsAsync(int page, int pageSize)
{
    var total = await _matchupRepo.CountAsync();
    var matchups = await _matchupRepo
        .GetAllAsync()
        .Skip((page - 1) * pageSize)
        .Take(pageSize);

    return new PagedResult<MatchupDto>(matchups, total, page, pageSize);
}
```

### 3. Caching

**Próximo paso:** Redis/Memory cache para datos frecuentes

```csharp
public async Task<ChampionDto?> GetChampionByIdAsync(int id)
{
    var cacheKey = $"champion:{id}";
    var cached = await _cache.GetAsync<ChampionDto>(cacheKey);

    if (cached != null)
        return cached;

    var champion = await _championRepo.GetByIdAsync(id);
    await _cache.SetAsync(cacheKey, champion, TimeSpan.FromHours(1));

    return champion;
}
```

## Escalabilidad

### Vertical Scaling
- Más CPU/RAM al servidor
- SQL Server con más recursos

### Horizontal Scaling
- Múltiples instancias de API detrás de load balancer
- Session state debe ser stateless (ya lo es con JWT)
- Base de datos: replicación read-only

### Microservicios (Futuro)
- Separar ChampionService en su propio servicio
- API Gateway (Ocelot, YARP)
- Event-driven con message bus (RabbitMQ, Azure Service Bus)

## Configuración y Seguridad de API Keys

### Gestión de Secrets

**Archivos de configuración**:
- `appsettings.json` - Configuración base (versionado en Git)
- `appsettings.Development.json` - Configuración local con secrets (NO versionado)
- `appsettings.Development.json.example` - Plantilla sin secrets (versionado)

**Estructura de appsettings.Development.json**:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=MatchupCompanionDb;..."
  },
  "RiotApi": {
    "ApiKey": "RGAPI-XXXXX-XXXXX-XXXXX",  // <- Secret, no commitear
    "DataDragonBaseUrl": "https://ddragon.leagueoflegends.com"
  }
}
```

**Protección en .gitignore**:
```gitignore
# Archivos con API keys NO se suben a Git
**/appsettings.*.json
!**/appsettings.json
# appsettings.Development.json será ignorado
```

**⚠️ IMPORTANTE**:
- NUNCA commitear archivos con API keys reales
- Usar User Secrets en desarrollo: `dotnet user-secrets set "RiotApi:ApiKey" "RGAPI-..."`
- En producción: Azure Key Vault, AWS Secrets Manager, o variables de entorno

### Obtener API Key de Riot

1. Registrarse en: https://developer.riotgames.com/
2. Crear una aplicación
3. Copiar la API key generada
4. Agregar a `appsettings.Development.json` (local)

**Nota**: Data Dragon NO requiere API key para sincronización de campeones.

---

**Documentación generada:** 19 Enero 2026
**Versión:** 3.0
**Última actualización**: Entidades Rune e Item añadidas, sincronización en español, campos de estrategia en Matchup
