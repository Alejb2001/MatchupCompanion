# Matchup Companion

A comprehensive League of Legends matchup strategy management system built with Blazor WebAssembly and ASP.NET Core Web API.

## Overview

Matchup Companion helps League of Legends players share and discover champion matchup strategies. Users can create detailed guides including recommended runes, items, summoner spells, ability orders, and strategic tips for specific matchups.

**Current Status**: Fully functional application with authentication, authorization, CRUD operations, and Riot Games data integration.

## Key Features

### 🎮 Matchup Management
- **Create & Share**: Detailed matchup guides with difficulty ratings
- **Rich Content**: Include runes, items, summoner spells, and ability order
- **Strategic Tips**: Add categorized tips for different game phases
- **Search & Filter**: Find matchups by champion, role, or difficulty

### 🔐 Authentication & Security
- **JWT Authentication**: Secure token-based authentication
- **Role-Based Access**: Admin, User, and Guest roles
- **Protected Endpoints**: Permission-based resource access
- **Guest Mode**: Read-only access without registration

### 📊 Game Data Integration
- **Automatic Sync**: Champion, rune, item, and spell data from Riot API
- **Spanish Support**: Game data in Spanish (es_ES)
- **Up-to-Date**: Synchronized on application startup

### 💻 Modern Architecture
- **Frontend**: Blazor WebAssembly with Bootstrap 5
- **Backend**: ASP.NET Core 8 Web API
- **Database**: Entity Framework Core + SQL Server
- **Authentication**: ASP.NET Core Identity + JWT

## Technology Stack

### Backend
- ASP.NET Core 8 Web API
- Entity Framework Core 8
- ASP.NET Core Identity
- JWT Bearer Authentication
- SQL Server LocalDB
- Swagger/OpenAPI

### Frontend
- Blazor WebAssembly (.NET 8)
- Bootstrap 5
- Blazored.LocalStorage
- Custom HTTP Message Handlers

### External Services
- Riot Games Data Dragon API

## Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server LocalDB (included with Visual Studio)
- Modern web browser

### Installation

1. **Clone the repository**
```bash
git clone https://github.com/yourusername/MatchupCompanion.git
cd MatchupCompanion/MatchupCompanion
```

2. **Run the Backend**
```bash
cd MatchupCompanion.API
dotnet run
```
The API will start at `http://localhost:5007`

3. **Run the Frontend** (new terminal)
```bash
cd MatchupCompanion.Client
dotnet run
```
The client will start at `http://localhost:5173`

4. **First Run**
   - Database is created automatically
   - Game data synchronizes from Riot API
   - Default admin account: `admin@matchup.com` / `Admin123`

### Quick Start Guide

1. **Browse**: View matchups without authentication
2. **Register**: Create an account at `/register` to contribute
3. **Login**: Access full features with your account
4. **Create**: Share your matchup strategies
5. **Manage**: Edit or delete your own matchups

## Project Structure

```
MatchupCompanion/
├── MatchupCompanion.API/          # Backend Web API
│   ├── Controllers/               # REST endpoints
│   ├── Services/                  # Business logic
│   ├── Data/                      # EF Core repositories
│   ├── Models/                    # Domain entities
│   ├── ExternalServices/          # Riot API integration
│   └── Migrations/                # Database migrations
│
├── MatchupCompanion.Client/       # Blazor WebAssembly
│   ├── Pages/                     # UI components
│   ├── Services/                  # HTTP services
│   ├── Handlers/                  # Request handlers
│   └── wwwroot/                   # Static assets
│
├── MatchupCompanion.Shared/       # Shared models
│   └── Models/                    # DTOs
│
└── ARCHITECTURE.md                # Technical documentation
```

## API Overview

### Authentication
- `POST /api/auth/register` - Create account
- `POST /api/auth/login` - Get JWT token
- `POST /api/auth/guest` - Guest session
- `GET /api/auth/me` - Current user info

### Matchups
- `GET /api/matchups` - List all matchups
- `GET /api/matchups/{id}` - Get matchup details
- `GET /api/matchups/search` - Search matchups
- `POST /api/matchups` - Create matchup (auth required)
- `PUT /api/matchups/{id}` - Update matchup (auth + ownership)
- `DELETE /api/matchups/{id}` - Delete matchup (auth + ownership)

### Game Data
- `/api/champions` - Champion data
- `/api/roles` - Role data
- `/api/runes` - Rune data
- `/api/items` - Item data
- `/api/summonerspells` - Summoner spell data

## Features in Detail

### User Roles & Permissions

| Action | Guest | User | Creator | Admin |
|--------|-------|------|---------|-------|
| View Matchups | ✅ | ✅ | ✅ | ✅ |
| Create Matchup | ❌ | ✅ | ✅ | ✅ |
| Edit Matchup | ❌ | ❌ | ✅ | ✅ |
| Delete Matchup | ❌ | ❌ | ✅ | ✅ |
| Add Tips | ❌ | ✅ | ✅ | ✅ |

### Security Features
- Password requirements (6+ chars, upper/lower/numbers)
- JWT token expiration (60 minutes, configurable)
- Automatic token injection in requests
- Protected API endpoints
- Guest session expiration (24 hours)

## Configuration

### JWT Settings (`appsettings.json`)
```json
{
  "Jwt": {
    "SecretKey": "your-secret-key",
    "Issuer": "MatchupCompanionAPI",
    "Audience": "MatchupCompanionClient",
    "ExpirationInMinutes": 60
  }
}
```

### Database Connection
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=MatchupCompanionDb;Trusted_Connection=true"
  }
}
```

## Development

### Building
```bash
dotnet build
```

### Running Tests
```bash
dotnet test
```

### Database Migrations
```bash
cd MatchupCompanion.API
dotnet ef migrations add MigrationName
dotnet ef database update
```

### Swagger Documentation
When running the API, access interactive documentation at:
`http://localhost:5007/swagger`

## Documentation

- [ARCHITECTURE.md](ARCHITECTURE.md) - Detailed technical architecture
- API documentation available via Swagger UI

## Future Enhancements

- Matchup voting and rating system
- User profiles and statistics
- Image uploads for strategies
- Video guide integration
- Advanced search and filtering
- Email notifications
- Mobile app support
- Social sharing features

## License

This project is provided for educational and portfolio purposes.

## Acknowledgments

- **Riot Games** for the Data Dragon API
- **Microsoft** for .NET and Blazor
- **Bootstrap** for UI components

> **Estado Actual**: Backend funcional con sincronización de campeones desde Data Dragon de Riot Games. Base de datos SQL Server LocalDB configurada con 172 campeones sincronizados.

---

**Note**: This project is not endorsed by Riot Games and doesn't reflect the views or opinions of Riot Games or anyone officially involved in producing or managing League of Legends.

**Developer**: Alejandro Burciaga Calzadillas
