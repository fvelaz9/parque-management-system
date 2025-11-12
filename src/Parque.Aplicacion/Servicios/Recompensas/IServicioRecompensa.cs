using Parque.Aplicacion.DTOs.RecompensasDtos;
using Parque.Dominio.Gamificacion;
using Recompensa = Parque.Dominio.Gamificacion.Recompensa;

namespace Parque.Aplicacion.Servicios.Recompensas;

public interface IServicioRecompensa
{
    public Recompensa CrearRecompensa(RecompensaDto recompensa);
    Recompensa ActualizarRecompensa(Guid id, RecompensaDto dto);
    List<Recompensa> ObtenerRecompensas();
    Recompensa ObtenerRecompensaPorId(Guid id);
    HistorialCanje CanjearRecompensa(CanjearRecompensaRequest request);
    List<HistorialCanjeDto> ObtenerHistorialCanjes(Guid visitanteId);
}
