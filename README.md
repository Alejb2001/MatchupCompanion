# Matchup Companion

Sistema de gestión de estrategias de matchups para League of Legends, desarrollado con Blazor WebAssembly y ASP.NET Core Web API.

## Descripción

Matchup Companion ayuda a los jugadores de League of Legends a compartir y descubrir estrategias de matchups entre campeones. Los usuarios pueden crear guías detalladas incluyendo runas recomendadas, objetos, hechizos de invocador, orden de habilidades y consejos estratégicos para enfrentamientos específicos.

## Características Principales

### Gestión de Matchups
- Crear y compartir guías detalladas con calificación de dificultad
- Incluir runas, objetos, hechizos de invocador y orden de habilidades
- Agregar consejos categorizados por fase del juego
- Buscar y filtrar matchups por campeón, rol o dificultad

### Autenticación y Seguridad
- Autenticación basada en tokens JWT
- Control de acceso basado en roles (Admin, Usuario, Invitado)
- Endpoints protegidos con permisos específicos
- Modo invitado con acceso de solo lectura

### Integración con Datos del Juego
- Sincronización automática de datos de campeones, runas, objetos y hechizos desde la API de Riot
- Soporte en español (es_ES)
- Datos actualizados al iniciar la aplicación

## Stack Tecnológico

### Backend
- ASP.NET Core 8 Web API
- Entity Framework Core 8
- ASP.NET Core Identity
- Autenticación JWT Bearer
- SQL Server LocalDB

### Frontend
- Blazor WebAssembly (.NET 8)
- Bootstrap 5
- Blazored.LocalStorage

### Servicios Externos
- Riot Games Data Dragon API

## Inicio Rápido

### Requisitos Previos
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server LocalDB (incluido con Visual Studio)
- Navegador web moderno

### Instalación

1. **Clonar el repositorio**
```bash
git clone https://github.com/AlejandroBurworworwork/MatchupCompanion.git
cd MatchupCompanion
```

2. **Ejecutar el Backend**
```bash
cd MatchupCompanion.API
dotnet run
```
La API estará disponible en `http://localhost:5007`

3. **Ejecutar el Frontend** (nueva terminal)
```bash
cd MatchupCompanion.Client
dotnet run
```
El cliente estará disponible en `http://localhost:5173`

4. **Primera Ejecución**
   - La base de datos se crea automáticamente
   - Los datos del juego se sincronizan desde la API de Riot
   - Usuario administrador por defecto: `admin@matchup.com` / `Admin123`

## Estructura del Proyecto

```
MatchupCompanion/
├── MatchupCompanion.API/          # Backend Web API
│   ├── Controllers/               # Endpoints REST
│   ├── Services/                  # Lógica de negocio
│   ├── Data/                      # Repositorios EF Core
│   ├── Models/                    # Entidades de dominio
│   ├── ExternalServices/          # Integración con API de Riot
│   └── Migrations/                # Migraciones de base de datos
│
├── MatchupCompanion.Client/       # Blazor WebAssembly
│   ├── Pages/                     # Componentes UI
│   ├── Services/                  # Servicios HTTP
│   ├── Handlers/                  # Manejadores de requests
│   └── wwwroot/                   # Recursos estáticos
│
└── MatchupCompanion.Shared/       # Modelos compartidos
    └── Models/                    # DTOs
```

## Endpoints de la API

### Autenticación
- `POST /api/auth/register` - Crear cuenta
- `POST /api/auth/login` - Obtener token JWT
- `POST /api/auth/guest` - Sesión de invitado
- `GET /api/auth/me` - Información del usuario actual

### Matchups
- `GET /api/matchups` - Listar todos los matchups
- `GET /api/matchups/{id}` - Obtener detalles de matchup
- `GET /api/matchups/search` - Buscar matchups
- `POST /api/matchups` - Crear matchup (requiere autenticación)
- `PUT /api/matchups/{id}` - Actualizar matchup (requiere autenticación y propiedad)
- `DELETE /api/matchups/{id}` - Eliminar matchup (requiere autenticación y propiedad)

### Datos del Juego
- `/api/champions` - Datos de campeones
- `/api/roles` - Datos de roles
- `/api/runes` - Datos de runas
- `/api/items` - Datos de objetos
- `/api/summonerspells` - Datos de hechizos de invocador

## Roles y Permisos

| Acción | Invitado | Usuario | Creador | Admin |
|--------|----------|---------|---------|-------|
| Ver Matchups | Si | Si | Si | Si |
| Crear Matchup | No | Si | Si | Si |
| Editar Matchup | No | No | Si | Si |
| Eliminar Matchup | No | No | Si | Si |
| Agregar Tips | No | Si | Si | Si |

## Documentación

- [ARCHITECTURE.md](ARCHITECTURE.md) - Arquitectura técnica detallada
- [AUTENTICACION.md](AUTENTICACION.md) - Guía del sistema de autenticación
- [PROJECT-STATUS.md](PROJECT-STATUS.md) - Estado actual del proyecto
- [FRONTEND-GUIDE.md](FRONTEND-GUIDE.md) - Guía del frontend

## Licencia

Este proyecto se proporciona con fines educativos y de portafolio.

## Aviso Legal

Este proyecto no está avalado por Riot Games y no refleja las opiniones de Riot Games ni de ninguna persona involucrada oficialmente en la producción o gestión de League of Legends.

---

**Desarrollador**: Alejandro Burciaga Calzadillas
