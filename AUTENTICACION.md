# Sistema de Autenticación - Matchup Companion

## Resumen

Se ha implementado un sistema completo de autenticación y autorización con las siguientes características:

### Funcionalidades Implementadas

1. **Registro de Usuarios**
   - Los usuarios pueden crear cuentas con email, nombre de usuario y contraseña
   - Validación de contraseñas (mínimo 6 caracteres, con mayúsculas, minúsculas y números)
   - Posibilidad de seleccionar rol preferido de League of Legends

2. **Inicio de Sesión**
   - Login con email y contraseña
   - Autenticación basada en JWT tokens
   - Opción "Recordarme" para sesiones persistentes

3. **Modo Invitado**
   - Los usuarios pueden acceder como invitados sin registrarse
   - Las sesiones de invitado expiran después de 24 horas
   - Los invitados pueden **VER** matchups pero **NO pueden editar** ni crear

4. **Control de Acceso**
   - Las rutas de creación y edición requieren autenticación
   - Los usuarios invitados son bloqueados en endpoints protegidos
   - Mensajes claros de error para usuarios no autorizados

## Arquitectura

### Backend (MatchupCompanion.API)

#### Configuración de Identity y JWT
- **Ubicación**: [Program.cs](MatchupCompanion.API/Program.cs)
- ASP.NET Core Identity configurado con ApplicationUser
- JWT Bearer authentication
- Configuración de políticas de contraseña

#### Controlador de Autenticación
- **Ubicación**: [AuthController.cs](MatchupCompanion.API/Controllers/AuthController.cs)
- Endpoints disponibles:
  - `POST /api/auth/register` - Registrar nuevo usuario
  - `POST /api/auth/login` - Iniciar sesión
  - `POST /api/auth/guest` - Crear sesión de invitado
  - `POST /api/auth/logout` - Cerrar sesión
  - `GET /api/auth/me` - Obtener usuario actual
  - `GET /api/auth/validate` - Validar token

#### Servicio de Autenticación
- **Ubicación**: [AuthService.cs](MatchupCompanion.API/Services/Auth/AuthService.cs)
- Generación de tokens JWT
- Gestión de usuarios y sesiones de invitados
- Verificación de permisos

#### Protección de Endpoints
- **Ubicación**: [MatchupsController.cs](MatchupCompanion.API/Controllers/MatchupsController.cs)
- Endpoints protegidos con `[Authorize]`:
  - `POST /api/matchups` - Crear matchup
  - `PUT /api/matchups/{id}` - Actualizar matchup
  - `DELETE /api/matchups/{id}` - Eliminar matchup
  - `POST /api/matchups/tips` - Agregar tip
- Validación adicional para bloquear usuarios invitados

### Frontend (MatchupCompanion.Client)

#### Páginas de Autenticación
- [Login.razor](MatchupCompanion.Client/Pages/Login.razor) - Página de inicio de sesión
- [Register.razor](MatchupCompanion.Client/Pages/Register.razor) - Página de registro

#### Servicio de Autenticación
- **Ubicación**: [AuthenticationService.cs](MatchupCompanion.Client/Services/Auth/AuthenticationService.cs)
- Gestión de tokens en localStorage
- Llamadas a API de autenticación
- Actualización del estado de autenticación

#### AuthenticationStateProvider
- **Ubicación**: [CustomAuthenticationStateProvider.cs](MatchupCompanion.Client/Services/Auth/CustomAuthenticationStateProvider.cs)
- Lectura de claims desde JWT
- Validación de expiración de tokens
- Notificación de cambios de estado

#### Rutas Protegidas
Las siguientes páginas requieren autenticación:
- [CreateMatchup.razor](MatchupCompanion.Client/Pages/CreateMatchup.razor) - Crear matchup
- [EditMatchup.razor](MatchupCompanion.Client/Pages/EditMatchup.razor) - Editar matchup
- [AddTip.razor](MatchupCompanion.Client/Pages/AddTip.razor) - Agregar tip

#### Navegación Dinámica
- **Ubicación**: [NavMenu.razor](MatchupCompanion.Client/Layout/NavMenu.razor)
- Muestra/oculta opciones según estado de autenticación
- Botón de login/logout en header

### Modelos Compartidos (MatchupCompanion.Shared)

DTOs de autenticación:
- [LoginRequest.cs](MatchupCompanion.Shared/Models/Auth/LoginRequest.cs)
- [RegisterRequest.cs](MatchupCompanion.Shared/Models/Auth/RegisterRequest.cs)
- [AuthResponse.cs](MatchupCompanion.Shared/Models/Auth/AuthResponse.cs)
- [UserDto.cs](MatchupCompanion.Shared/Models/Auth/UserDto.cs)

## Base de Datos

### Tablas de Identity

La migración `AddIdentityTables` creó las siguientes tablas:

- `AspNetUsers` - Usuarios del sistema (extiende ApplicationUser)
- `AspNetRoles` - Roles del sistema
- `AspNetUserRoles` - Relación usuarios-roles
- `AspNetUserClaims` - Claims de usuarios
- `AspNetRoleClaims` - Claims de roles
- `AspNetUserLogins` - Logins externos
- `AspNetUserTokens` - Tokens de usuario

### Campos Personalizados de ApplicationUser

- `DisplayName` - Nombre para mostrar
- `PreferredRoleId` - Rol preferido de LoL (FK a GameRoles)
- `CreatedAt` - Fecha de creación
- `LastLoginAt` - Último inicio de sesión
- `IsGuest` - Indica si es invitado
- `GuestExpiresAt` - Expiración de sesión de invitado

### Relaciones

- `Matchup.CreatedById` → `AspNetUsers.Id`
- `MatchupTip.AuthorId` → `AspNetUsers.Id`
- `ApplicationUser.PreferredRoleId` → `GameRoles.Id`

## Configuración

### appsettings.json

```json
{
  "Jwt": {
    "SecretKey": "MatchupCompanion-SuperSecretKey-ForDevelopment-2026-ChangeInProduction",
    "Issuer": "MatchupCompanionAPI",
    "Audience": "MatchupCompanionClient",
    "ExpirationInMinutes": 60,
    "RefreshTokenExpirationInDays": 7
  },
  "Identity": {
    "Password": {
      "RequireDigit": true,
      "RequireLowercase": true,
      "RequireUppercase": true,
      "RequireNonAlphanumeric": false,
      "RequiredLength": 6
    },
    "User": {
      "RequireUniqueEmail": true
    },
    "SignIn": {
      "RequireConfirmedEmail": false
    }
  }
}
```

**IMPORTANTE**: En producción, cambiar el `SecretKey` por uno seguro y almacenarlo en variables de entorno.

## Uso

### Ejecutar la Aplicación

1. **Iniciar el Backend**:
```bash
cd MatchupCompanion.API
dotnet run
```
La API estará disponible en `http://localhost:5007`

2. **Iniciar el Frontend**:
```bash
cd MatchupCompanion.Client
dotnet run
```
El cliente estará disponible en `http://localhost:5000`

### Flujo de Usuario

1. **Usuario Nuevo**:
   - Acceder a `/register`
   - Completar formulario con email, usuario y contraseña
   - (Opcional) Seleccionar rol preferido
   - Hacer clic en "Crear Cuenta"
   - Serás redirigido automáticamente a la home con sesión iniciada

2. **Usuario Existente**:
   - Acceder a `/login`
   - Ingresar email y contraseña
   - (Opcional) Marcar "Recordarme"
   - Hacer clic en "Iniciar Sesión"

3. **Invitado**:
   - En `/login`, hacer clic en "Continuar como Invitado"
   - Podrás ver matchups pero no editarlos
   - La sesión expira en 24 horas

4. **Crear/Editar Matchup**:
   - Solo usuarios autenticados (no invitados)
   - Acceder a "Crear Matchup" o "Editar Matchup" desde el menú
   - Si no estás autenticado, serás redirigido a `/login`

## Seguridad

### Medidas Implementadas

1. **Validación de Contraseñas**:
   - Longitud mínima de 6 caracteres
   - Requiere mayúsculas, minúsculas y números
   - Configurable en `appsettings.json`

2. **JWT Tokens**:
   - Firmados con HMAC SHA256
   - Incluyen claims de usuario (ID, email, roles, IsGuest)
   - Expiran después de 60 minutos (configurable)

3. **Protección de Endpoints**:
   - `[Authorize]` en controladores
   - Validación adicional para bloquear invitados
   - Validación de permisos en el servicio

4. **Validación en Frontend**:
   - Tokens almacenados en localStorage
   - Validación de expiración antes de cada request
   - Rutas protegidas con `@attribute [Authorize]`

### Consideraciones para Producción

1. **Cambiar la clave secreta JWT** en `appsettings.json`
2. **Usar HTTPS** en todos los endpoints
3. **Habilitar confirmación de email** (`RequireConfirmedEmail: true`)
4. **Implementar refresh tokens** para renovar tokens expirados
5. **Agregar rate limiting** para prevenir ataques de fuerza bruta
6. **Almacenar secretos en variables de entorno** o Azure Key Vault
7. **Agregar logging** de intentos de autenticación fallidos

## Testing

### Endpoints de API (Swagger)

Acceder a `http://localhost:5007` para ver la documentación interactiva de Swagger.

Los endpoints protegidos muestran un candado 🔒 y requieren un token Bearer.

### Usuarios de Prueba

Crear usuarios de prueba mediante:
1. La página de registro en el frontend
2. El endpoint `POST /api/auth/register` en Swagger

### Probar Modo Invitado

1. En `/login`, hacer clic en "Continuar como Invitado"
2. Intentar acceder a `/create-matchup` o `/edit-matchup`
3. Verificar que se muestra un mensaje de acceso denegado

## Troubleshooting

### Error: "No se puede convertir IdentityRole a Role"
- Asegurarse de usar `_context.GameRoles` en lugar de `_context.Roles`
- La tabla `Roles` de LoL fue renombrada a `GameRoles` para evitar conflictos

### Error: "401 Unauthorized" en requests
- Verificar que el token esté presente en localStorage
- Verificar que el token no haya expirado
- Verificar que el header `Authorization: Bearer {token}` se esté enviando

### Usuario invitado puede editar matchups
- Verificar que la validación `IsGuest` esté en los endpoints del backend
- No confiar solo en la UI del frontend

### Token no se guarda después del login
- Verificar que `Blazored.LocalStorage` esté configurado en `Program.cs`
- Ver console del navegador para errores de JavaScript

## Próximos Pasos (Mejoras Futuras)

1. **Refresh Tokens**: Implementar renovación automática de tokens
2. **Confirmación de Email**: Enviar emails de confirmación al registrarse
3. **Recuperación de Contraseña**: Implementar flujo de "Olvidé mi contraseña"
4. **Roles y Permisos**: Agregar roles de administrador y moderador
5. **OAuth**: Permitir login con Google, Discord, Riot Games
6. **2FA**: Autenticación de dos factores
7. **Audit Logging**: Registrar todas las acciones de usuarios
8. **Rate Limiting**: Limitar intentos de login

## Soporte

Para problemas o preguntas, revisar:
- [ARCHITECTURE.md](ARCHITECTURE.md) - Arquitectura general del proyecto
- [PROJECT-STATUS.md](PROJECT-STATUS.md) - Estado del proyecto
- [README.md](README.md) - Documentación general
