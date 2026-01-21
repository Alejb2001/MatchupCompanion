using MatchupCompanion.API.Models.Entities;
using Microsoft.AspNetCore.Identity;

namespace MatchupCompanion.API.Data;

public static class SeedData
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        // Crear rol Admin si no existe
        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            await roleManager.CreateAsync(new IdentityRole("Admin"));
        }

        // Asignar rol Admin al usuario admin@matchup.com si existe
        var adminUser = await userManager.FindByEmailAsync("admin@matchup.com");
        if (adminUser != null && !await userManager.IsInRoleAsync(adminUser, "Admin"))
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }
}
