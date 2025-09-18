namespace Parque.Dominio.Usuarios;
public class Visitante
{
    public Guid Id { get; private set; }
    public DateTime FechaNacimiento { get; private set; }
    public NivelMembresia NivelMembresia { get; private set; }
    private Visitante()
    {
    }

    public static Visitante Crear(DateTime fechaNacimiento)
    {
        var visitante = new Visitante();
        visitante.Id = Guid.NewGuid();
        visitante.FechaNacimiento = fechaNacimiento;
        visitante.NivelMembresia = NivelMembresia.Estandar;
        return visitante;
    }
}
