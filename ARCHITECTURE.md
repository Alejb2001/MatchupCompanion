# Matchup Companion - Documentación de Arquitectura

## Descripción General

Matchup Companion es una aplicación web full-stack diseñada para que los jugadores de League of Legends gestionen y compartan estrategias de matchups entre campeones. El sistema utiliza una arquitectura moderna con Blazor WebAssembly para el frontend y ASP.NET Core Web API para el backend.

## Stack Tecnológico

### Frontend
- **Blazor WebAssembly** - Framework SPA del lado del cliente
- **Bootstrap 5** - Componentes UI y diseño responsivo
- **Blazored.LocalStorage** - Gestión de almacenamiento del navegador
- **C# / .NET 8** - Lenguaje de programación y runtime

### Backend
- **ASP.NET Core 8 Web API** - API RESTful
- **Entity Framework Core** - ORM para acceso a base de datos
- **ASP.NET Core Identity** - Autenticación y autorización
- **JWT Bearer Authentication** - Autenticación sin estado
- **SQL Server LocalDB** - Base de datos de desarrollo

### Servicios Externos
- **Riot Games Data Dragon API** - Datos de campeones, runas, objetos y hechizos

## Diagrama de Arquitectura

```
+-------------------------------------------------------------+
|                  Cliente Blazor WebAssembly                  |
|                  (MatchupCompanion.Client)                   |
+-------------------------------------------------------------+
|  Páginas: Login, Register, MatchupSearch, MatchupDetail      |
|          CreateMatchup, EditMatchup, AddTip                  |
|                           |                                  |
|  Servicios: AuthenticationService, MatchupService            |
|            ChampionService, RoleService, RuneService         |
|            ItemService, SummonerSpellService                 |
|                           |                                  |
|  Handlers: AuthorizationMessageHandler (inyección JWT)       |
+-----------------------------+--------------------------------+
                              | HTTP/HTTPS + JWT
                              v
+-------------------------------------------------------------+
|                  ASP.NET Core Web API                        |
|                  (MatchupCompanion.API)                      |
+-------------------------------------------------------------+
|  Controllers: AuthController, MatchupsController             |
|              ChampionsController, RolesController            |
|              RunesController, ItemsController                |
|              SummonerSpellsController, RiotSyncController    |
|                           |                                  |
|  Services: AuthService, MatchupService                       |
|           ChampionService, RiotApiService                    |
|                           |                                  |
|  Repositories: MatchupRepository, ChampionRepository         |
|               RoleRepository, RuneRepository                 |
|               ItemRepository, SummonerSpellRepository        |
|                           |                                  |
|  Data: ApplicationDbContext (EF Core)                        |
+-----------------------------+--------------------------------+
                              |
                              v
                  +------------------+
                  |   SQL Server     |
                  |   LocalDB        |
                  +------------------+
```

## Autenticación y Autorización

### Flujo de Autenticación

1. **Registro/Login de Usuario**
   - El usuario envía credenciales a `/api/auth/login` o `/api/auth/register`
   - El backend valida las credenciales y genera un token JWT
   - El token se devuelve junto con información del usuario

2. **Almacenamiento del Token**
   - El frontend almacena el token en LocalStorage del navegador
   - El token incluye claims: UserId, Username, Email, DisplayName, IsGuest, Roles

3. **Autenticación de Requests**
   - `AuthorizationMessageHandler` intercepta todas las peticiones HTTP
   - Añade automáticamente el header `Authorization: Bearer <token>`
   - El backend valida la firma del token, emisor, audiencia y expiración

4. **Tipos de Sesión**
   - **Usuarios Regulares**: Acceso completo (crear, editar, eliminar matchups propios)
   - **Usuarios Invitados**: Acceso de solo lectura (expiración de 24 horas)
   - **Administradores**: Acceso completo a todos los matchups

### Reglas de Autorización

| Recurso | Invitado | Usuario | Creador | Admin |
|---------|----------|---------|---------|-------|
| Ver Matchups | Si | Si | Si | Si |
| Crear Matchup | No | Si | Si | Si |
| Editar Matchup | No | No | Si | Si |
| Eliminar Matchup | No | No | Si | Si |
| Agregar Tip | No | Si | Si | Si |

### Configuración JWT

- **Clave Secreta**: Configurada en appsettings.json
- **Emisor**: `MatchupCompanionAPI`
- **Audiencia**: `MatchupCompanionClient`
- **Expiración**: 60 minutos (configurable)
- **Validación**: Firma, emisor, audiencia y tiempo de vida

## Modelos de Datos

### Entidades Principales

**Matchup**
- Representa un enfrentamiento entre campeones en un rol específico
- Contiene calificación de dificultad, consejos generales y estrategia detallada
- Incluye runas, objetos, hechizos de invocador y orden de habilidades recomendados
- Registra el creador (CreatedById) y marcas de tiempo

**MatchupTip**
- Consejo específico categorizado por fase del juego o tema
- Vinculado a un matchup
- Incluye calificación de prioridad e información del autor

**Champion**
- Datos del campeón de League of Legends desde la API de Riot
- Incluye nombre, título, imagen, descripción y rol principal

**Rune**
- Datos de runas organizados por árboles (Precisión, Dominación, etc.)
- Incluye piedras angulares y runas secundarias

**Item**
- Datos de objetos con estadísticas, costo y rutas de construcción

**SummonerSpell**
- Datos de hechizos de invocador con tiempos de recarga y efectos

### Entidades de Autenticación

**ApplicationUser** (extiende IdentityUser)
- Cuenta de usuario con nombre para mostrar
- Seguimiento de usuario invitado (IsGuest, GuestExpiresAt)
- Vinculado a matchups creados

## Endpoints de la API

### Autenticación (`/api/auth`)
- `POST /register` - Crear nueva cuenta de usuario
- `POST /login` - Autenticar y obtener token JWT
- `POST /guest` - Crear sesión temporal de invitado
- `POST /refresh` - Refrescar token expirado
- `GET /me` - Obtener información del usuario actual
- `GET /validate` - Validar token
- `POST /logout` - Cerrar sesión (lado del cliente)

### Matchups (`/api/matchups`)
- `GET /` - Obtener todos los matchups
- `GET /{id}` - Obtener matchup por ID
- `GET /search` - Buscar matchup por campeones y rol
- `POST /` - Crear nuevo matchup (requiere autenticación)
- `PUT /{id}` - Actualizar matchup (requiere autenticación + propiedad)
- `DELETE /{id}` - Eliminar matchup (requiere autenticación + propiedad)
- `POST /tips` - Agregar tip a matchup (requiere autenticación)

### Datos del Juego
- `/api/champions` - Datos de campeones
- `/api/roles` - Datos de roles
- `/api/runes` - Datos de runas y árboles de runas
- `/api/items` - Datos de objetos
- `/api/summonerspells` - Datos de hechizos de invocador
- `/api/riotsync` - Activar sincronización manual de datos

## Características Principales

### 1. Gestión de Matchups
- Operaciones CRUD (Crear, Leer, Actualizar, Eliminar)
- Detalles completos: runas, objetos, hechizos, orden de habilidades, estrategia
- Control de acceso basado en permisos
- Búsqueda por campeón y rol

### 2. Sistema de Autenticación
- Autenticación sin estado basada en JWT
- Autorización basada en roles (Admin, Usuario, Invitado)
- Inyección automática de token via handler HTTP
- Requisitos de contraseña seguros

### 3. Sincronización de Datos
- Sincronización automática desde Riot Data Dragon al iniciar
- Sincronización manual disponible via endpoint de API
- Soporte para idioma español

### 4. Experiencia de Usuario
- UI responsiva con Bootstrap
- Validación en tiempo real
- Estados de carga y manejo de errores
- Modales de confirmación para acciones destructivas

## Consideraciones de Seguridad

### Implementado
- Validación de firma JWT
- Verificación de expiración de token
- Soporte HTTPS (configurable)
- Requisitos de complejidad de contraseña
- Autorización basada en roles
- Restricciones para usuarios invitados
- Política CORS (configurable)

### Recomendaciones para Producción
- Cambiar la clave secreta JWT
- Habilitar HTTPS obligatorio
- Configurar política CORS restrictiva
- Agregar rate limiting
- Implementar rotación de refresh tokens
- Agregar logging de requests
- Habilitar confirmación de email

## Esquema de Base de Datos

### Tablas de Identity (ASP.NET Core Identity)
- AspNetUsers - Cuentas de usuario
- AspNetRoles - Roles de usuario
- AspNetUserRoles - Relaciones usuario-rol
- AspNetUserClaims, AspNetUserLogins, AspNetUserTokens

### Tablas de la Aplicación
- Champions - Datos de campeones
- GameRoles - Datos de roles de línea
- Runes - Datos de runas
- RuneTrees - Datos de árboles de runas
- Items - Datos de objetos
- SummonerSpells - Datos de hechizos de invocador
- Matchups - Estrategias de matchups
- MatchupTips - Tips específicos de matchups

## Configuración de Desarrollo

### Requisitos Previos
- .NET 8 SDK
- Visual Studio 2022 o VS Code
- SQL Server LocalDB

### Ejecutar la Aplicación

1. **Backend**
```bash
cd MatchupCompanion.API
dotnet run
```
La API estará disponible en `http://localhost:5007`

2. **Frontend**
```bash
cd MatchupCompanion.Client
dotnet run
```
El cliente estará disponible en `http://localhost:5173`

### Primera Ejecución
- La base de datos se crea automáticamente
- Los datos de Riot se sincronizan en el primer inicio
- Usuario administrador por defecto: `admin@matchup.com` / `Admin123`

## Estructura del Proyecto

```
MatchupCompanion/
├── MatchupCompanion.API/          # Backend Web API
│   ├── Controllers/               # Endpoints de API
│   ├── Services/                  # Lógica de negocio
│   ├── Data/                      # Contexto EF Core y repositorios
│   ├── Models/                    # Entidades y DTOs
│   ├── ExternalServices/          # Integración con API de Riot
│   └── Migrations/                # Migraciones de EF Core
├── MatchupCompanion.Client/       # Frontend Blazor WebAssembly
│   ├── Pages/                     # Páginas Razor
│   ├── Services/                  # Servicios cliente HTTP
│   ├── Handlers/                  # Handlers de mensajes HTTP
│   └── wwwroot/                   # Recursos estáticos
└── MatchupCompanion.Shared/       # DTOs y modelos compartidos
    └── Models/                    # Objetos de transferencia de datos
```
