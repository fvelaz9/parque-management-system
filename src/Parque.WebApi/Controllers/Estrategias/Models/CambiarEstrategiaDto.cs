using System.ComponentModel.DataAnnotations;

namespace Parque.WebApi.Controllers.Estrategias.Models;

public class CambiarEstrategiaDto
{
    [Required(ErrorMessage = "El nombre de la estrategia es requerido")]
    [MinLength(1, ErrorMessage = "El nombre no puede estar vacío")]
    public string NombreEstrategia { get; set; } = string.Empty;
}
