# Matchup Companion API

API REST desarrollada con ASP.NET Core 8 para gestionar información de matchups de League of Legends.

## Estructura del Proyecto

El proyecto sigue una arquitectura limpia con separación de responsabilidades:

```
MatchupCompanion.API/
├── Controllers/              # Controladores de la API REST
│   ├── ChampionsController.cs
│   ├── MatchupsController.cs
│   ├── RolesController.cs
│   └── RiotSyncController.cs
├── Services/                 # Lógica de negocio
│   ├── Interfaces/
│   ├── ChampionService.cs
│   └── MatchupService.cs
├── Data/                     # Capa de acceso a datos
│   ├── Repositories/
│   │   ├── Interfaces/
│   │   ├── ChampionRepository.cs
│   │   ├── MatchupRepository.cs
│   │   ├── MatchupTipRepository.cs
│   │   └── RoleRepository.cs
│   └── ApplicationDbContext.cs
├── Models/                   # Modelos de datos
│   ├── Entities/            # Entidades de base de datos
│   │   ├── Champion.cs
│   │   ├── Matchup.cs
│   │   ├── MatchupTip.cs
│   │   └── Role.cs
│   └── DTOs/                # Data Transfer Objects
│       ├── ChampionDto.cs
│       ├── MatchupDto.cs
│       ├── CreateMatchupRequest.cs
│       └── CreateMatchupTipRequest.cs
├── ExternalServices/         # Servicios externos
│   └── RiotApiService.cs    # Integración con Riot API
└── Program.cs               # Configuración y punto de entrada
```

## Tecnologías Utilizadas

- **ASP.NET Core 8**: Framework web
- **Entity Framework Core 8**: ORM para acceso a datos
- **SQL Server**: Base de datos (LocalDB para desarrollo)
- **Swagger/OpenAPI**: Documentación de la API
- **Riot Games API**: Fuente de datos de campeones

## Configuración Inicial

### 1. Requisitos Previos

- .NET 8 SDK
- SQL Server o LocalDB
- Visual Studio 2022 o VS Code (opcional)

### 2. Configurar la Base de Datos

Edita `appsettings.json` para configurar tu cadena de conexión:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=MatchupCompanionDb;Trusted_Connection=true;MultipleActiveResultSets=true"
  }
}
```

**Alternativas de cadena de conexión:**

- **SQL Server Express:**
  ```
  Server=.\\SQLEXPRESS;Database=MatchupCompanionDb;Trusted_Connection=true;
  ```

- **PostgreSQL:**
  ```
  Host=localhost;Database=MatchupCompanionDb;Username=postgres;Password=tu_password
  ```
  (Requiere cambiar el paquete a `Npgsql.EntityFrameworkCore.PostgreSQL`)

### 3. Aplicar Migraciones

```bash
cd MatchupCompanion.API
dotnet ef database update
```

Esto creará la base de datos y todas las tablas necesarias.

### 4. Ejecutar la Aplicación

```bash
dotnet run
```

La API estará disponible en:
- HTTPS: `https://localhost:7xxx`
- HTTP: `http://localhost:5xxx`
- Swagger UI: `https://localhost:7xxx/` (raíz)

## Endpoints Principales

### Campeones

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| GET | `/api/champions` | Obtener todos los campeones |
| GET | `/api/champions/{id}` | Obtener un campeón por ID |
| GET | `/api/champions/role/{roleId}` | Obtener campeones por rol |

### Matchups

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| GET | `/api/matchups` | Obtener todos los matchups |
| GET | `/api/matchups/{id}` | Obtener un matchup por ID |
| GET | `/api/matchups/search?playerChampionId={id}&enemyChampionId={id}&roleId={id}` | Buscar matchup específico |
| GET | `/api/matchups/champion/{championId}` | Obtener matchups de un campeón |
| POST | `/api/matchups` | Crear un nuevo matchup |
| POST | `/api/matchups/tips` | Agregar un tip a un matchup |
| DELETE | `/api/matchups/{id}` | Eliminar un matchup |

### Roles

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| GET | `/api/roles` | Obtener todos los roles |
| GET | `/api/roles/{id}` | Obtener un rol por ID |

### Sincronización con Riot API

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| POST | `/api/riotsync/sync-champions?language=en_US` | Sincronizar campeones desde Riot |
| GET | `/api/riotsync/version` | Obtener versión actual de Data Dragon |

## Ejemplos de Uso

### 1. Sincronizar Campeones desde Riot API

```bash
curl -X POST "https://localhost:7xxx/api/riotsync/sync-champions?language=es_MX"
```

### 2. Crear un Matchup

```bash
curl -X POST "https://localhost:7xxx/api/matchups" \
  -H "Content-Type: application/json" \
  -d '{
    "playerChampionId": 2,
    "enemyChampionId": 3,
    "roleId": 3,
    "difficulty": "Hard",
    "generalAdvice": "Juega seguro en early game, Zed tiene mucho burst damage"
  }'
```

### 3. Agregar un Consejo a un Matchup

```bash
curl -X POST "https://localhost:7xxx/api/matchups/tips" \
  -H "Content-Type: application/json" \
  -d '{
    "matchupId": 1,
    "category": "EarlyGame",
    "content": "Evita tradear cuando Zed tiene sus sombras disponibles",
    "priority": 8,
    "authorName": "ProPlayer123"
  }'
```

### 4. Buscar un Matchup Específico

```bash
curl "https://localhost:7xxx/api/matchups/search?playerChampionId=2&enemyChampionId=3&roleId=3"
```

## Modelo de Datos

### Champion
- `Id`: int (PK)
- `RiotChampionId`: string (único)
- `Name`: string
- `Title`: string
- `ImageUrl`: string
- `Description`: string
- `PrimaryRoleId`: int (FK)

### Matchup
- `Id`: int (PK)
- `PlayerChampionId`: int (FK)
- `EnemyChampionId`: int (FK)
- `RoleId`: int (FK)
- `Difficulty`: string (Easy, Medium, Hard, Extreme)
- `GeneralAdvice`: string

### MatchupTip
- `Id`: int (PK)
- `MatchupId`: int (FK)
- `Category`: string (EarlyGame, MidGame, LateGame, Items, Runes, Abilities, General)
- `Content`: string
- `Priority`: int (1-10)
- `AuthorName`: string

### Role
- `Id`: int (PK)
- `Name`: string (Top, Jungle, Mid, ADC, Support)
- `Description`: string

## Integración con Riot Games API

El proyecto incluye `RiotApiService` que integra con Data Dragon (API pública de Riot):

- **Data Dragon** no requiere API key
- Proporciona datos estáticos de campeones
- Se actualiza con cada parche del juego

Para usar la API oficial de Riot (datos en vivo, estadísticas, etc.):
1. Obtén una API key en [Riot Developer Portal](https://developer.riotgames.com/)
2. Agrégala en `appsettings.json`:
   ```json
   "RiotApi": {
     "ApiKey": "TU_API_KEY_AQUI"
   }
   ```

## Próximas Mejoras

- [ ] Implementar autenticación con ASP.NET Identity
- [ ] Sistema de votación para tips (upvote/downvote)
- [ ] Paginación en endpoints de listado
- [ ] Cache con Redis para mejorar performance
- [ ] Búsqueda avanzada y filtros
- [ ] Estadísticas de winrate desde Riot API
- [ ] Integración con servicios de análisis (Lolalytics, U.GG, etc.)
- [ ] Rate limiting para prevenir abuso
- [ ] Logging estructurado con Serilog

## Buenas Prácticas Implementadas

- **Separación de responsabilidades** (Controllers → Services → Repositories)
- **Inyección de dependencias** para facilitar testing
- **Async/await** en todas las operaciones de I/O
- **DTOs** para evitar exponer entidades directamente
- **Validación** con Data Annotations
- **Logging** con ILogger
- **Documentación** con Swagger/OpenAPI
- **CORS** configurado para desarrollo
- **Migraciones** de Entity Framework para control de versiones de BD

## Comandos Útiles

```bash
# Compilar el proyecto
dotnet build

# Ejecutar la aplicación
dotnet run

# Ejecutar con hot reload
dotnet watch run

# Crear una nueva migración
dotnet ef migrations add NombreMigracion

# Aplicar migraciones
dotnet ef database update

# Revertir última migración
dotnet ef migrations remove

# Ver migraciones pendientes
dotnet ef migrations list

# Generar script SQL de migraciones
dotnet ef migrations script
```

## Troubleshooting

### Error: "A connection was successfully established..."
- Verifica que SQL Server esté corriendo
- Revisa la cadena de conexión en `appsettings.json`

### Error: "Cannot create database"
- Asegúrate de tener permisos en SQL Server
- Intenta ejecutar Visual Studio o tu terminal como administrador

### Error: "No service for type DbContext"
- Verifica que la configuración en `Program.cs` esté correcta
- Asegúrate de que los paquetes de Entity Framework estén instalados

## Autor

Alejandro Burciaga Calzadillas

## Licencia

Este proyecto es de código abierto y está disponible para fines educativos.
