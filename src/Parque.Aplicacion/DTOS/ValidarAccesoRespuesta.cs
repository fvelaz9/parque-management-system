namespace Parque.Aplicacion.DTOS;

public class ValidarAccesoRespuesta
{
    public bool AccesoPermitido { get; set; }
    public string Mensaje { get; set; } = string.Empty;
    public string? NombreAtraccion { get; set; }
    public string? NombreVisitante { get; set; }
}
