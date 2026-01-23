using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.Authorization;
using MatchupCompanion.Client;
using MatchupCompanion.Client.Services;
using MatchupCompanion.Client.Services.Auth;
using MatchupCompanion.Client.Handlers;
using Blazored.LocalStorage;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configurar autenticación (debe estar antes de HttpClient)
builder.Services.AddAuthorizationCore();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();

// Registrar el handler de autorización como Scoped para acceder a ILocalStorageService
builder.Services.AddScoped<AuthorizationMessageHandler>();

// Configurar HttpClient con la URL de la API y el handler de autorización
builder.Services.AddScoped(sp =>
{
    var handler = sp.GetRequiredService<AuthorizationMessageHandler>();
    // Configurar el InnerHandler correctamente
    handler.InnerHandler = new HttpClientHandler();

    var httpClient = new HttpClient(handler)
    {
        BaseAddress = new Uri("http://localhost:5007/")
    };
    return httpClient;
});

// Registrar servicios
builder.Services.AddScoped<IChampionService, ChampionService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IMatchupService, MatchupService>();
builder.Services.AddScoped<IRuneService, RuneService>();
builder.Services.AddScoped<IItemService, ItemService>();
builder.Services.AddScoped<ISummonerSpellService, SummonerSpellService>();
builder.Services.AddScoped<IFavoriteChampionsService, FavoriteChampionsService>();

await builder.Build().RunAsync();
