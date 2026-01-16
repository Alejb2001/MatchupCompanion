# Matchup Companion

Un proyecto web full-stack construido con **.NET 8** que permite a los jugadores de League of Legends consultar, crear y compartir estrategias para enfrentamientos (matchups) específicos.

Este proyecto fue creado para demostrar habilidades en el ecosistema .NET, incluyendo **ASP.NET Core Web API**, **Blazor WebAssembly** y **Entity Framework Core**.

> **Estado Actual**: Backend funcional con sincronización de campeones desde Data Dragon de Riot Games. Base de datos SQL Server LocalDB configurada con 172 campeones sincronizados.

---

## 📖 Descripción del Proyecto

Esta aplicación web resuelve un problema común para los jugadores de LoL: "¿Cómo juego este matchup?". La aplicación permite a los usuarios:

1.  **Seleccionar** dos campeones (tu campeón y el campeón enemigo).
2.  **Ver** instantáneamente los consejos, nivel de dificultad y estrategias enviadas por otros usuarios para ese enfrentamiento.
3.  **Contribuir** añadiendo sus propios consejos para un matchup que aún no existe o que quieran complementar.

Todo esto se gestiona a través de una interfaz de usuario reactiva construida con Blazor que consume una API de backend de ASP.NET Core.

---

## 🛠️ Stack de Tecnologías

Este proyecto utiliza una arquitectura de Aplicación Blazor WebAssembly Hospedada en ASP.NET Core, lo que permite un desarrollo full-stack cohesivo.

### Backend (`.API`)
* **ASP.NET Core Web API (.NET 8)**: Para construir los endpoints RESTful que gestionan los datos.
* **Entity Framework Core 8**: Para el ORM (mapeo objeto-relacional) y la comunicación con la base de datos.
* **SQL Server LocalDB**: Como motor de la base de datos (desarrollo).
* **Swagger/OpenAPI**: Documentación interactiva de la API.
* **Integración con Data Dragon**: Sincronización automática de campeones desde la API de Riot Games.

### Frontend (`.Client`)
* **Blazor WebAssembly**: Para construir una SPA (Single Page Application) interactiva y de alto rendimiento que se ejecuta en el navegador.
* **C#**: Lógica del cliente escrita en C# en lugar de JavaScript.
* **CSS / Bootstrap**: Para el diseño y la interfaz responsiva.

### Compartido (`.Shared`)
* Modelos de datos y DTOs (Data Transfer Objects) compartidos entre el cliente y el servidor para asegurar consistencia.

---

## ✨ Características Implementadas

### ✅ Backend API
* **API RESTful Completa**: Endpoints para operaciones CRUD sobre Campeones, Matchups, Tips y Roles.
* **Sincronización con Riot Games**: 172 campeones sincronizados automáticamente desde Data Dragon.
* **Documentación Swagger**: Interfaz interactiva para probar todos los endpoints.
* **Persistencia de Datos**: Entity Framework Core con SQL Server LocalDB.
* **Arquitectura en Capas**: Repositorios, Servicios y Controladores bien separados.

### 🚧 Frontend (Pendiente)
* Interfaz de usuario con Blazor WebAssembly.
* Componentes reactivos para selección de campeones.
* Visualización de matchups y consejos.

---

## 🚀 Inicio Rápido

### Prerrequisitos
- .NET 8 SDK
- SQL Server LocalDB (incluido con Visual Studio)
- Visual Studio 2022 o VS Code

### Configuración

1. **Clonar el repositorio**
   ```bash
   git clone <repository-url>
   cd MatchupCompanion
   ```

2. **Configurar la API Key de Riot** (Opcional, solo para sincronización)
   - Copia `appsettings.Development.json.example` a `appsettings.Development.json`
   - Obtén una API key en: https://developer.riotgames.com/
   - Reemplaza `YOUR_RIOT_API_KEY_HERE` con tu key
   - **IMPORTANTE**: Este archivo NO debe subirse a Git

3. **Aplicar migraciones** (si es necesario)
   ```bash
   cd MatchupCompanion.API
   dotnet ef database update
   ```

4. **Ejecutar la aplicación**
   ```bash
   dotnet run
   ```

5. **Acceder a Swagger**
   - HTTP: http://localhost:5007
   - HTTPS: https://localhost:7285

6. **Sincronizar campeones**
   - En Swagger, ejecuta `POST /api/RiotSync/sync-champions`
   - Esto descargará los ~172 campeones actuales de League of Legends

---

## 📈 Mejoras Futuras

* **Autenticación de Usuarios**: Implementar ASP.NET Core Identity para que los usuarios se registren y puedan editar/eliminar sus propios consejos.
* **Sistema de Votación**: Permitir a los usuarios votar los consejos más útiles.
* **Estadísticas Avanzadas**: Calcular win rates y estadísticas de matchups.
* **Frontend Blazor**: Completar la interfaz de usuario.
* **Caching**: Implementar cache para mejorar rendimiento.
* **Tests Unitarios**: Agregar cobertura de pruebas.

---

## 📂 Estructura del Proyecto

```
MatchupCompanion/
├── MatchupCompanion.API/           # Backend ASP.NET Core Web API
│   ├── Controllers/                # Endpoints de la API
│   ├── Services/                   # Lógica de negocio
│   ├── Data/                       # DbContext y Repositorios
│   │   └── Repositories/
│   ├── Models/                     # Entidades y DTOs
│   ├── ExternalServices/           # RiotApiService (Data Dragon)
│   └── Migrations/                 # Migraciones de EF Core
├── ARCHITECTURE.md                 # Documentación de arquitectura
├── GETTING-STARTED.md             # Guía de inicio
└── PROJECT-STATUS.md              # Estado actual del proyecto
```

---

## 📚 Documentación Adicional

- [ARCHITECTURE.md](ARCHITECTURE.md) - Detalles de arquitectura y patrones utilizados
- [GETTING-STARTED.md](GETTING-STARTED.md) - Guía detallada de configuración
- [PROJECT-STATUS.md](PROJECT-STATUS.md) - Estado actual y próximos pasos

---

**Proyecto creado por Alejandro Burciaga Calzadillas**
