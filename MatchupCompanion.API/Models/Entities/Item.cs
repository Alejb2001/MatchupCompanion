using System.ComponentModel.DataAnnotations;

namespace MatchupCompanion.API.Models.Entities;

/// <summary>
/// Representa un item de League of Legends
/// </summary>
public class Item
{
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// ID único de Riot para este item (ej: 3006 para Berserker's Greaves)
    /// </summary>
    [Required]
    public int RiotItemId { get; set; }

    /// <summary>
    /// Nombre del item
    /// </summary>
    [Required]
    [MaxLength(300)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Descripción HTML del item con stats y efectos
    /// </summary>
    [MaxLength(2000)]
    public string? Description { get; set; }

    /// <summary>
    /// Path del icono para construir la URL de imagen
    /// </summary>
    [MaxLength(100)]
    public string? IconPath { get; set; }

    /// <summary>
    /// Costo total en oro del item
    /// </summary>
    public int TotalGold { get; set; }

    /// <summary>
    /// Tags del item en formato JSON (ej: ["Damage", "CriticalStrike"])
    /// </summary>
    [MaxLength(500)]
    public string? Tags { get; set; }

    /// <summary>
    /// Si el item puede comprarse directamente en la tienda
    /// </summary>
    public bool IsPurchasable { get; set; }

    /// <summary>
    /// Si es un item completo/final (depth >= 2)
    /// </summary>
    public bool IsCompleted { get; set; }

    /// <summary>
    /// IDs de items componente necesarios para construir este (formato: "1001,1036")
    /// </summary>
    [MaxLength(200)]
    public string? BuildsFrom { get; set; }

    /// <summary>
    /// IDs de items que se pueden construir con este (formato: "3006,3009")
    /// </summary>
    [MaxLength(500)]
    public string? BuildsInto { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
