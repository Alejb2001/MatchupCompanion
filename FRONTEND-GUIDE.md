# Guía del Frontend - Matchup Companion

## Resumen

Esta guía documenta la implementación completa del frontend Blazor WebAssembly para Matchup Companion.

**Última actualización**: 19 de Enero, 2026

## Arquitectura del Frontend

### Estructura de Archivos

```
MatchupCompanion.Client/
├── Services/                       # Servicios para comunicación con API
│   ├── IChampionService.cs        # Interface para ChampionService
│   ├── ChampionService.cs         # Servicio de campeones
│   ├── IRoleService.cs            # Interface para RoleService
│   ├── RoleService.cs             # Servicio de roles
│   ├── IMatchupService.cs         # Interface para MatchupService
│   ├── MatchupService.cs          # Servicio de matchups y tips
│   ├── IRuneService.cs            # Interface para RuneService
│   ├── RuneService.cs             # Servicio de runas
│   ├── IItemService.cs            # Interface para ItemService
│   └── ItemService.cs             # Servicio de items
├── Pages/                          # Páginas Razor Components
│   ├── Home.razor                 # Página principal
│   ├── MatchupsList.razor         # Lista de todos los matchups
│   ├── MatchupSearch.razor        # Búsqueda de matchup específico
│   ├── MatchupDetail.razor        # Vista detallada de matchup
│   ├── CreateMatchup.razor        # Crear nuevo matchup
│   ├── EditMatchup.razor          # Editar matchup (con autocompletado)
│   └── AddTip.razor               # Agregar tip a matchup
├── Layout/                         # Componentes de layout
│   ├── MainLayout.razor           # Layout principal
│   └── NavMenu.razor              # Menú de navegación
├── wwwroot/                        # Archivos estáticos
│   ├── css/
│   │   ├── bootstrap/             # Bootstrap 5
│   │   └── app.css                # Estilos personalizados
│   └── index.html                 # HTML principal
├── Program.cs                      # Punto de entrada y configuración
├── _Imports.razor                 # Directivas using globales
└── App.razor                      # Componente raíz

MatchupCompanion.Shared/
└── Models/                         # DTOs compartidos con API
    ├── ChampionDto.cs
    ├── RoleDto.cs
    ├── MatchupDto.cs
    ├── MatchupTipDto.cs
    ├── CreateMatchupDto.cs
    ├── UpdateMatchupDto.cs
    ├── CreateMatchupTipDto.cs
    ├── RuneDto.cs
    └── ItemDto.cs
```

## Servicios HTTP

### 1. ChampionService

**Ubicación**: `Services/ChampionService.cs`

**Funcionalidades**:
- `GetAllChampionsAsync()`: Obtiene todos los campeones
- `GetChampionByIdAsync(int id)`: Obtiene un campeón por ID

**Características**:
- Manejo de errores con try-catch
- Retorna lista vacía en caso de error
- Usa `System.Net.Http.Json` para deserialización

### 2. RoleService

**Ubicación**: `Services/RoleService.cs`

**Funcionalidades**:
- `GetAllRolesAsync()`: Obtiene todos los roles
- `GetRoleByIdAsync(int id)`: Obtiene un rol por ID

**Características**:
- Similar a ChampionService
- Manejo consistente de errores

### 3. MatchupService

**Ubicación**: `Services/MatchupService.cs`

**Funcionalidades**:
- `GetAllMatchupsAsync()`: Obtiene todos los matchups
- `GetMatchupByIdAsync(int id)`: Obtiene matchup por ID
- `SearchMatchupAsync(int, int, int)`: Busca matchup específico
- `CreateMatchupAsync(CreateMatchupDto)`: Crea nuevo matchup
- `UpdateMatchupAsync(UpdateMatchupDto)`: Actualiza matchup existente
- `AddTipAsync(CreateMatchupTipDto)`: Agrega tip a matchup

**Características**:
- Manejo de código HTTP 404 en búsqueda
- Lanza excepciones en operaciones de escritura
- Logging en consola para debugging

### 4. RuneService

**Ubicación**: `Services/RuneService.cs`

**Funcionalidades**:
- `GetAllRunesAsync()`: Obtiene todas las runas

### 5. ItemService

**Ubicación**: `Services/ItemService.cs`

**Funcionalidades**:
- `GetAllItemsAsync()`: Obtiene todos los items

## Páginas

### 1. Home.razor (`/`)

**Propósito**: Página de bienvenida e información del proyecto

**Contenido**:
- Descripción del proyecto
- Cards con enlaces a funcionalidades principales
- Lista de características
- Diseño con Bootstrap 5

**Rutas de Navegación**:
- `/matchup-search` - Buscar matchup
- `/matchups` - Ver todos los matchups
- `/create-matchup` - Crear matchup

### 2. MatchupSearch.razor (`/matchup-search`)

**Propósito**: Buscar matchup por campeones y rol

**Funcionalidades**:
- Dropdowns para seleccionar campeones y rol
- Validación de selección
- Búsqueda con loading state
- Muestra resultado o sugerencia para crear

**Estados**:
- `isLoadingData`: Cargando campeones y roles
- `isSearching`: Buscando matchup
- `searchCompleted`: Búsqueda completada
- `errorMessage`: Mensaje de error si hay

**Validaciones**:
- Campos requeridos
- Campeones no pueden ser iguales

### 3. MatchupsList.razor (`/matchups`)

**Propósito**: Listar todos los matchups disponibles

**Funcionalidades**:
- Tabla con todos los matchups
- Filtrado en tiempo real por nombre
- Badges de dificultad con colores
- Navegación a detalles

**Características**:
- Filtrado reactivo con `@bind:event="oninput"`
- Contador de matchups filtrados
- Loading spinner
- Tabla responsive

**Colores de Dificultad**:
- Easy: Verde (bg-success)
- Medium: Amarillo (bg-warning)
- Hard: Rojo (bg-danger)
- Extreme: Negro (bg-dark)

### 4. MatchupDetail.razor (`/matchup/{id}`)

**Propósito**: Vista detallada de un matchup con tips

**Funcionalidades**:
- Información completa del matchup
- Tips organizados por categoría en acordeón
- Botón para agregar tip
- Navegación de vuelta

**Características**:
- Tips agrupados por categoría
- Ordenados por prioridad descendente
- Bootstrap Accordion para organización
- Información de autor si disponible

**Categorías de Tips**:
- EarlyGame → "Early Game"
- MidGame → "Mid Game"
- LateGame → "Late Game"
- Items → "Items"
- Runes → "Runas"
- Abilities → "Habilidades"
- General → "General"

### 5. CreateMatchup.razor (`/create-matchup`)

**Propósito**: Formulario para crear nuevo matchup

**Funcionalidades**:
- Formulario con validación
- Selección de campeones y rol
- Selección de dificultad
- Campo opcional de consejo general
- Redirección automática tras crear

**Validaciones** (vía DataAnnotations):
- PlayerChampionId: Requerido
- EnemyChampionId: Requerido
- RoleId: Requerido
- Difficulty: Requerido, debe ser Easy|Medium|Hard|Extreme
- GeneralAdvice: Máximo 1000 caracteres

**Estados**:
- `isLoadingData`: Cargando datos
- `isSubmitting`: Enviando formulario
- `successMessage`: Mensaje de éxito
- `errorMessage`: Mensaje de error

### 6. EditMatchup.razor (`/edit-matchup/{id}`)

**Propósito**: Formulario para editar matchup con búsqueda por autocompletado

**Funcionalidades**:
- Campos de texto con autocompletado para campeones (no dropdowns)
- Campo de texto con autocompletado para búsqueda de items
- Selección de items por categoría con botones (+Ini, +Core, +Sit)
- Badges mostrando items seleccionados con botón de eliminar
- Dropdown solo para rol (5 opciones fijas)
- Campo de notas de estrategia
- Validación de formulario
- Redirección después de guardar

**Patrón de autocompletado**:
```razor
<input type="text" @bind="searchText" @bind:event="oninput" />
@if (filteredList.Any())
{
    <ul class="list-group">
        @foreach (var item in filteredList.Take(10))
        {
            <li @onclick="@(() => SelectItem(item))">@item.Name</li>
        }
    </ul>
}
```

**Importante - Sintaxis Razor para onclick con strings**:
```razor
<!-- CORRECTO -->
@onclick="@(() => AddItemToList(item.Id, "starting"))"

<!-- INCORRECTO - causa error de compilación -->
@onclick="AddItemToList(item.Id, \"starting\")"
```

**Estados**:
- `playerSearchText`, `enemySearchText`, `itemSearchText`: Texto de búsqueda
- `filteredChampionsForPlayer`, `filteredChampionsForEnemy`, `filteredItems`: Listas filtradas
- `startingItemIds`, `coreItemIds`, `situationalItemIds`: IDs de items seleccionados

### 7. AddTip.razor (`/add-tip/{matchupId}`)

**Propósito**: Agregar tip a matchup existente

**Funcionalidades**:
- Formulario para agregar tip
- Categorías predefinidas
- Sistema de prioridad (1-10)
- Campo opcional de autor
- Muestra información del matchup

**Validaciones** (vía DataAnnotations):
- Category: Requerida, valores específicos
- Content: Requerido, 10-2000 caracteres
- Priority: 1-10
- AuthorName: Máximo 100 caracteres

**Características**:
- Contador de caracteres en tiempo real
- Textarea con 6 filas
- Range input para prioridad
- Redirección a detalles tras crear

## Configuración

### Program.cs

```csharp
// HttpClient configurado con URL del API (HTTP, no HTTPS para evitar problemas de certificado)
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("http://localhost:5007/")
});

// Registro de servicios
builder.Services.AddScoped<IChampionService, ChampionService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IMatchupService, MatchupService>();
builder.Services.AddScoped<IRuneService, RuneService>();
builder.Services.AddScoped<IItemService, ItemService>();
```

**Importante**:
- API debe correr en `http://localhost:5007`
- Se usa HTTP en lugar de HTTPS para evitar problemas de certificado SSL en desarrollo
- Si cambias el puerto, actualiza aquí

### _Imports.razor

Directivas globales agregadas:
```csharp
@using MatchupCompanion.Client.Services
@using MatchupCompanion.Shared.Models
```

### MatchupCompanion.Client.csproj

Referencia al proyecto compartido:
```xml
<ItemGroup>
  <ProjectReference Include="..\MatchupCompanion.Shared\MatchupCompanion.Shared.csproj" />
</ItemGroup>
```

## DTOs Compartidos

Ubicados en `MatchupCompanion.Shared/Models/`

### ChampionDto
- Id, RiotChampionId, Name, Title
- ImageUrl, Description
- PrimaryRoleId, PrimaryRoleName

### RoleDto
- Id, Name, Description

### MatchupDto
- Id
- PlayerChampion (ChampionDto)
- EnemyChampion (ChampionDto)
- Role (RoleDto)
- Difficulty, GeneralAdvice
- Tips (List<MatchupTipDto>)
- CreatedAt, UpdatedAt

### MatchupTipDto
- Id, MatchupId
- Category, Content
- Priority, AuthorName
- CreatedAt

### CreateMatchupDto
- PlayerChampionId, EnemyChampionId, RoleId
- Difficulty, GeneralAdvice
- Validaciones con DataAnnotations

### UpdateMatchupDto
- Id, PlayerChampionId, EnemyChampionId, RoleId
- Difficulty, GeneralAdvice, StrategyNotes
- StartingItems, CoreItems, SituationalItems (listas de strings con RiotItemId)
- RecommendedRunes (lista de strings con RiotRuneId)
- Validaciones con DataAnnotations

### CreateMatchupTipDto
- MatchupId, Category, Content
- Priority (default: 5)
- AuthorName (opcional)
- Validaciones con DataAnnotations

### RuneDto
- Id, RiotRuneId, Name, Description
- ImageUrl, Slot, Tree

### ItemDto
- Id, RiotItemId, Name, Description
- ImageUrl, Gold

## Patrones y Buenas Prácticas

### 1. Manejo de Estados
```csharp
private bool isLoading = true;
private string? errorMessage;

protected override async Task OnInitializedAsync()
{
    await LoadData();
}

private async Task LoadData()
{
    isLoading = true;
    errorMessage = null;

    try
    {
        // Lógica
    }
    catch (Exception ex)
    {
        errorMessage = $"Error: {ex.Message}";
    }
    finally
    {
        isLoading = false;
    }
}
```

### 2. Validación de Formularios
```razor
<EditForm Model="@dto" OnValidSubmit="@HandleSubmit">
    <DataAnnotationsValidator />

    <InputText @bind-Value="dto.Property" />
    <ValidationMessage For="@(() => dto.Property)" />

    <button type="submit" disabled="@isSubmitting">
        Submit
    </button>
</EditForm>
```

### 3. Navegación Programática
```csharp
@inject NavigationManager Navigation

private void Navigate()
{
    Navigation.NavigateTo($"/matchup/{id}");
}
```

### 4. Loading States
```razor
@if (isLoading)
{
    <div class="spinner-border"></div>
}
else if (errorMessage != null)
{
    <div class="alert alert-danger">@errorMessage</div>
}
else
{
    <!-- Contenido -->
}
```

## Estilos y Diseño

### Bootstrap 5
- Grids responsive
- Cards para información
- Badges para categorías y dificultad
- Tables para listas
- Forms con validación
- Alerts para mensajes
- Spinners para loading

### Colores Consistentes
- Primary: Botones principales
- Success: Easy, éxito
- Warning: Medium
- Danger: Hard, errores
- Dark: Extreme
- Info: Tips count
- Secondary: Roles

## API Endpoints Utilizados

```
GET  /api/Champions                                          → Lista de campeones
GET  /api/Roles                                              → Lista de roles
GET  /api/Runes                                              → Lista de runas
GET  /api/Items                                              → Lista de items
GET  /api/Matchups                                           → Lista de matchups
GET  /api/Matchups/{id}                                      → Matchup por ID
GET  /api/Matchups/search?playerChampionId=&enemyChampionId=&roleId= → Búsqueda
POST /api/Matchups                                           → Crear matchup
PUT  /api/Matchups/{id}                                      → Actualizar matchup
POST /api/MatchupTips                                        → Agregar tip
```

## Ejecución

### 1. Iniciar API
```bash
cd MatchupCompanion.API
dotnet run
```

### 2. Iniciar Cliente
```bash
cd MatchupCompanion.Client
dotnet run
```

### 3. Navegar
- Abrir browser en la URL indicada (ej: https://localhost:5001)
- El API debe estar corriendo

## Troubleshooting

### Error CORS
- Verificar que API tiene CORS configurado
- Verificar URL en Program.cs del cliente
- **Usar HTTP en lugar de HTTPS** para evitar problemas de certificado SSL

### Error 404 en llamadas
- Verificar que API esté corriendo
- Verificar BaseAddress en Program.cs (debe ser `http://localhost:5007/`)
- Verificar que endpoints existan

### Validación no funciona
- Verificar DataAnnotations en DTOs
- Verificar DataAnnotationsValidator en formulario
- Verificar ValidationMessage components

### Datos no cargan
- Revisar consola del browser (F12)
- Verificar que API responda (Swagger en http://localhost:5007)
- Verificar servicios registrados en Program.cs

### Error de compilación con comillas en Razor
- Usar `@(() => Method("string"))` en lugar de `Method(\"string\")`
- Las comillas escapadas dentro de atributos Razor causan errores

## Próximas Mejoras

1. **Paginación**: Para lista de matchups
2. **Ordenamiento**: Por diferentes campos
3. **Imágenes**: Mostrar avatares de campeones
4. **Caché**: Para reducir llamadas al API
5. **Offline Support**: PWA features
6. **Búsqueda Avanzada**: Más filtros
7. **Edición**: Editar matchups y tips
8. **Eliminación**: Eliminar con confirmación
9. **Autenticación**: JWT tokens
10. **Favoritos**: Guardar matchups favoritos

## Conclusión

El frontend Blazor WebAssembly está completamente implementado con todas las funcionalidades básicas necesarias para gestionar matchups de League of Legends. La arquitectura es escalable y sigue las mejores prácticas de Blazor y ASP.NET Core.
