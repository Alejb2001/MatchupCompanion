using MatchupCompanion.API.Models.Entities;
using Microsoft.AspNetCore.Identity;

namespace MatchupCompanion.API.Data;

public static class SeedData
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

        logger.LogInformation("🔧 Iniciando seed de roles y usuarios...");

        // Crear rol Admin si no existe
        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            await roleManager.CreateAsync(new IdentityRole("Admin"));
            logger.LogInformation("✅ Rol 'Admin' creado exitosamente");
        }
        else
        {
            logger.LogInformation("ℹ️ Rol 'Admin' ya existe");
        }

        // Asignar rol Admin al usuario admin@matchup.com si existe
        var adminUser = await userManager.FindByEmailAsync("admin@matchup.com");
        if (adminUser != null)
        {
            if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
                logger.LogInformation("✅ Rol 'Admin' asignado a usuario: admin@matchup.com");
            }
            else
            {
                logger.LogInformation("ℹ️ Usuario admin@matchup.com ya tiene el rol 'Admin'");
            }
        }
        else
        {
            logger.LogWarning("⚠️ Usuario admin@matchup.com no encontrado en la base de datos");
        }

        logger.LogInformation("🔧 Seed de roles y usuarios completado");
    }
}
