namespace Parque.Dominio.Usuarios;
public class Visitante
{
    public Guid Id { get; private set; }
    public DateTime FechaNacimiento { get; private set; }
    public NivelMembresia Nivel { get; private set; }
    private Visitante()
    {
    }
}
