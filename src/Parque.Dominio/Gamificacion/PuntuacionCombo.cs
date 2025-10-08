using Parque.Dominio.Atracciones;

namespace Parque.Dominio.Gamificacion;

public class PuntuacionCombo : IEstrategiaPuntuacion
{
    public string Nombre => "Combo";
    
    private readonly int _puntosBase;
    private readonly int _puntosCombo;
    private readonly int _minutosVentana;
    private readonly int _atraccionesMinimasCombo;
    
    public PuntuacionCombo(
        int minutosVentana = 10, 
        int atraccionesMinimasCombo = 3,
        int puntosBase = 8, 
        int puntosCombo = 25)
    {
        if(minutosVentana <= 0)
        {
            throw new ArgumentException("Los minutos de ventana deben ser mayores a 0", nameof(minutosVentana));
        }

        if(atraccionesMinimasCombo < 2)
        {
            throw new ArgumentException("Debe requerir al menos 2 atracciones para combo",
                nameof(atraccionesMinimasCombo));
        }

        if(puntosBase < 0)
        {
            throw new ArgumentException("Los puntos base no pueden ser negativos", nameof(puntosBase));
        }

        if(puntosCombo < 0)
        {
            throw new ArgumentException("Los puntos combo no pueden ser negativos", nameof(puntosCombo));
        }

        _minutosVentana = minutosVentana;
        _atraccionesMinimasCombo = atraccionesMinimasCombo;
        _puntosBase = puntosBase;
        _puntosCombo = puntosCombo;
    }

    public int CalcularPuntos(RegistroVisita registro, AtraccionParque atraccion, List<RegistroVisita> historialDiario, Evento? eventoActivo)
    {
        var atraccionesDistintas = historialDiario
            .Where(h => (registro.FechaIngreso - h.FechaIngreso).TotalMinutes <= _minutosVentana)
            .Where(h => h.AtraccionId != registro.AtraccionId) 
            .Select(h => h.AtraccionId)
            .Distinct()
            .Count();

        var totalAtracciones = atraccionesDistintas + 1;

        if (totalAtracciones >= _atraccionesMinimasCombo)
        {
            return _puntosCombo; 
        }

        return _puntosBase;   
    }
}
