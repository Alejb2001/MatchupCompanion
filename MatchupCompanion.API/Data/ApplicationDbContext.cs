using MatchupCompanion.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace MatchupCompanion.API.Data;

/// <summary>
/// Contexto de base de datos principal de la aplicación
/// </summary>
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // DbSets para cada entidad
    public DbSet<Champion> Champions { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Matchup> Matchups { get; set; }
    public DbSet<MatchupTip> MatchupTips { get; set; }
    public DbSet<Rune> Runes { get; set; }
    public DbSet<Item> Items { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuración de la entidad Champion
        modelBuilder.Entity<Champion>(entity =>
        {
            entity.HasIndex(e => e.RiotChampionId).IsUnique();
            entity.HasIndex(e => e.Name);

            // Relación con Role (muchos a uno)
            entity.HasOne(c => c.PrimaryRole)
                .WithMany(r => r.Champions)
                .HasForeignKey(c => c.PrimaryRoleId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Configuración de la entidad Role
        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasIndex(e => e.Name).IsUnique();
        });

        // Configuración de la entidad Matchup
        modelBuilder.Entity<Matchup>(entity =>
        {
            // Relación con PlayerChampion
            entity.HasOne(m => m.PlayerChampion)
                .WithMany(c => c.MatchupsAsPlayerChampion)
                .HasForeignKey(m => m.PlayerChampionId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación con EnemyChampion
            entity.HasOne(m => m.EnemyChampion)
                .WithMany(c => c.MatchupsAsEnemyChampion)
                .HasForeignKey(m => m.EnemyChampionId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación con Role
            entity.HasOne(m => m.Role)
                .WithMany(r => r.MatchupsAsPlayerRole)
                .HasForeignKey(m => m.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            // Índice compuesto para búsquedas rápidas
            entity.HasIndex(m => new { m.PlayerChampionId, m.EnemyChampionId, m.RoleId })
                .IsUnique();
        });

        // Configuración de la entidad MatchupTip
        modelBuilder.Entity<MatchupTip>(entity =>
        {
            entity.HasOne(mt => mt.Matchup)
                .WithMany(m => m.Tips)
                .HasForeignKey(mt => mt.MatchupId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(mt => mt.Category);
            entity.HasIndex(mt => mt.Priority);
        });

        // Configuración de la entidad Rune
        modelBuilder.Entity<Rune>(entity =>
        {
            entity.HasIndex(r => r.RiotRuneId).IsUnique();
            entity.HasIndex(r => r.TreeId);
            entity.HasIndex(r => r.SlotIndex);
        });

        // Configuración de la entidad Item
        modelBuilder.Entity<Item>(entity =>
        {
            entity.HasIndex(i => i.RiotItemId).IsUnique();
            entity.HasIndex(i => i.Name);
            entity.HasIndex(i => i.IsCompleted);
            entity.HasIndex(i => i.IsPurchasable);
        });

        // Seed data inicial
        SeedData(modelBuilder);
    }

    /// <summary>
    /// Datos iniciales para la base de datos
    /// </summary>
    private void SeedData(ModelBuilder modelBuilder)
    {
        // Seed de Roles
        modelBuilder.Entity<Role>().HasData(
            new Role { Id = 1, Name = "Top", Description = "Línea superior" },
            new Role { Id = 2, Name = "Jungle", Description = "Jungla" },
            new Role { Id = 3, Name = "Mid", Description = "Línea media" },
            new Role { Id = 4, Name = "ADC", Description = "Tirador (Bot Lane)" },
            new Role { Id = 5, Name = "Support", Description = "Soporte (Bot Lane)" }
        );

        // Seed de algunos campeones de ejemplo
        // Nota: En producción, estos deberían obtenerse de la API de Riot
        modelBuilder.Entity<Champion>().HasData(
            new Champion
            {
                Id = 1,
                RiotChampionId = "266",
                Name = "Aatrox",
                Title = "the Darkin Blade",
                PrimaryRoleId = 1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Champion
            {
                Id = 2,
                RiotChampionId = "103",
                Name = "Ahri",
                Title = "the Nine-Tailed Fox",
                PrimaryRoleId = 3,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Champion
            {
                Id = 3,
                RiotChampionId = "238",
                Name = "Zed",
                Title = "the Master of Shadows",
                PrimaryRoleId = 3,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        );
    }
}
