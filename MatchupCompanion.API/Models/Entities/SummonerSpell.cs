using System.ComponentModel.DataAnnotations;

namespace MatchupCompanion.API.Models.Entities;

/// <summary>
/// Representa un hechizo de invocador (Flash, Ignite, Teleport, etc.)
/// </summary>
public class SummonerSpell
{
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// ID del hechizo según Riot Games API
    /// </summary>
    [Required]
    public int RiotSpellId { get; set; }

    /// <summary>
    /// Nombre del hechizo (ej: "Flash", "Ignite", "Teleport")
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Descripción del hechizo
    /// </summary>
    [MaxLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// URL de la imagen del hechizo
    /// </summary>
    [MaxLength(200)]
    public string? ImageUrl { get; set; }

    /// <summary>
    /// Tiempo de enfriamiento en segundos
    /// </summary>
    public int Cooldown { get; set; }
}
