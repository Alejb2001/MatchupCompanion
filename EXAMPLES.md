# Ejemplos Prácticos de Uso - Matchup Companion API

## Índice
1. [Configuración Inicial](#configuración-inicial)
2. [Endpoints de Campeones](#endpoints-de-campeones)
3. [Endpoints de Matchups](#endpoints-de-matchups)
4. [Endpoints de Roles](#endpoints-de-roles)
5. [Sincronización con Riot API](#sincronización-con-riot-api)
6. [Escenarios Completos](#escenarios-completos)

---

## Configuración Inicial

### Base URL
```
https://localhost:7xxx/api
```

### Headers Comunes
```
Content-Type: application/json
Accept: application/json
```

---

## Endpoints de Campeones

### 1. Obtener Todos los Campeones

**Request:**
```http
GET /api/champions
```

**Response (200 OK):**
```json
[
  {
    "id": 1,
    "riotChampionId": "266",
    "name": "Aatrox",
    "title": "the Darkin Blade",
    "imageUrl": "https://ddragon.leagueoflegends.com/cdn/14.1.1/img/champion/Aatrox.png",
    "description": "Once honored defenders...",
    "primaryRoleId": 1,
    "primaryRoleName": "Top"
  },
  {
    "id": 2,
    "riotChampionId": "103",
    "name": "Ahri",
    "title": "the Nine-Tailed Fox",
    "imageUrl": "https://ddragon.leagueoflegends.com/cdn/14.1.1/img/champion/Ahri.png",
    "description": "Innately connected to the latent power...",
    "primaryRoleId": 3,
    "primaryRoleName": "Mid"
  }
]
```

### 2. Obtener un Campeón por ID

**Request:**
```http
GET /api/champions/2
```

**Response (200 OK):**
```json
{
  "id": 2,
  "riotChampionId": "103",
  "name": "Ahri",
  "title": "the Nine-Tailed Fox",
  "imageUrl": "https://ddragon.leagueoflegends.com/cdn/14.1.1/img/champion/Ahri.png",
  "description": "Innately connected to the latent power of Ionia...",
  "primaryRoleId": 3,
  "primaryRoleName": "Mid"
}
```

**Response (404 Not Found):**
```json
{
  "message": "Campeón con ID 999 no encontrado"
}
```

### 3. Obtener Campeones por Rol

**Request:**
```http
GET /api/champions/role/3
```

**Response (200 OK):**
```json
[
  {
    "id": 2,
    "name": "Ahri",
    "primaryRoleName": "Mid"
  },
  {
    "id": 3,
    "name": "Zed",
    "primaryRoleName": "Mid"
  }
]
```

---

## Endpoints de Matchups

### 1. Obtener Todos los Matchups

**Request:**
```http
GET /api/matchups
```

**Response (200 OK):**
```json
[
  {
    "id": 1,
    "playerChampion": {
      "id": 2,
      "name": "Ahri",
      "imageUrl": "..."
    },
    "enemyChampion": {
      "id": 3,
      "name": "Zed",
      "imageUrl": "..."
    },
    "role": {
      "id": 3,
      "name": "Mid"
    },
    "difficulty": "Hard",
    "generalAdvice": "Zed tiene alto burst damage. Juega seguro hasta nivel 6.",
    "tips": [
      {
        "id": 1,
        "category": "EarlyGame",
        "content": "Pushea la wave para llegar nivel 2 primero",
        "priority": 9,
        "authorName": "ProMidlaner"
      }
    ],
    "createdAt": "2025-01-15T10:30:00Z",
    "updatedAt": "2025-01-15T10:30:00Z"
  }
]
```

### 2. Obtener un Matchup por ID

**Request:**
```http
GET /api/matchups/1
```

**Response (200 OK):**
```json
{
  "id": 1,
  "playerChampion": {
    "id": 2,
    "name": "Ahri",
    "title": "the Nine-Tailed Fox"
  },
  "enemyChampion": {
    "id": 3,
    "name": "Zed",
    "title": "the Master of Shadows"
  },
  "role": {
    "id": 3,
    "name": "Mid",
    "description": "Línea media"
  },
  "difficulty": "Hard",
  "generalAdvice": "Zed tiene alto burst damage después de nivel 6",
  "tips": [
    {
      "id": 1,
      "matchupId": 1,
      "category": "EarlyGame",
      "content": "Pushea la wave para llegar nivel 2 primero y ganar presión",
      "priority": 9,
      "authorName": "ProMidlaner",
      "createdAt": "2025-01-15T10:30:00Z"
    },
    {
      "id": 2,
      "matchupId": 1,
      "category": "Items",
      "content": "Compra Zhonya's cuanto antes para contrarrestar su ultimate",
      "priority": 10,
      "authorName": "DiamondPlayer",
      "createdAt": "2025-01-15T11:00:00Z"
    }
  ],
  "createdAt": "2025-01-15T10:30:00Z",
  "updatedAt": "2025-01-15T11:00:00Z"
}
```

### 3. Buscar un Matchup Específico

**Request:**
```http
GET /api/matchups/search?playerChampionId=2&enemyChampionId=3&roleId=3
```

**Parámetros:**
- `playerChampionId`: ID del campeón que juegas (Ahri = 2)
- `enemyChampionId`: ID del campeón enemigo (Zed = 3)
- `roleId`: ID del rol (Mid = 3)

**Response (200 OK):**
```json
{
  "id": 1,
  "playerChampion": { "id": 2, "name": "Ahri" },
  "enemyChampion": { "id": 3, "name": "Zed" },
  "role": { "id": 3, "name": "Mid" },
  "difficulty": "Hard",
  "generalAdvice": "...",
  "tips": [...]
}
```

**Response (404 Not Found):**
```json
{
  "message": "No se encontró información para este matchup",
  "playerChampionId": 2,
  "enemyChampionId": 3,
  "roleId": 3
}
```

### 4. Obtener Matchups de un Campeón

**Request:**
```http
GET /api/matchups/champion/2
```

**Response (200 OK):**
```json
[
  {
    "id": 1,
    "playerChampion": { "id": 2, "name": "Ahri" },
    "enemyChampion": { "id": 3, "name": "Zed" },
    "difficulty": "Hard",
    "tips": [...]
  },
  {
    "id": 2,
    "playerChampion": { "id": 2, "name": "Ahri" },
    "enemyChampion": { "id": 50, "name": "Yasuo" },
    "difficulty": "Medium",
    "tips": [...]
  }
]
```

### 5. Crear un Nuevo Matchup

**Request:**
```http
POST /api/matchups
Content-Type: application/json

{
  "playerChampionId": 2,
  "enemyChampionId": 3,
  "roleId": 3,
  "difficulty": "Hard",
  "generalAdvice": "Zed tiene alto burst damage después de nivel 6. Respeta su ultimate y juega cerca de tu torre."
}
```

**Validaciones:**
- `playerChampionId`: Requerido, debe existir
- `enemyChampionId`: Requerido, debe existir
- `roleId`: Requerido, debe existir
- `difficulty`: Requerido, valores válidos: "Easy", "Medium", "Hard", "Extreme"
- `generalAdvice`: Opcional, máximo 1000 caracteres

**Response (201 Created):**
```json
{
  "id": 1,
  "playerChampion": { "id": 2, "name": "Ahri" },
  "enemyChampion": { "id": 3, "name": "Zed" },
  "role": { "id": 3, "name": "Mid" },
  "difficulty": "Hard",
  "generalAdvice": "Zed tiene alto burst damage...",
  "tips": [],
  "createdAt": "2025-01-15T10:30:00Z",
  "updatedAt": "2025-01-15T10:30:00Z"
}
```

**Response (400 Bad Request) - Champion no existe:**
```json
{
  "message": "Champion con ID 999 no encontrado"
}
```

**Response (409 Conflict) - Matchup ya existe:**
```json
{
  "message": "Este matchup ya existe"
}
```

### 6. Agregar un Consejo a un Matchup

**Request:**
```http
POST /api/matchups/tips
Content-Type: application/json

{
  "matchupId": 1,
  "category": "EarlyGame",
  "content": "Pushea la wave rápidamente para llegar nivel 2 primero. Esto te dará ventaja para tradear.",
  "priority": 8,
  "authorName": "ProPlayer123"
}
```

**Categorías Válidas:**
- `EarlyGame`: Consejos para fase de líneas temprana (niveles 1-5)
- `MidGame`: Consejos para mid game (niveles 6-11)
- `LateGame`: Consejos para late game (nivel 11+)
- `Items`: Recomendaciones de objetos
- `Runes`: Recomendaciones de runas
- `Abilities`: Consejos sobre habilidades
- `General`: Consejos generales

**Validaciones:**
- `matchupId`: Requerido, debe existir
- `category`: Requerido, debe ser una de las categorías válidas
- `content`: Requerido, mínimo 10 caracteres, máximo 2000
- `priority`: Opcional, entre 1 y 10 (default: 5)
- `authorName`: Opcional, máximo 100 caracteres

**Response (200 OK):**
```json
{
  "id": 1,
  "playerChampion": { "id": 2, "name": "Ahri" },
  "enemyChampion": { "id": 3, "name": "Zed" },
  "role": { "id": 3, "name": "Mid" },
  "difficulty": "Hard",
  "generalAdvice": "...",
  "tips": [
    {
      "id": 1,
      "matchupId": 1,
      "category": "EarlyGame",
      "content": "Pushea la wave rápidamente...",
      "priority": 8,
      "authorName": "ProPlayer123",
      "createdAt": "2025-01-15T10:35:00Z"
    }
  ],
  "createdAt": "2025-01-15T10:30:00Z",
  "updatedAt": "2025-01-15T10:35:00Z"
}
```

**Response (404 Not Found):**
```json
{
  "message": "Matchup con ID 999 no encontrado"
}
```

### 7. Eliminar un Matchup

**Request:**
```http
DELETE /api/matchups/1
```

**Response (204 No Content):**
(Sin cuerpo de respuesta)

---

## Endpoints de Roles

### 1. Obtener Todos los Roles

**Request:**
```http
GET /api/roles
```

**Response (200 OK):**
```json
[
  { "id": 1, "name": "Top", "description": "Línea superior" },
  { "id": 2, "name": "Jungle", "description": "Jungla" },
  { "id": 3, "name": "Mid", "description": "Línea media" },
  { "id": 4, "name": "ADC", "description": "Tirador (Bot Lane)" },
  { "id": 5, "name": "Support", "description": "Soporte (Bot Lane)" }
]
```

### 2. Obtener un Rol por ID

**Request:**
```http
GET /api/roles/3
```

**Response (200 OK):**
```json
{
  "id": 3,
  "name": "Mid",
  "description": "Línea media"
}
```

---

## Sincronización con Riot API

### 1. Sincronizar Campeones desde Riot

**Request:**
```http
POST /api/riotsync/sync-champions?language=es_MX
```

**Parámetros:**
- `language` (opcional): Idioma de los datos
  - `en_US`: Inglés (default)
  - `es_MX`: Español (México)
  - `es_ES`: Español (España)
  - Otros: ver [Data Dragon Languages](https://developer.riotgames.com/docs/lol#data-dragon)

**Response (200 OK):**
```json
{
  "message": "Sincronización completada exitosamente",
  "championsSynced": 168,
  "language": "es_MX"
}
```

**Response (500 Internal Server Error):**
```json
{
  "message": "Error al sincronizar campeones",
  "error": "Connection timeout"
}
```

### 2. Obtener Versión de Data Dragon

**Request:**
```http
GET /api/riotsync/version
```

**Response (200 OK):**
```json
{
  "version": "14.1.1"
}
```

---

## Escenarios Completos

### Escenario 1: Setup Inicial de la Aplicación

**Paso 1:** Sincronizar campeones desde Riot
```http
POST /api/riotsync/sync-champions?language=es_MX
```

**Paso 2:** Verificar que se crearon los campeones
```http
GET /api/champions
```

**Paso 3:** Obtener roles disponibles
```http
GET /api/roles
```

### Escenario 2: Usuario Busca Consejos para un Matchup

**Paso 1:** Usuario selecciona "Juego Ahri (Mid) vs Zed"

Frontend hace request:
```http
GET /api/matchups/search?playerChampionId=2&enemyChampionId=3&roleId=3
```

**Paso 2a:** Si existe el matchup (200 OK)
→ Mostrar consejos al usuario

**Paso 2b:** Si no existe (404 Not Found)
→ Mostrar mensaje: "Aún no hay consejos para este matchup. ¿Quieres agregar uno?"

### Escenario 3: Usuario Contribuye con Consejos

**Paso 1:** Verificar si el matchup existe
```http
GET /api/matchups/search?playerChampionId=2&enemyChampionId=3&roleId=3
```

**Paso 2a:** Si existe → Ir al paso 3

**Paso 2b:** Si no existe → Crear el matchup
```http
POST /api/matchups
{
  "playerChampionId": 2,
  "enemyChampionId": 3,
  "roleId": 3,
  "difficulty": "Hard",
  "generalAdvice": "Cuidado con su burst damage"
}
```

**Paso 3:** Agregar el consejo
```http
POST /api/matchups/tips
{
  "matchupId": 1,
  "category": "Items",
  "content": "Compra Zhonya's como segundo ítem para sobrevivir su ultimate",
  "priority": 10,
  "authorName": "Usuario123"
}
```

### Escenario 4: Ver Todos los Matchups de un Campeón

**Frontend:** Página de "Guía de Ahri"

**Request:**
```http
GET /api/matchups/champion/2
```

**Uso:**
- Mostrar lista de todos los matchups de Ahri
- Ordenar por dificultad
- Permitir filtrar por rol

### Escenario 5: Dashboard de Admin - Estadísticas

**Request 1:** Total de campeones
```http
GET /api/champions
→ Contar elementos en el array
```

**Request 2:** Total de matchups
```http
GET /api/matchups
→ Contar elementos en el array
```

**Request 3:** Matchups por dificultad
```javascript
// En el frontend:
const matchups = await fetch('/api/matchups').then(r => r.json());
const byDifficulty = matchups.reduce((acc, m) => {
  acc[m.difficulty] = (acc[m.difficulty] || 0) + 1;
  return acc;
}, {});
// { Easy: 10, Medium: 25, Hard: 15, Extreme: 5 }
```

---

## Manejo de Errores

### 400 Bad Request
**Causa:** Validación fallida

**Ejemplo:**
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Difficulty": [
      "Difficulty debe ser: Easy, Medium, Hard o Extreme"
    ],
    "Content": [
      "Content debe tener al menos 10 caracteres"
    ]
  }
}
```

### 404 Not Found
**Causa:** Recurso no existe

**Ejemplo:**
```json
{
  "message": "Matchup con ID 999 no encontrado"
}
```

### 409 Conflict
**Causa:** Recurso duplicado

**Ejemplo:**
```json
{
  "message": "Este matchup ya existe"
}
```

### 500 Internal Server Error
**Causa:** Error del servidor

**Ejemplo:**
```json
{
  "message": "Error al sincronizar campeones",
  "error": "Connection timeout"
}
```

---

## Testing con cURL

### Linux/macOS
```bash
# GET request
curl https://localhost:7xxx/api/champions

# POST request
curl -X POST https://localhost:7xxx/api/matchups \
  -H "Content-Type: application/json" \
  -d '{"playerChampionId":2,"enemyChampionId":3,"roleId":3,"difficulty":"Hard"}'

# Con certificado autofirmado
curl -k https://localhost:7xxx/api/champions
```

### PowerShell (Windows)
```powershell
# GET request
Invoke-RestMethod -Uri "https://localhost:7xxx/api/champions"

# POST request
$body = @{
    playerChampionId = 2
    enemyChampionId = 3
    roleId = 3
    difficulty = "Hard"
} | ConvertTo-Json

Invoke-RestMethod -Uri "https://localhost:7xxx/api/matchups" `
    -Method POST `
    -Body $body `
    -ContentType "application/json"
```

---

## Testing con Postman

### 1. Importar Colección

Crea una colección con estas requests:

```
Matchup Companion API
├── Champions
│   ├── GET All Champions
│   ├── GET Champion by ID
│   └── GET Champions by Role
├── Matchups
│   ├── GET All Matchups
│   ├── GET Matchup by ID
│   ├── Search Matchup
│   ├── GET Matchups by Champion
│   ├── POST Create Matchup
│   ├── POST Add Tip
│   └── DELETE Matchup
├── Roles
│   ├── GET All Roles
│   └── GET Role by ID
└── Riot Sync
    ├── POST Sync Champions
    └── GET Version
```

### 2. Variables de Entorno

```
baseUrl: https://localhost:7xxx/api
```

### 3. Pre-request Scripts (opcional)

Para generar datos dinámicos:
```javascript
pm.environment.set("timestamp", Date.now());
pm.environment.set("authorName", `User${Math.floor(Math.random() * 1000)}`);
```

---

**Última actualización:** Enero 2026
