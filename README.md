# Matchup Companion

Aplicación web full-stack construida con .NET 8 que permite a los jugadores de League of Legends buscar, crear y compartir estrategias para matchups de campeones.

**Estado**: Backend y frontend completamente funcionales con sistema de autenticación. Base de datos configurada con 172 campeones, 200+ items, runas y usuarios sincronizados desde Riot Games Data Dragon en español.

## Descripción del Proyecto

Esta aplicación web resuelve un problema común para los jugadores de League of Legends: "¿Cómo juego este matchup?" La aplicación permite a los usuarios:

1. **Acceder como invitado** o registrarse para crear una cuenta
2. Seleccionar dos campeones (tu campeón y el campeón enemigo)
3. Ver consejos, niveles de dificultad y estrategias enviadas por otros usuarios para ese matchup específico
4. **Registrarse para contribuir**: Agregar sus propios consejos y crear matchups
5. Configurar builds recomendados con items iniciales, core y situacionales
6. Seleccionar runas recomendadas para cada matchup

La aplicación cuenta con un frontend reactivo en Blazor WebAssembly que consume una API backend de ASP.NET Core.

## Stack de Tecnologías

Este proyecto usa una estructura de repositorio monolítico con una aplicación Blazor WebAssembly hospedada en ASP.NET Core.

**Backend (MatchupCompanion.API)**
- ASP.NET Core Web API (.NET 8) - Endpoints RESTful
- Entity Framework Core 8 - ORM y comunicación con base de datos
- **ASP.NET Core Identity** - Sistema de autenticación y gestión de usuarios
- **JWT Authentication** - Tokens seguros para autenticación stateless
- SQL Server LocalDB - Motor de base de datos (desarrollo)
- Swagger/OpenAPI - Documentación interactiva de la API
- Integración con Data Dragon - Sincronización automática de campeones, runas e items desde Riot Games
- Soporte multiidioma (datos sincronizados en español es_ES)

**Frontend (MatchupCompanion.Client)**
- Blazor WebAssembly - SPA interactiva que se ejecuta en el navegador
- C# - Lógica del cliente escrita en C# en lugar de JavaScript
- **AuthenticationStateProvider** - Gestión de estado de autenticación
- **Blazored.LocalStorage** - Almacenamiento de tokens JWT
- Bootstrap 5 - Diseño UI responsivo

**Compartido (MatchupCompanion.Shared)**
- DTOs (Data Transfer Objects) compartidos entre cliente y servidor para type safety

## Características

**Backend API**
- API RESTful completa con operaciones CRUD para Campeones, Matchups, Tips, Roles, Runas e Items
- **Sistema de autenticación completo** con registro, login y sesiones de invitado
- **Protección de endpoints**: Solo usuarios autenticados pueden crear/editar matchups
- **Control de acceso granular**: Los invitados solo pueden ver contenido
- 172 campeones, 200+ items y runas sincronizados automáticamente desde Data Dragon
- Sincronización automática al iniciar si la BD está vacía
- Datos en español (es_ES) desde Data Dragon
- Documentación interactiva con Swagger (con autenticación JWT integrada)
- Arquitectura de patrón Repository con capa de servicios
- Entity Framework Core con SQL Server LocalDB

**Frontend**
- Interfaz de usuario completa con Bootstrap 5
- **Sistema de autenticación completo**: Login, registro y modo invitado
- **Navegación dinámica**: Botones y menús que se adaptan al estado de autenticación
- **Rutas protegidas**: CreateMatchup, EditMatchup y AddTip requieren autenticación
- Búsqueda de matchups con selección de campeones y roles
- **Campos de búsqueda con autocompletado** para campeones e items
- Sistema de selección de items por categoría (Iniciales, Core, Situacionales)
- Lista de matchups con filtrado dinámico
- Vista detallada de matchup con tips categorizados
- Formularios para crear y editar matchups (solo usuarios autenticados)
- Servicios HTTP para comunicación con API (con tokens JWT)
- Diseño responsivo para móvil y escritorio

## Inicio Rápido

**Prerrequisitos**
- .NET 8 SDK
- SQL Server LocalDB (incluido con Visual Studio)
- Visual Studio 2022 o VS Code

**Configuración**

1. Clonar el repositorio
   ```bash
   git clone <repository-url>
   cd MatchupCompanion
   ```

2. Aplicar migraciones (si es necesario)
   ```bash
   cd MatchupCompanion.API
   dotnet ef database update
   ```

3. Ejecutar el backend API
   ```bash
   cd MatchupCompanion.API
   dotnet run
   ```
   - La API estará disponible en http://localhost:5007
   - Swagger UI en http://localhost:5007
   - **Sincronización automática**: Al iniciar, si la BD está vacía, se sincronizan automáticamente campeones, runas e items en español

4. Ejecutar el frontend (en otra terminal)
   ```bash
   cd MatchupCompanion.Client
   dotnet run
   ```
   - El cliente estará disponible en el puerto mostrado en consola
   - La API debe estar ejecutándose en http://localhost:5007

**Nota sobre autenticación**: Al iniciar por primera vez, puedes:
- **Registrarte** en `/register` para crear una cuenta y poder editar matchups
- **Continuar como invitado** para solo ver matchups sin necesidad de cuenta

**Nota sobre Data Dragon**: La sincronización no requiere API key. Los datos se obtienen del CDN público de Riot Games.

## Estructura del Proyecto

```
MatchupCompanion/
├── MatchupCompanion.API/           # Backend ASP.NET Core Web API
│   ├── Controllers/                # Endpoints de la API
│   ├── Services/                   # Lógica de negocio
│   ├── Data/                       # DbContext y Repositorios
│   ├── Models/                     # Entidades (Champion, Matchup, Rune, Item)
│   ├── ExternalServices/           # RiotApiService (Data Dragon sync)
│   └── Migrations/                 # Migraciones de EF Core
├── MatchupCompanion.Client/        # Frontend Blazor WebAssembly
│   ├── Pages/                      # Páginas Razor (incluyendo EditMatchup)
│   ├── Services/                   # Servicios HTTP
│   └── Layout/                     # Componentes de layout
├── MatchupCompanion.Shared/        # DTOs compartidos
│   └── Models/                     # DTOs incluyendo RuneDto, ItemDto
├── ARCHITECTURE.md                 # Documentación de arquitectura
├── PROJECT-STATUS.md               # Estado actual del proyecto
└── FRONTEND-GUIDE.md               # Guía del frontend
```

## Mejoras Futuras

- ~~Autenticación de usuarios con ASP.NET Core Identity~~ ✅ **Completado**
- Recuperación de contraseña (forgot password)
- Confirmación de email
- Refresh tokens automáticos
- OAuth (Google, Discord, Riot Games)
- Sistema de votación para tips
- Estadísticas avanzadas y win rates
- Imágenes de campeones en la interfaz
- Caching para optimización de rendimiento
- Tests unitarios y de integración

## Documentación

- [ARCHITECTURE.md](ARCHITECTURE.md) - Detalles de arquitectura y patrones
- [PROJECT-STATUS.md](PROJECT-STATUS.md) - Estado actual y próximos pasos
- [FRONTEND-GUIDE.md](FRONTEND-GUIDE.md) - Guía del frontend Blazor
- [AUTENTICACION.md](AUTENTICACION.md) - Guía completa del sistema de autenticación ⭐ NUEVO

---

**Creado por Alejandro Burciaga Calzadillas**
