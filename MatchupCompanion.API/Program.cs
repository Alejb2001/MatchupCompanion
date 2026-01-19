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
builder.Services.AddScoped<IRuneRepository, RuneRepository>();
builder.Services.AddScoped<IItemRepository, ItemRepository>();

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

// Sincronización automática de datos de Riot al iniciar
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var riotApiService = services.GetRequiredService<RiotApiService>();

        // Verificar si hay datos, si no, sincronizar
        var hasChampions = await context.Champions.AnyAsync();
        var hasRunes = await context.Runes.AnyAsync();
        var hasItems = await context.Items.AnyAsync();

        if (!hasChampions || !hasRunes || !hasItems)
        {
            app.Logger.LogInformation("Sincronizando datos desde Riot Data Dragon (español)...");

            // Usar español (es_ES) para todos los datos
            const string language = "es_ES";

            if (!hasChampions)
            {
                app.Logger.LogInformation("Sincronizando campeones...");
                await riotApiService.SyncChampionsFromRiotAsync(language);
            }

            if (!hasRunes)
            {
                app.Logger.LogInformation("Sincronizando runas...");
                await riotApiService.SyncRunesFromRiotAsync(language);
            }

            if (!hasItems)
            {
                app.Logger.LogInformation("Sincronizando items...");
                await riotApiService.SyncItemsFromRiotAsync(language);
            }

            app.Logger.LogInformation("Sincronización completada");
        }
        else
        {
            app.Logger.LogInformation("Base de datos ya tiene datos de Riot");
        }
    }
    catch (Exception ex)
    {
        app.Logger.LogError("Error al sincronizar datos de Riot: {Message}", ex.Message);
        app.Logger.LogError("Stack trace: {StackTrace}", ex.StackTrace);
        if (ex.InnerException != null)
        {
            app.Logger.LogError("Inner exception: {InnerMessage}", ex.InnerException.Message);
        }
        app.Logger.LogWarning("La app continuará, puedes sincronizar manualmente desde Swagger: POST /api/RiotSync/sync-all");
    }
}

app.Logger.LogInformation("Matchup Companion API iniciada correctamente");

app.Run();
