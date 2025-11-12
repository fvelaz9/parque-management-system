using Parque.Aplicacion.DTOs.RecompensasDtos;
using Parque.Infraestructura.Migrations;

namespace Parque.Aplicacion.Servicios.Recompensas;

public interface IServicioRecompensa
{
    public Recompensa CrearRecompensa(RecompensaDto recompensa);
}
