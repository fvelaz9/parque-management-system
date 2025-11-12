using System.ComponentModel.DataAnnotations;
using Parque.Dominio.Usuarios;

namespace Parque.Aplicacion.DTOs.RecompensasDtos;

public class RecompensaDto
{
    public Guid Id { get; set; }
    
    [Required(ErrorMessage = "El nombre es obligatorio")]
    [MaxLength(100)]
    public string Nombre { get; set; } = string.Empty;
    
    [MaxLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres")]
    public string? Descripcion { get; set; }
    
    [Required(ErrorMessage = "El costo en puntos es obligatorio")]
    [Range(1, int.MaxValue, ErrorMessage = "El costo debe ser mayor a 0")]
    public int CostoEnPuntos { get; set; }
    
    [Required]
    [Range(0, int.MaxValue)]
    public int CantidadDisponible { get; set; }
    
    public NivelMembresia? NivelMembresiaRequerido { get; set; }
    
    public DateTime? FechaCreacion { get; set; } 
}
