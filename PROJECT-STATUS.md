# Estado Actual del Proyecto - Matchup Companion

**Última actualización**: 19 de Enero, 2026
**Desarrollador**: Alejandro Burciaga Calzadillas

---

## Resumen Ejecutivo

**Estado General**: Backend y Frontend funcionales y operativos
**Versión**: 0.3.0 (Alpha)
**Base de Datos**: Configurada con 172 campeones, 200+ items y runas (en español)
**Frontend**: Blazor WebAssembly con sistema de edición de matchups y autocompletado

---

## Componentes Completados

### 1. Backend API (.NET 8)

#### Infraestructura
- ASP.NET Core Web API configurada
- Entity Framework Core 8 implementado
- SQL Server LocalDB configurado
- Swagger UI habilitado (http://localhost:5007)
- CORS configurado para desarrollo (HTTP puerto 5007)
- Logging configurado
- Sincronización automática al iniciar

#### Base de Datos
- Migraciones creadas y aplicadas
- 7 tablas: `Roles`, `Champions`, `Matchups`, `MatchupTips`, `Runes`, `Items`, `__EFMigrationsHistory`
- Relaciones entre tablas configuradas correctamente
- 5 roles predefinidos (Top, Jungle, Mid, ADC, Support)
- 172 campeones sincronizados desde Data Dragon (español)
- 200+ items sincronizados desde Data Dragon (español)
- Runas sincronizadas desde Data Dragon (español)

**Detalles de la BD**:
```
Instancia: (localdb)\mssqllocaldb
Base de datos: MatchupCompanionDb
Ubicación: C:\Users\alejb\AppData\Local\Microsoft\Microsoft SQL Server Local DB\Instances\MSSQLLocalDB\
```

#### Arquitectura Implementada

**Patrón Repository**:
- `IChampionRepository` / `ChampionRepository`
- `IMatchupRepository` / `MatchupRepository`
- `IMatchupTipRepository` / `MatchupTipRepository`
- `IRoleRepository` / `RoleRepository`
- `IRuneRepository` / `RuneRepository`
- `IItemRepository` / `ItemRepository`

**Servicios de Negocio**:
- `IChampionService` / `ChampionService`
- `IMatchupService` / `MatchupService`

**Servicios Externos**:
- `RiotApiService` - Sincronización con Data Dragon
  - Obtiene la versión más reciente de Data Dragon
  - Sincroniza campeones automáticamente (es_ES)
  - Sincroniza runas automáticamente (es_ES)
  - Sincroniza items automáticamente (es_ES) con limpieza de HTML
  - Deserialización JSON con `[JsonPropertyName]` correctamente configurada
  - Manejo de errores y logging detallado

#### Controladores (Endpoints)

**ChampionsController** (`/api/Champions`):
- GET - Obtener todos los campeones
- GET /{id} - Obtener campeón por ID
- GET /riot/{riotId} - Obtener campeón por RiotChampionId
- GET /role/{roleId} - Obtener campeones por rol
- POST - Crear campeón
- PUT /{id} - Actualizar campeón
- DELETE /{id} - Eliminar campeón

**MatchupsController** (`/api/Matchups`):
- GET - Obtener todos los matchups
- GET /{id} - Obtener matchup por ID
- GET /champion/{championId} - Matchups de un campeón
- GET /specific - Matchup específico (playerChampionId, enemyChampionId)
- POST - Crear matchup
- PUT /{id} - Actualizar matchup
- DELETE /{id} - Eliminar matchup

**MatchupTipsController** (`/api/MatchupTips`):
- GET - Obtener todos los tips
- GET /{id} - Obtener tip por ID
- GET /matchup/{matchupId} - Tips de un matchup
- GET /category/{category} - Tips por categoría
- POST - Crear tip
- PUT /{id} - Actualizar tip
- DELETE /{id} - Eliminar tip

**RolesController** (`/api/Roles`):
- GET - Obtener todos los roles
- GET /{id} - Obtener rol por ID

**RunesController** (`/api/Runes`):
- GET - Obtener todas las runas
- GET /{id} - Obtener runa por ID

**ItemsController** (`/api/Items`):
- GET - Obtener todos los items
- GET /{id} - Obtener item por ID

**RiotSyncController** (`/api/RiotSync`):
- POST /sync-champions - Sincronizar campeones desde Data Dragon
- POST /sync-runes - Sincronizar runas desde Data Dragon
- POST /sync-items - Sincronizar items desde Data Dragon
- POST /sync-all - Sincronizar todo (campeones, runas, items)
- GET /version - Obtener versión actual de Data Dragon

#### Modelos de Datos

**Entidades**:
- `Champion` - Campeones de LoL
- `Role` - Roles/Líneas (Top, Jungle, Mid, ADC, Support)
- `Matchup` - Enfrentamiento entre dos campeones (con campos de estrategia, items y runas)
- `MatchupTip` - Consejos específicos para un matchup
- `Rune` - Runas de LoL (sincronizadas desde Data Dragon)
- `Item` - Items de LoL (sincronizados desde Data Dragon)

**DTOs**:
- `ChampionDto`, `CreateChampionDto`, `UpdateChampionDto`
- `MatchupDto`, `CreateMatchupDto`, `UpdateMatchupDto` (incluye campos para items y runas)
- `MatchupTipDto`, `CreateMatchupTipDto`, `UpdateMatchupTipDto`
- `RoleDto`
- `RuneDto`
- `ItemDto`

---

### 2. Frontend (Blazor WebAssembly)

#### Infraestructura
- Blazor WebAssembly configurado
- HttpClient con base URL configurada (http://localhost:5007)
- Inyección de dependencias configurada
- Bootstrap 5 integrado
- Navegación con NavMenu

#### Servicios HTTP
- `IChampionService` / `ChampionService` - Comunicación con API de campeones
- `IRoleService` / `RoleService` - Comunicación con API de roles
- `IMatchupService` / `MatchupService` - Comunicación con API de matchups
- `IRuneService` / `RuneService` - Comunicación con API de runas
- `IItemService` / `ItemService` - Comunicación con API de items

**Métodos implementados**:
- GetAllChampionsAsync(), GetChampionByIdAsync(int id), GetChampionsByRoleAsync(int roleId)
- GetAllRolesAsync(), GetRoleByIdAsync(int id)
- GetAllMatchupsAsync(), GetMatchupByIdAsync(int id), GetMatchupsByChampionAsync(int championId)
- GetSpecificMatchupAsync(int playerChampionId, int enemyChampionId)
- CreateMatchupAsync(CreateMatchupDto dto), UpdateMatchupAsync(UpdateMatchupDto dto)
- CreateMatchupTipAsync(CreateMatchupTipDto dto)
- GetAllRunesAsync(), GetAllItemsAsync()

#### Páginas Implementadas

**Home.razor**:
- Página de bienvenida con descripción del proyecto
- Enlaces a las funcionalidades principales

**MatchupSearch.razor** (`/matchup-search`):
- Selección de campeón jugador
- Selección de campeón enemigo
- Búsqueda de matchup específico
- Navegación a detalles del matchup si existe
- Redirección a creación si no existe

**MatchupsList.razor** (`/matchups`):
- Lista completa de matchups
- Filtrado dinámico por nombre de campeón
- Visualización de dificultad
- Enlaces a detalles de cada matchup

**MatchupDetail.razor** (`/matchup-detail/{id}`):
- Información detallada del matchup
- Visualización de campeones y rol
- Lista de tips organizados
- Navegación a agregar tips

**CreateMatchup.razor** (`/create-matchup`):
- Formulario para crear nuevo matchup
- Selección de campeón jugador
- Selección de campeón enemigo
- Selección de rol
- Selección de dificultad
- Campo de consejos generales
- Validación de formulario
- Redirección después de crear

**EditMatchup.razor** (`/edit-matchup/{id}`):
- Formulario para editar matchup existente
- **Búsqueda con autocompletado** para campeones (campos de texto con filtrado en tiempo real)
- **Búsqueda con autocompletado** para items
- Selección de items por categoría (Iniciales, Core, Situacionales) con botones
- Badges mostrando items seleccionados con opción de eliminar
- Dropdown para rol (solo 5 opciones)
- Campo de notas de estrategia
- Validación de formulario

**AddTip.razor** (`/add-tip/{matchupId}`):
- Formulario para agregar tip a matchup
- Selección de categoría (Early Game, Mid Game, Late Game, Items, Runes, General)
- Campo de descripción del tip
- Selección de prioridad (1-5)
- Validación de formulario
- Redirección a detalles del matchup después de agregar

#### Modelos Compartidos (MatchupCompanion.Shared)

**DTOs implementados**:
- `ChampionDto` - Representación de campeón
- `RoleDto` - Representación de rol
- `MatchupDto` - Representación de matchup con navegación, items y runas
- `MatchupTipDto` - Representación de tip
- `CreateMatchupDto` - DTO para crear matchup
- `UpdateMatchupDto` - DTO para actualizar matchup (incluye items y runas)
- `CreateMatchupTipDto` - DTO para crear tip
- `RuneDto` - Representación de runa
- `ItemDto` - Representación de item

---

## Componentes Pendientes

### Funcionalidades Adicionales
- Autenticación y autorización (ASP.NET Core Identity)
- Sistema de votación para tips
- Caching (Redis o in-memory)
- Tests unitarios
- Tests de integración
- CI/CD pipeline
- Deployment a producción

---

## Seguridad

### Configuración

**Estado**: Configurado correctamente

**Nota**: La sincronización con Data Dragon **no requiere API key**. Los datos se obtienen del CDN público de Riot Games.

**Configuración de .gitignore**:
```
# Archivos de configuración local NO se suben a Git
**/appsettings.*.json
!**/appsettings.json
```

---

## Cómo Usar el Sistema Actual

### 1. Iniciar la API
```bash
cd MatchupCompanion.API
dotnet run
```

### 2. Acceder a Swagger
- HTTP: http://localhost:5007
- HTTPS: https://localhost:7285

### 3. Endpoints Principales

**Ver todos los campeones**:
```
GET /api/Champions
```

**Ver campeones de un rol específico**:
```
GET /api/Champions/role/1  # Top
GET /api/Champions/role/2  # Jungle
GET /api/Champions/role/3  # Mid
GET /api/Champions/role/4  # ADC
GET /api/Champions/role/5  # Support
```

**Crear un matchup**:
```
POST /api/Matchups
{
  "playerChampionId": 1,
  "enemyChampionId": 2,
  "roleId": 3,
  "difficulty": "Medium",
  "generalAdvice": "Play safe early game"
}
```

**Agregar un tip a un matchup**:
```
POST /api/MatchupTips
{
  "matchupId": 1,
  "category": "Early Game",
  "description": "Ward at 2:30 to spot jungle ganks",
  "priority": 1
}
```

**Resincronizar datos** (español):
```
POST /api/RiotSync/sync-all?language=es_ES
```

**Resincronizar individualmente**:
```
POST /api/RiotSync/sync-champions?language=es_ES
POST /api/RiotSync/sync-runes?language=es_ES
POST /api/RiotSync/sync-items?language=es_ES
```

### 4. Consultas SQL Útiles

**Ver estadísticas de la BD**:
```sql
-- Total de campeones
SELECT COUNT(*) as TotalChampions FROM Champions;

-- Campeones por rol
SELECT r.Name as Role, COUNT(c.Id) as Count
FROM Champions c
LEFT JOIN Roles r ON c.PrimaryRoleId = r.Id
GROUP BY r.Name;

-- Matchups creados
SELECT
    pc.Name as PlayerChampion,
    ec.Name as EnemyChampion,
    r.Name as Role,
    m.Difficulty
FROM Matchups m
JOIN Champions pc ON m.PlayerChampionId = pc.Id
JOIN Champions ec ON m.EnemyChampionId = ec.Id
JOIN Roles r ON m.RoleId = r.Id;
```

---

## Notas Técnicas

### Problemas Resueltos Recientemente

1. **Deserialización JSON de Data Dragon** (16/01/2026)
   - Problema: La API retornaba 0 campeones
   - Solución: Agregar atributos `[JsonPropertyName]` a todas las clases de deserialización
   - Resultado: 172 campeones sincronizados exitosamente

2. **Truncamiento de nombres de items** (19/01/2026)
   - Problema: "String or binary data would be truncated" en columna Name de Items
   - Solución: Aumentar MaxLength de 100 a 300 y añadir función StripHtmlTags()
   - Ubicación: `Item.cs`, `RiotApiService.cs`

3. **CORS con HTTPS** (19/01/2026)
   - Problema: Error CORS con status code null (problema de certificado SSL)
   - Solución: Usar HTTP en lugar de HTTPS para desarrollo local (puerto 5007)
   - Ubicación: `MatchupCompanion.Client/Program.cs`

4. **Comillas escapadas en Razor** (19/01/2026)
   - Problema: Error de compilación con `\"` en atributos onclick
   - Solución: Usar sintaxis `@(() => Method("string"))` en lugar de comillas escapadas
   - Ubicación: `EditMatchup.razor`

5. **Datos en inglés** (19/01/2026)
   - Problema: Campeones, runas e items se mostraban en inglés
   - Solución: Configurar sincronización automática con `language = "es_ES"` en Program.cs
   - Ubicación: `Program.cs` (auto-sync al iniciar)

### Configuración de Puertos

**HTTP**: 5007
**HTTPS**: 7285

Configurado en: `Properties/launchSettings.json`

### Versión de Data Dragon Actual

La API usa automáticamente la versión más reciente de Data Dragon.
Última versión detectada: ~14.24.x (se actualiza automáticamente)

---

## Próximos Pasos Recomendados

### Corto Plazo
1. Implementar tests unitarios para servicios y repositorios
2. Agregar validaciones más robustas en DTOs
3. Implementar paginación en endpoints GET
4. Mostrar imágenes de campeones en la interfaz

### Mediano Plazo
1. Implementar autenticación básica con ASP.NET Core Identity
2. Agregar caching para mejorar rendimiento
3. Sistema de votación para tips

### Largo Plazo
1. Estadísticas y analytics
2. Deployment a Azure/AWS
3. Integración con más APIs de Riot (match history, win rates)

---

## Contacto y Soporte

**Desarrollador**: Alejandro Burciaga Calzadillas

Para reportar problemas o sugerencias, crear un issue en el repositorio.

---

**Última revisión**: 19 de Enero, 2026
