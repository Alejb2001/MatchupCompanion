using System.ComponentModel.DataAnnotations;

namespace MatchupCompanion.API.Models.Entities;

/// <summary>
/// Representa una runa del sistema de runas reforjadas de League of Legends
/// </summary>
public class Rune
{
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// ID único de Riot para esta runa (ej: 8112 para Electrocute)
    /// </summary>
    [Required]
    public int RiotRuneId { get; set; }

    /// <summary>
    /// Clave interna de la runa (ej: "Electrocute")
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// Nombre mostrado de la runa
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Path del icono para construir la URL de imagen
    /// </summary>
    [MaxLength(300)]
    public string? IconPath { get; set; }

    /// <summary>
    /// Nombre del árbol de runas (Domination, Precision, Sorcery, Resolve, Inspiration)
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string TreeName { get; set; } = string.Empty;

    /// <summary>
    /// ID del árbol de runas (8100=Domination, 8000=Precision, 8200=Sorcery, 8400=Resolve, 8300=Inspiration)
    /// </summary>
    [Required]
    public int TreeId { get; set; }

    /// <summary>
    /// Índice del slot dentro del árbol (0=Keystone, 1=Row2, 2=Row3, 3=Row4)
    /// </summary>
    [Required]
    public int SlotIndex { get; set; }

    /// <summary>
    /// Descripción corta del efecto de la runa
    /// </summary>
    [MaxLength(1000)]
    public string? ShortDescription { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
