using Parque.Dominio.Atracciones;

namespace Parque.Dominio.Gamificacion;

public interface IEstrategiaPuntuacion
{
    string Nombre { get; }
    int CalcularPuntos(RegistroVisita registro, AtraccionParque atraccionParque, List<RegistroVisita> historialDiario, Evento? eventoActivo);
}
