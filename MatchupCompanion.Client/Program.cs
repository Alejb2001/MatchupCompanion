using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MatchupCompanion.Client;
using MatchupCompanion.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configurar HttpClient con la URL de la API
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("https://localhost:7285")
});

// Registrar servicios
builder.Services.AddScoped<IChampionService, ChampionService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IMatchupService, MatchupService>();

await builder.Build().RunAsync();
