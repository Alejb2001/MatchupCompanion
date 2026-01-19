using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MatchupCompanion.Client;
using MatchupCompanion.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configurar HttpClient con la URL de la API
// Usar HTTP para evitar problemas de certificados SSL en desarrollo
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("http://localhost:5007/")
});

// Registrar servicios
builder.Services.AddScoped<IChampionService, ChampionService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IMatchupService, MatchupService>();
builder.Services.AddScoped<IRuneService, RuneService>();
builder.Services.AddScoped<IItemService, ItemService>();

await builder.Build().RunAsync();
