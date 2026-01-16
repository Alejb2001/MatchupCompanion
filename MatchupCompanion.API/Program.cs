using MatchupCompanion.API.Data;
using MatchupCompanion.API.Data.Repositories;
using MatchupCompanion.API.Data.Repositories.Interfaces;
using MatchupCompanion.API.ExternalServices;
using MatchupCompanion.API.Services;
using MatchupCompanion.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configuración de la base de datos
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseSqlServer(connectionString);
});

// Configuración de CORS (importante para conexiones desde el frontend)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Configuración de Controllers
builder.Services.AddControllers();

// Configuración de Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Matchup Companion API",
        Version = "v1",
        Description = "API para gestionar información de matchups de League of Legends",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Alejandro Burciaga Calzadillas"
        }
    });

    // Incluir comentarios XML en Swagger (opcional)
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

// Inyección de dependencias - Repositorios
builder.Services.AddScoped<IChampionRepository, ChampionRepository>();
builder.Services.AddScoped<IMatchupRepository, MatchupRepository>();
builder.Services.AddScoped<IMatchupTipRepository, MatchupTipRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();

// Inyección de dependencias - Servicios
builder.Services.AddScoped<IChampionService, ChampionService>();
builder.Services.AddScoped<IMatchupService, MatchupService>();

// Inyección de dependencias - Servicios externos
builder.Services.AddHttpClient<RiotApiService>();
builder.Services.AddScoped<RiotApiService>();

// Logging mejorado (opcional)
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var app = builder.Build();

// Configuración del pipeline de requests HTTP

// Habilitar Swagger (en desarrollo y producción para testing)
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Matchup Companion API v1");
    options.RoutePrefix = string.Empty; // Swagger UI en la raíz
});

app.UseHttpsRedirection();

// Aplicar política de CORS
app.UseCors("AllowAll");

// Autorización (para futuras implementaciones con autenticación)
app.UseAuthorization();

// Mapear controllers
app.MapControllers();

// Opcional: Aplicar migraciones automáticamente al iniciar (solo en desarrollo)
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        try
        {
            var context = services.GetRequiredService<ApplicationDbContext>();
            // Descomentar la siguiente línea para aplicar migraciones automáticamente
            // await context.Database.MigrateAsync();
            app.Logger.LogInformation("Base de datos lista");
        }
        catch (Exception ex)
        {
            app.Logger.LogError(ex, "Error al inicializar la base de datos");
        }
    }
}

app.Logger.LogInformation("Matchup Companion API iniciada correctamente");

app.Run();
