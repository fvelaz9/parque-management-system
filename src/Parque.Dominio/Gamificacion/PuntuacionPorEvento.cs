using Parque.Dominio.Atracciones;

namespace Parque.Dominio.Gamificacion;

public class PuntuacionPorEvento : IEstrategiaPuntuacion
{
    public string Nombre => "PorEvento";
    private const int PUNTOS_BASE = 10;
    private const int MULTIPLICADOR_EVENTO = 3;

    public int CalcularPuntos(RegistroVisita registro, AtraccionParque atraccion, List<RegistroVisita> historialDiario, Evento? eventoActivo)
    {
        var puntos = PUNTOS_BASE;

        if (eventoActivo != null && 
            eventoActivo.Estado == EstadoEvento.Programado &&
            eventoActivo.Atracciones.Any(a => a.Id == atraccion.Id))
        {
            puntos *= MULTIPLICADOR_EVENTO;
        }

        return puntos;
    }
}
