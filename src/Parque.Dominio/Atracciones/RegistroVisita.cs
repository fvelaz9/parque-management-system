namespace Parque.Dominio.Atracciones;

public class RegistroVisita
{
    public int Id { get; set; }
    public int AtraccionId { get; set; }
    public Guid Identificador { get; set; } // QR del ticket o NFC de la pulsera
    public DateTime FechaIngreso { get; set; }
    public DateTime? FechaEgreso { get; set; }
}
