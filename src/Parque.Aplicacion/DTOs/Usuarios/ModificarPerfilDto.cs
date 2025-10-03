namespace Parque.Aplicacion.DTOs.Usuarios;
public class ModificarPerfilDto
{
    public string? Nombre { get; set; }
    public string? Apellido { get; set; }
    public string? Email { get; set; }
    public DateTime? FechaNacimiento { get; set; }
}
