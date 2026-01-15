# Matchup Companion - Guía de Inicio Rápido

## Introducción

Este proyecto es un backend desarrollado con ASP.NET Core 8 que proporciona una API REST para gestionar información de matchups de League of Legends.

## Arquitectura del Proyecto

El proyecto implementa una **arquitectura limpia** con separación clara de responsabilidades:

### Capas de la Aplicación

1. **Controllers (Capa de Presentación)**
   - `ChampionsController`: Endpoints para gestionar campeones
   - `MatchupsController`: Endpoints para matchups y consejos
   - `RolesController`: Endpoints para roles/líneas
   - `RiotSyncController`: Sincronización con Riot API

2. **Services (Capa de Lógica de Negocio)**
   - `ChampionService`: Lógica de negocio para campeones
   - `MatchupService`: Lógica de negocio para matchups
   - Validaciones y reglas de negocio

3. **Repositories (Capa de Acceso a Datos)**
   - Implementación del patrón Repository
   - Interfaces para facilitar testing
   - Operaciones CRUD con Entity Framework

4. **Data (Persistencia)**
   - `ApplicationDbContext`: Contexto de Entity Framework
   - Configuración de relaciones y constraints
   - Seed data inicial

5. **External Services**
   - `RiotApiService`: Integración con Data Dragon API de Riot

## Inicio Rápido

### 1. Requisitos

- .NET 8 SDK
- SQL Server (LocalDB es suficiente)
- Visual Studio 2022, VS Code o Rider (opcional)

### 2. Instalación

```bash
# Clonar el repositorio
git clone <tu-repositorio>
cd MatchupCompanion

# Restaurar paquetes
dotnet restore

# Aplicar migraciones (crear base de datos)
cd MatchupCompanion.API
dotnet ef database update

# Ejecutar la aplicación
dotnet run
```

### 3. Acceder a la API

Una vez iniciada la aplicación:

- **Swagger UI**: https://localhost:7xxx/ (interfaz web interactiva)
- **API Base URL**: https://localhost:7xxx/api

### 4. Sincronizar Campeones

Antes de usar la aplicación, sincroniza los campeones desde Riot:

**Usando Swagger:**
1. Abre https://localhost:7xxx/
2. Busca el endpoint `POST /api/riotsync/sync-champions`
3. Haz clic en "Try it out" → "Execute"

**Usando curl:**
```bash
curl -X POST "https://localhost:7xxx/api/riotsync/sync-champions?language=es_MX"
```

## Flujo de Trabajo Típico

### 1. Obtener Roles Disponibles
```bash
GET /api/roles
```

### 2. Obtener Campeones
```bash
GET /api/champions
GET /api/champions/role/3  # Campeones de Mid
```

### 3. Crear un Matchup
```bash
POST /api/matchups
Body:
{
  "playerChampionId": 2,
  "enemyChampionId": 3,
  "roleId": 3,
  "difficulty": "Hard",
  "generalAdvice": "Cuidado con el burst damage de Zed"
}
```

### 4. Agregar Consejos al Matchup
```bash
POST /api/matchups/tips
Body:
{
  "matchupId": 1,
  "category": "EarlyGame",
  "content": "Pushea la wave para llegar nivel 2 primero",
  "priority": 8
}
```

### 5. Consultar Matchup
```bash
GET /api/matchups/search?playerChampionId=2&enemyChampionId=3&roleId=3
```

## Estructura de la Base de Datos

### Tablas Principales

**Champions**
- Almacena información de campeones
- Se sincroniza desde Riot API
- Incluye nombre, título, imagen, rol principal

**Roles**
- Top, Jungle, Mid, ADC, Support
- Pre-poblado con seed data

**Matchups**
- Relaciona 2 campeones en un rol específico
- Incluye dificultad y consejo general
- Un matchup es único por combinación (jugador, enemigo, rol)

**MatchupTips**
- Consejos específicos para un matchup
- Categorizados (Early, Mid, Late, Items, Runes, Abilities)
- Priorizables (1-10)

### Relaciones

```
Champion 1 ---< Matchup >--- N Champion 2
    |              |
    v              v
  Role           Tips
```

## Patrones y Buenas Prácticas Implementadas

### 1. Repository Pattern
- Abstrae el acceso a datos
- Facilita el testing con mocks
- Cambiar ORM es más sencillo

```csharp
public interface IChampionRepository
{
    Task<Champion?> GetByIdAsync(int id);
    Task<IEnumerable<Champion>> GetAllAsync();
    // ...
}
```

### 2. Dependency Injection
- Todos los servicios se inyectan
- Configurado en `Program.cs`
- Lifecycle: Scoped para servicios con DbContext

```csharp
builder.Services.AddScoped<IChampionService, ChampionService>();
```

### 3. DTOs (Data Transfer Objects)
- Separación entre entidades y datos expuestos
- Evita sobre-exposición de datos
- Control sobre la serialización

```csharp
public class ChampionDto { /* solo propiedades necesarias */ }
```

### 4. Async/Await
- Todas las operaciones I/O son asíncronas
- Mejora el rendimiento y escalabilidad

```csharp
public async Task<Champion?> GetByIdAsync(int id)
{
    return await _context.Champions.FindAsync(id);
}
```

### 5. Validación
- Data Annotations en DTOs
- Validación de ModelState en Controllers
- Validaciones de negocio en Services

## Configuración Avanzada

### Cambiar a PostgreSQL

1. Instalar paquete:
```bash
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
```

2. Actualizar `Program.cs`:
```csharp
options.UseNpgsql(connectionString);
```

3. Actualizar `appsettings.json`:
```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Database=MatchupCompanionDb;Username=postgres;Password=password"
}
```

4. Crear nueva migración:
```bash
dotnet ef migrations add InitialPostgreSQL
dotnet ef database update
```

### Habilitar HTTPS en Desarrollo

```bash
dotnet dev-certs https --trust
```

### Configurar Logging con Serilog (Opcional)

```bash
dotnet add package Serilog.AspNetCore
```

Actualizar `Program.cs` para usar Serilog en lugar del logger por defecto.

## Próximos Pasos Recomendados

### Funcionalidad

1. **Autenticación y Autorización**
   - Implementar ASP.NET Identity
   - JWT tokens para el frontend
   - Roles: Admin, Contributor, User

2. **Sistema de Votación**
   - Upvote/downvote para tips
   - Ordenar por popularidad

3. **Búsqueda Avanzada**
   - Filtros por dificultad
   - Búsqueda por texto en consejos
   - Estadísticas agregadas

4. **Cache**
   - Redis para campeones
   - Memory cache para roles
   - Cache de matchups populares

### Performance

1. **Paginación**
   - Implementar en endpoints de listado
   - Usar `Skip()` y `Take()`

2. **Rate Limiting**
   - Proteger endpoints públicos
   - Prevenir abuso

3. **Índices de Base de Datos**
   - Ya implementados en migraciones
   - Monitorear queries lentas

### DevOps

1. **Containerización**
   - Dockerfile para la API
   - docker-compose con SQL Server

2. **CI/CD**
   - GitHub Actions para build y tests
   - Deploy automático a Azure/AWS

3. **Monitoreo**
   - Application Insights
   - Health checks

## Documentación Adicional

- [README-API.md](MatchupCompanion.API/README-API.md): Documentación detallada de la API
- [Swagger UI](https://localhost:7xxx/): Documentación interactiva en tiempo de ejecución

## Troubleshooting

### Error de Conexión a Base de Datos

**Problema:** "Cannot connect to SQL Server"

**Solución:**
1. Verifica que SQL Server esté corriendo
2. Revisa la cadena de conexión en `appsettings.json`
3. Si usas LocalDB: `sqllocaldb start mssqllocaldb`

### Error en Migraciones

**Problema:** "Build failed" al ejecutar `dotnet ef`

**Solución:**
1. Asegúrate de estar en la carpeta del proyecto (.csproj)
2. Compila primero: `dotnet build`
3. Verifica que los paquetes de EF Core estén instalados

### Swagger no Muestra Endpoints

**Problema:** Swagger UI está vacío

**Solución:**
1. Verifica que `app.MapControllers()` esté en `Program.cs`
2. Asegúrate de que los controllers estén decorados con `[ApiController]`
3. Revisa la ruta: debe ser la raíz `/` en desarrollo

## Contribuir

Si encuentras bugs o tienes sugerencias:

1. Abre un issue describiendo el problema
2. Si quieres contribuir código, crea un fork y abre un PR
3. Sigue las convenciones de código del proyecto

## Contacto

Alejandro Burciaga Calzadillas

---

Última actualización: Enero 2026
