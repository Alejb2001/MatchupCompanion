# Estado Actual del Proyecto - Matchup Companion

**Última actualización**: 20 de Enero, 2026
**Desarrollador**: Alejandro Burciaga Calzadillas

---

## Resumen Ejecutivo

**Estado General**: Backend y Frontend funcionales y operativos con autenticación completa
**Versión**: 0.4.0 (Alpha)
**Base de Datos**: Configurada con 172 campeones, 200+ items, runas y sistema de usuarios (en español)
**Frontend**: Blazor WebAssembly con sistema de edición de matchups, autocompletado y autenticación JWT
**Autenticación**: Sistema completo de login, registro y sesiones de invitado implementado

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
- 13 tablas principales:
  - **Juego**: `GameRoles`, `Champions`, `Matchups`, `MatchupTips`, `Runes`, `Items`
  - **Identity**: `AspNetUsers`, `AspNetRoles`, `AspNetUserRoles`, `AspNetUserClaims`, `AspNetRoleClaims`, `AspNetUserLogins`, `AspNetUserTokens`
  - **Sistema**: `__EFMigrationsHistory`
- Relaciones entre tablas configuradas correctamente
- 5 roles de juego predefinidos (Top, Jungle, Mid, ADC, Support)
- 172 campeones sincronizados desde Data Dragon (español)
- 200+ items sincronizados desde Data Dragon (español)
- Runas sincronizadas desde Data Dragon (español)
- Sistema de usuarios con ASP.NET Core Identity configurado

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
- `IAuthService` / `AuthService` - Autenticación y gestión de usuarios

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
- GET - Obtener todos los matchups (público)
- GET /{id} - Obtener matchup por ID (público)
- GET /champion/{championId} - Matchups de un campeón (público)
- GET /specific - Matchup específico (playerChampionId, enemyChampionId) (público)
- POST - Crear matchup ⚠️ **Requiere autenticación - No invitados**
- PUT /{id} - Actualizar matchup ⚠️ **Requiere autenticación - No invitados**
- DELETE /{id} - Eliminar matchup ⚠️ **Requiere autenticación - No invitados**
- POST /tips - Agregar tip ⚠️ **Requiere autenticación - No invitados**

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

**AuthController** (`/api/Auth`) - ⭐ NUEVO:
- POST /register - Registrar nuevo usuario
- POST /login - Iniciar sesión (retorna JWT token)
- POST /guest - Crear sesión de invitado (24 horas)
- POST /logout - Cerrar sesión
- GET /me - Obtener usuario actual (requiere autenticación)
- GET /validate - Validar token JWT

#### Modelos de Datos

**Entidades**:
- `Champion` - Campeones de LoL
- `Role` - Roles/Líneas (Top, Jungle, Mid, ADC, Support)
- `Matchup` - Enfrentamiento entre dos campeones (con campos de estrategia, items, runas y creador)
- `MatchupTip` - Consejos específicos para un matchup (con autor)
- `Rune` - Runas de LoL (sincronizadas desde Data Dragon)
- `Item` - Items de LoL (sincronizados desde Data Dragon)
- `ApplicationUser` - Usuario del sistema (extiende IdentityUser) ⭐ NUEVO
  - Campos: DisplayName, PreferredRoleId, CreatedAt, LastLoginAt, IsGuest, GuestExpiresAt
  - Relaciones: CreatedMatchups, CreatedTips

**DTOs**:
- `ChampionDto`, `CreateChampionDto`, `UpdateChampionDto`
- `MatchupDto`, `CreateMatchupDto`, `UpdateMatchupDto` (incluye campos para items y runas)
- `MatchupTipDto`, `CreateMatchupTipDto`, `UpdateMatchupTipDto`
- `RoleDto`
- `RuneDto`
- `ItemDto`
- **Auth DTOs** ⭐ NUEVO:
  - `LoginRequest`, `RegisterRequest`, `AuthResponse`, `UserDto`, `RefreshTokenRequest`

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
- `IAuthenticationService` / `AuthenticationService` - Autenticación y gestión de tokens ⭐ NUEVO

#### Autenticación ⭐ NUEVO
- `CustomAuthenticationStateProvider` - Proveedor de estado de autenticación
- Tokens JWT almacenados en localStorage (Blazored.LocalStorage)
- Validación automática de expiración de tokens
- Actualización automática de headers de autorización

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

**CreateMatchup.razor** (`/create-matchup`) ⚠️ **Requiere autenticación - No invitados**:
- Formulario para crear nuevo matchup
- Selección de campeón jugador
- Selección de campeón enemigo
- Selección de rol
- Selección de dificultad
- Campo de consejos generales
- Validación de formulario
- Redirección después de crear

**EditMatchup.razor** (`/edit-matchup/{id}`) ⚠️ **Requiere autenticación - No invitados**:
- Formulario para editar matchup existente
- **Búsqueda con autocompletado** para campeones (campos de texto con filtrado en tiempo real)
- **Búsqueda con autocompletado** para items
- Selección de items por categoría (Iniciales, Core, Situacionales) con botones
- Badges mostrando items seleccionados con opción de eliminar
- Dropdown para rol (solo 5 opciones)
- Campo de notas de estrategia
- Validación de formulario

**AddTip.razor** (`/add-tip/{matchupId}`) ⚠️ **Requiere autenticación**:
- Formulario para agregar tip a matchup
- Selección de categoría (Early Game, Mid Game, Late Game, Items, Runes, General)
- Campo de descripción del tip
- Selección de prioridad (1-5)
- Validación de formulario
- Redirección a detalles del matchup después de agregar

**Login.razor** (`/login`) ⭐ NUEVO:
- Formulario de inicio de sesión
- Campos: Email, Contraseña
- Checkbox "Recordarme"
- Botón "Continuar como Invitado" (crea sesión de 24 horas)
- Manejo de errores
- Redirección a página original tras login exitoso

**Register.razor** (`/register`) ⭐ NUEVO:
- Formulario de registro de usuario
- Campos: Email, Nombre de Usuario, Nombre para Mostrar, Contraseña, Confirmar Contraseña
- Selección de rol preferido de LoL (opcional)
- Validación de contraseñas (min 6 caracteres, mayúsculas, minúsculas, números)
- Redirección automática tras registro exitoso

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
- **Auth DTOs** ⭐ NUEVO:
  - `LoginRequest` - Datos de inicio de sesión
  - `RegisterRequest` - Datos de registro
  - `AuthResponse` - Respuesta con token JWT y datos de usuario
  - `UserDto` - Información de usuario

---

## Componentes Pendientes

### Funcionalidades Adicionales
- ~~Autenticación y autorización (ASP.NET Core Identity)~~ ✅ **COMPLETADO** (20/01/2026)
- Sistema de votación para tips
- Recuperación de contraseña (forgot password)
- Confirmación de email
- Refresh tokens para renovación automática
- OAuth (Google, Discord, Riot Games)
- Roles de administrador y moderador
- Caching (Redis o in-memory)
- Tests unitarios
- Tests de integración
- CI/CD pipeline
- Deployment a producción

---

## Seguridad

### Configuración de Autenticación ⭐ NUEVO

**Estado**: Sistema de autenticación completo implementado

**Características de seguridad**:
- JWT tokens con firma HMAC SHA256
- Tokens de 60 minutos de expiración (configurable)
- Contraseñas hasheadas con ASP.NET Core Identity
- Validación de contraseñas: min 6 caracteres, mayúsculas, minúsculas, números
- Protección de endpoints con `[Authorize]`
- Validación adicional para bloquear usuarios invitados en endpoints de edición
- Tokens almacenados en localStorage del navegador
- Sesiones de invitado con expiración de 24 horas

**IMPORTANTE**: La clave secreta JWT en `appsettings.json` es para desarrollo. **CAMBIARLA EN PRODUCCIÓN** y usar variables de entorno.

**Control de acceso**:
- **Usuarios registrados**: Pueden crear, editar y eliminar matchups y tips
- **Invitados**: Solo pueden ver matchups (sesión de 24 horas)
- **No autenticados**: Redirigidos a `/login` al intentar acceder a rutas protegidas

### Configuración de Data Dragon

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

6. **Conflicto de tabla Roles con IdentityRole** (20/01/2026)
   - Problema: La tabla `Roles` de LoL colisionaba con `Roles` de Identity
   - Solución: Renombrar DbSet a `GameRoles` y tabla a `GameRoles` en OnModelCreating
   - Ubicación: `ApplicationDbContext.cs`, `RoleRepository.cs`

4. **Variable Duplicada en MatchupsList** (16/01/2026)
   - Problema: `error CS0102: El tipo 'MatchupsList' ya contiene una definición para 'filterText'`
   - Solución: Remover declaración simple en línea 99, mantener propiedad con backing field
   - Ubicación: `MatchupsList.razor:99`

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
1. ~~Implementar autenticación básica con ASP.NET Core Identity~~ ✅ **COMPLETADO**
2. Implementar recuperación de contraseña
3. Agregar confirmación de email
4. Implementar refresh tokens
5. Agregar caching para mejorar rendimiento
6. Sistema de votación para tips

### Largo Plazo
1. Estadísticas y analytics
2. Deployment a Azure/AWS
3. Integración con más APIs de Riot (match history, win rates)

---

## Contacto y Soporte

**Desarrollador**: Alejandro Burciaga Calzadillas

Para reportar problemas o sugerencias, crear un issue en el repositorio.

---

## Documentación Adicional

- [AUTENTICACION.md](AUTENTICACION.md) - Guía completa del sistema de autenticación
- [ARCHITECTURE.md](ARCHITECTURE.md) - Arquitectura del proyecto
- [README.md](README.md) - Documentación general

---

**Última revisión**: 20 de Enero, 2026
