# Matchup Companion - Architecture Documentation

## Overview

Matchup Companion is a full-stack web application designed for League of Legends players to manage and share champion matchup strategies. The system uses a modern architecture with Blazor WebAssembly for the frontend and ASP.NET Core Web API for the backend.

## Technology Stack

### Frontend
- **Blazor WebAssembly** - Client-side SPA framework
- **Bootstrap 5** - UI components and responsive design
- **Blazored.LocalStorage** - Browser storage management
- **C# / .NET 8** - Programming language and runtime

### Backend
- **ASP.NET Core 8 Web API** - RESTful API
- **Entity Framework Core** - ORM for database access
- **ASP.NET Core Identity** - Authentication and authorization
- **JWT Bearer Authentication** - Stateless authentication
- **SQL Server LocalDB** - Development database

### External Services
- **Riot Games Data Dragon API** - Champion, rune, item, and spell data

## Architecture Diagram

```
┌─────────────────────────────────────────────────────────────┐
│                  Blazor WebAssembly Client                   │
│                  (MatchupCompanion.Client)                   │
├─────────────────────────────────────────────────────────────┤
│  Pages: Login, Register, MatchupSearch, MatchupDetail       │
│         CreateMatchup, EditMatchup, AddTip                   │
│                           │                                  │
│  Services: AuthenticationService, MatchupService             │
│           ChampionService, RoleService, RuneService          │
│           ItemService, SummonerSpellService                  │
│                           │                                  │
│  Handlers: AuthorizationMessageHandler (JWT injection)      │
└──────────────────────────┬──────────────────────────────────┘
                           │ HTTP/HTTPS + JWT
                           ▼
┌─────────────────────────────────────────────────────────────┐
│                  ASP.NET Core Web API                        │
│                  (MatchupCompanion.API)                      │
├─────────────────────────────────────────────────────────────┤
│  Controllers: AuthController, MatchupsController             │
│              ChampionsController, RolesController            │
│              RunesController, ItemsController                │
│              SummonerSpellsController, RiotSyncController    │
│                           │                                  │
│  Services: AuthService, MatchupService                       │
│           ChampionService, RiotApiService                    │
│                           │                                  │
│  Repositories: MatchupRepository, ChampionRepository         │
│               RoleRepository, RuneRepository                 │
│               ItemRepository, SummonerSpellRepository        │
│                           │                                  │
│  Data: ApplicationDbContext (EF Core)                        │
└──────────────────────────┬──────────────────────────────────┘
                           │
                           ▼
                  ┌──────────────────┐
                  │   SQL Server     │
                  │   LocalDB        │
                  └──────────────────┘
```

## Authentication & Authorization

### Authentication Flow

1. **User Registration/Login**
   - User submits credentials to `/api/auth/login` or `/api/auth/register`
   - Backend validates credentials and generates JWT token
   - Token is returned with user information

2. **Token Storage**
   - Frontend stores token in browser's LocalStorage
   - Token includes claims: UserId, Username, Email, DisplayName, IsGuest, Roles

3. **Request Authentication**
   - `AuthorizationMessageHandler` intercepts all HTTP requests
   - Automatically adds `Authorization: Bearer <token>` header
   - Backend validates token signature, issuer, audience, and expiration

4. **Session Types**
   - **Regular Users**: Full access (create, edit, delete own matchups)
   - **Guest Users**: Read-only access (24-hour expiration)
   - **Admin Users**: Full access to all matchups

### Authorization Rules

| Resource | Guest | User | Creator | Admin |
|----------|-------|------|---------|-------|
| View Matchups | ✅ | ✅ | ✅ | ✅ |
| Create Matchup | ❌ | ✅ | ✅ | ✅ |
| Edit Matchup | ❌ | ❌ | ✅ | ✅ |
| Delete Matchup | ❌ | ❌ | ✅ | ✅ |
| Add Tip | ❌ | ✅ | ✅ | ✅ |

### JWT Configuration

- **Secret Key**: Configured in appsettings.json
- **Issuer**: `MatchupCompanionAPI`
- **Audience**: `MatchupCompanionClient`
- **Expiration**: 60 minutes (configurable)
- **Validation**: Signature, issuer, audience, and lifetime

## Data Models

### Core Entities

**Matchup**
- Represents a champion vs champion matchup in a specific role
- Contains difficulty rating, general advice, and detailed strategy
- Includes recommended runes, items, summoner spells, and ability order
- Tracks creator (CreatedById) and timestamps

**MatchupTip**
- Specific advice categorized by game phase or topic
- Linked to a matchup
- Includes priority rating and author information

**Champion**
- League of Legends champion data from Riot API
- Includes name, title, image, description, and primary role

**Rune**
- Rune data organized by trees (Precision, Domination, etc.)
- Includes keystones and secondary runes

**Item**
- Item data with stats, cost, and build paths

**SummonerSpell**
- Summoner spell data with cooldowns and effects

### Authentication Entities

**ApplicationUser** (extends IdentityUser)
- User account with display name
- Guest user tracking (IsGuest, GuestExpiresAt)
- Linked to created matchups

## API Endpoints

### Authentication (`/api/auth`)
- `POST /register` - Create new user account
- `POST /login` - Authenticate and get JWT token
- `POST /guest` - Create temporary guest session
- `POST /refresh` - Refresh expired token
- `GET /me` - Get current user information
- `GET /validate` - Validate token
- `POST /logout` - Logout (client-side)

### Matchups (`/api/matchups`)
- `GET /` - Get all matchups
- `GET /{id}` - Get matchup by ID
- `GET /search` - Search matchup by champions and role
- `POST /` - Create new matchup (auth required)
- `PUT /{id}` - Update matchup (auth + ownership required)
- `DELETE /{id}` - Delete matchup (auth + ownership required)
- `POST /tips` - Add tip to matchup (auth required)

### Game Data
- `/api/champions` - Champion data
- `/api/roles` - Role data
- `/api/runes` - Rune and rune tree data
- `/api/items` - Item data
- `/api/summonerspells` - Summoner spell data
- `/api/riotsync` - Trigger manual data synchronization

## Key Features

### 1. Matchup Management
- Create, read, update, delete (CRUD) operations
- Rich matchup details: runes, items, spells, ability order, strategy
- Permission-based access control
- Search by champion and role

### 2. Authentication System
- JWT-based stateless authentication
- Role-based authorization (Admin, User, Guest)
- Automatic token injection via HTTP handler
- Secure password requirements

### 3. Data Synchronization
- Automatic sync from Riot Data Dragon on startup
- Manual sync available via API endpoint
- Spanish language support for game data

### 4. User Experience
- Responsive Bootstrap UI
- Real-time validation
- Loading states and error handling
- Confirmation modals for destructive actions

## Security Considerations

### Implemented
- ✅ JWT signature validation
- ✅ Token expiration checking
- ✅ HTTPS support (configurable)
- ✅ Password complexity requirements
- ✅ Role-based authorization
- ✅ Guest user restrictions
- ✅ CORS policy (configurable)

### Production Recommendations
- Change JWT secret key
- Enable HTTPS enforcement
- Configure restrictive CORS policy
- Add rate limiting
- Implement refresh token rotation
- Add request logging
- Enable email confirmation

## Database Schema

### Identity Tables (ASP.NET Core Identity)
- AspNetUsers - User accounts
- AspNetRoles - User roles
- AspNetUserRoles - User-role relationships
- AspNetUserClaims, AspNetUserLogins, AspNetUserTokens

### Application Tables
- Champions - Champion data
- Roles - Lane role data
- Runes - Rune data
- RuneTrees - Rune tree data
- Items - Item data
- SummonerSpells - Summoner spell data
- Matchups - Matchup strategies
- MatchupTips - Matchup-specific tips

## Development Setup

### Prerequisites
- .NET 8 SDK
- Visual Studio 2022 or VS Code
- SQL Server LocalDB

### Running the Application

1. **Backend**
```bash
cd MatchupCompanion.API
dotnet run
```
API will be available at `http://localhost:5007`

2. **Frontend**
```bash
cd MatchupCompanion.Client
dotnet run
```
Client will be available at `http://localhost:5173`

### First Run
- Database is created automatically
- Riot data is synchronized on first startup
- Default admin user: `admin@matchup.com` / `Admin123`

## Project Structure

```
MatchupCompanion/
├── MatchupCompanion.API/          # Backend Web API
│   ├── Controllers/               # API endpoints
│   ├── Services/                  # Business logic
│   ├── Data/                      # EF Core context and repositories
│   ├── Models/                    # Entities and DTOs
│   ├── ExternalServices/          # Riot API integration
│   └── Migrations/                # EF Core migrations
├── MatchupCompanion.Client/       # Blazor WebAssembly frontend
│   ├── Pages/                     # Razor pages
│   ├── Services/                  # HTTP client services
│   ├── Handlers/                  # HTTP message handlers
│   └── wwwroot/                   # Static assets
└── MatchupCompanion.Shared/       # Shared DTOs and models
    └── Models/                    # Data transfer objects
```

## Future Enhancements

- Real-time collaborative editing
- Matchup voting and ratings
- User profiles and statistics
- Image uploads for strategies
- Video guide integration
- Mobile app support
- Advanced search and filtering
- Email notifications
- Social sharing features
