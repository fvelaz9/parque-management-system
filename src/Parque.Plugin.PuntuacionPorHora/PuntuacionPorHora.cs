using System.Diagnostics.CodeAnalysis;
using Parque.Dominio;
using Parque.Dominio.Atracciones;
using Parque.Dominio.Gamificacion;

namespace Parque.Plugin.PuntuacionPorHora;
[ExcludeFromCodeCoverage]
public class PuntuacionPorHoraEstrategia : IEstrategiaPuntuacion
{
    private const int PUNTOS_BASE = 10;
    private const int MULTIPLICADOR_PICO = 2;
    private const double MULTIPLICADOR_VALLE = 0.5;

    public string Nombre => "PorHora";

    public int CalcularPuntos(
        RegistroVisita registro,
        AtraccionParque atraccionParque,
        List<RegistroVisita> historialDiario,
        Evento? eventoActivo)
    {
        var hora = registro.FechaIngreso.Hour;

        if(EsHorarioPico(hora))
        {
            return PUNTOS_BASE * MULTIPLICADOR_PICO;
        }

        if(EsHorarioValle(hora))
        {
            return (int)(PUNTOS_BASE * MULTIPLICADOR_VALLE);
        }

        return PUNTOS_BASE;
    }

    private static bool EsHorarioPico(int hora)
    {
        return (hora >= 10 && hora < 12) || (hora >= 18 && hora < 20);
    }

    private static bool EsHorarioValle(int hora)
    {
        return hora < 10 || hora >= 20;
    }
}
