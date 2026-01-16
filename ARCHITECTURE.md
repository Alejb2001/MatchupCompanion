# Arquitectura del Sistema - Matchup Companion

## Diagrama de Arquitectura

```
┌─────────────────────────────────────────────────────────────┐
│                        Cliente (Frontend)                    │
│              (Blazor WebAssembly / React / etc.)            │
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
┌──────▼───────────────┐           ┌─────────────────┐
│    Champion          │           │   Matchup       │
│──────────────────────│           │─────────────────│
│ Id (PK)              │◄─────────┤│ Id (PK)         │
│ RiotChampionId       │ N   Player│ PlayerChampId   │
│ Name                 │           │ EnemyChampId    │
│ Title                │◄─────────┤│ RoleId          │
│ ImageUrl             │ N   Enemy │ Difficulty      │
│ PrimaryRoleId (FK)   │           │ GeneralAdvice   │
│ CreatedAt            │           └────────┬────────┘
│ UpdatedAt            │                    │ 1
└──────────────────────┘                    │
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
```

### Relaciones Clave

1. **Champion → Role**: Muchos a Uno (opcional)
2. **Matchup → Champion (Player)**: Muchos a Uno
3. **Matchup → Champion (Enemy)**: Muchos a Uno
4. **Matchup → Role**: Muchos a Uno
5. **Matchup → MatchupTip**: Uno a Muchos

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
    private readonly ILogger<RiotApiService> _logger;

    // URL base de Data Dragon
    private const string DataDragonBaseUrl = "https://ddragon.leagueoflegends.com";

    public async Task<string> GetLatestVersionAsync()
    {
        // Obtiene la versión más reciente (ej: "14.24.1")
        // URL: https://ddragon.leagueoflegends.com/api/versions.json
    }

    public async Task<int> SyncChampionsFromRiotAsync(string language = "en_US")
    {
        // Sincroniza campeones desde:
        // https://ddragon.leagueoflegends.com/cdn/{version}/data/{language}/champion.json

        // 1. Obtiene versión actual
        // 2. Descarga JSON de campeones
        // 3. Deserializa con [JsonPropertyName]
        // 4. Crea/actualiza cada campeón en BD
        // 5. Retorna cantidad sincronizada
    }
}
```

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

**Documentación generada:** 16 Enero 2026
**Versión:** 1.1
**Última actualización**: Integración con Data Dragon completada
