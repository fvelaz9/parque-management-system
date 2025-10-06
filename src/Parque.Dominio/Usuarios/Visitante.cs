using Parque.Dominio.Excepciones;

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
        ValidarFecha(fechaNacimiento);

        var visitante = new Visitante();
        visitante.Id = Guid.NewGuid();
        visitante.FechaNacimiento = fechaNacimiento;
        visitante.NivelMembresia = NivelMembresia.Estandar;
        return visitante;
    }

    private static void ValidarFecha(DateTime fecha)
    {
        var fechaMinima = DateTime.UtcNow.AddYears(-125);

        if(fecha < fechaMinima)
        {
            throw new ExcepcionDominio("La edad máxima son 125 años.");
        }

        if(fecha > DateTime.UtcNow)
        {
            throw new ExcepcionDominio("La fecha de nacimiento no puede ser en el futuro");
        }
    }

    public void ActualizarFecha(DateTime fecha)
    {
        ValidarFecha(fecha);
        FechaNacimiento = fecha;
    }

    public void AsignarMembresia(NivelMembresia nivel)
    {
        NivelMembresia = nivel;
    }
}
