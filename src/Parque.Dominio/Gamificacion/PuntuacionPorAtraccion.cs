using Parque.Dominio.Atracciones;

namespace Parque.Dominio.Gamificacion;

public class PuntuacionPorAtraccion : IEstrategiaPuntuacion
{
    public string Nombre => "porAtraccion";

    public int CalcularPuntos(RegistroVisita registro, AtraccionParque atraccion,
        List<RegistroVisita> historialDiario, Evento? eventoActivo)
    {
        return atraccion.Tipo switch
        {
            TipoAtraccion.MontañaRusa => 15,
            TipoAtraccion.Simulador => 12,
            TipoAtraccion.Espectaculo => 10,
            TipoAtraccion.ZonaInteractiva => 8,
            _ => 10
        };
    }
}
