using System.ComponentModel.DataAnnotations;
using Parque.Dominio.Usuarios;

namespace Parque.Aplicacion.DTOs.RecompensasDtos;

public class RecompensaDto
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public int CostoEnPuntos { get; set; }
    public int CantidadDisponible { get; set; }
    public NivelMembresia? NivelMembresiaRequerido { get; set; }
    public DateTime? FechaCreacion { get; set; } 
}
