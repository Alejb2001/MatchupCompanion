# Matchup Companion

Aplicación web full-stack construida con .NET 8 que permite a los jugadores de League of Legends buscar, crear y compartir estrategias para matchups de campeones.

Este proyecto demuestra competencia en el ecosistema .NET, incluyendo ASP.NET Core Web API, Blazor WebAssembly y Entity Framework Core.

**Estado**: Backend y frontend completamente funcionales. Base de datos configurada con 172 campeones sincronizados desde Riot Games Data Dragon.

## Descripción del Proyecto

Esta aplicación web resuelve un problema común para los jugadores de League of Legends: "¿Cómo juego este matchup?" La aplicación permite a los usuarios:

1. Seleccionar dos campeones (tu campeón y el campeón enemigo)
2. Ver consejos, niveles de dificultad y estrategias enviadas por otros usuarios para ese matchup específico
3. Contribuir agregando sus propios consejos para matchups

La aplicación cuenta con un frontend reactivo en Blazor WebAssembly que consume una API backend de ASP.NET Core.

## Stack de Tecnologías

Este proyecto usa una estructura de repositorio monolítico con una aplicación Blazor WebAssembly hospedada en ASP.NET Core.

**Backend (MatchupCompanion.API)**
- ASP.NET Core Web API (.NET 8) - Endpoints RESTful
- Entity Framework Core 8 - ORM y comunicación con base de datos
- SQL Server LocalDB - Motor de base de datos (desarrollo)
- Swagger/OpenAPI - Documentación interactiva de la API
- Integración con Data Dragon - Sincronización automática de campeones desde la API de Riot Games

**Frontend (MatchupCompanion.Client)**
- Blazor WebAssembly - SPA interactiva que se ejecuta en el navegador
- C# - Lógica del cliente escrita en C# en lugar de JavaScript
- Bootstrap 5 - Diseño UI responsivo

**Compartido (MatchupCompanion.Shared)**
- DTOs (Data Transfer Objects) compartidos entre cliente y servidor para type safety

## Características

**Backend API**
- API RESTful completa con operaciones CRUD para Campeones, Matchups, Tips y Roles
- 172 campeones sincronizados automáticamente desde Data Dragon
- Documentación interactiva con Swagger
- Arquitectura de patrón Repository con capa de servicios
- Entity Framework Core con SQL Server LocalDB

**Frontend**
- Interfaz de usuario completa con Bootstrap 5
- Búsqueda de matchups con selección de campeones y roles
- Lista de matchups con filtrado dinámico
- Vista detallada de matchup con tips categorizados
- Formularios para crear matchups y agregar tips
- Servicios HTTP para comunicación con API
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

2. Configurar API Key de Riot (opcional, solo para sincronización)
   - Copiar `appsettings.Development.json.example` a `appsettings.Development.json`
   - Obtener una API key en https://developer.riotgames.com/
   - Reemplazar `YOUR_RIOT_API_KEY_HERE` con tu key
   - Este archivo NO debe ser commiteado a Git

3. Aplicar migraciones (si es necesario)
   ```bash
   cd MatchupCompanion.API
   dotnet ef database update
   ```

4. Ejecutar el backend API
   ```bash
   cd MatchupCompanion.API
   dotnet run
   ```
   - La API estará disponible en https://localhost:7285
   - Swagger UI en https://localhost:7285

5. Sincronizar campeones (si la base de datos está vacía)
   - En Swagger, ejecutar `POST /api/RiotSync/sync-champions`
   - Esto descargará ~172 campeones actuales de League of Legends

6. Ejecutar el frontend (en otra terminal)
   ```bash
   cd MatchupCompanion.Client
   dotnet run
   ```
   - El cliente estará disponible en el puerto mostrado en consola (usualmente https://localhost:5001)
   - La API debe estar ejecutándose en https://localhost:7285

## Estructura del Proyecto

```
MatchupCompanion/
├── MatchupCompanion.API/           # Backend ASP.NET Core Web API
│   ├── Controllers/                # Endpoints de la API
│   ├── Services/                   # Lógica de negocio
│   ├── Data/                       # DbContext y Repositorios
│   ├── Models/                     # Entidades y DTOs
│   ├── ExternalServices/           # RiotApiService (Data Dragon)
│   └── Migrations/                 # Migraciones de EF Core
├── MatchupCompanion.Client/        # Frontend Blazor WebAssembly
│   ├── Pages/                      # Páginas Razor
│   ├── Services/                   # Servicios HTTP
│   └── Layout/                     # Componentes de layout
├── MatchupCompanion.Shared/        # DTOs compartidos
│   └── Models/                     # Objetos de transferencia de datos
├── ARCHITECTURE.md                 # Documentación de arquitectura
└── PROJECT-STATUS.md               # Estado actual del proyecto
```

## Mejoras Futuras

- Autenticación de usuarios con ASP.NET Core Identity
- Sistema de votación para tips
- Estadísticas avanzadas y win rates
- Funcionalidad de edición y eliminación para matchups y tips
- Imágenes de campeones desde Data Dragon
- Caching para optimización de rendimiento
- Tests unitarios y de integración

## Documentación

- [ARCHITECTURE.md](ARCHITECTURE.md) - Detalles de arquitectura y patrones
- [PROJECT-STATUS.md](PROJECT-STATUS.md) - Estado actual y próximos pasos

> **Estado Actual**: Backend funcional con sincronización de campeones desde Data Dragon de Riot Games. Base de datos SQL Server LocalDB configurada con 172 campeones sincronizados.

---

**Creado por Alejandro Burciaga Calzadillas**
