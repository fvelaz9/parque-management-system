namespace Parque.Dominio.Gamificacion;

public class ConfiguracionEstrategia
{
    public int Id { get; set; }
    public string EstrategiaActiva { get; set; } = string.Empty;
    public DateTime FechaModificacion { get; set; }
    
    public ConfiguracionEstrategia() 
    {
        FechaModificacion = DateTime.UtcNow;
    }
    
    public ConfiguracionEstrategia(string estrategiaInicial)
    {
        if(string.IsNullOrWhiteSpace(estrategiaInicial))
        {
            throw new ArgumentException("La estrategia inicial no puede estar vacía", nameof(estrategiaInicial));
        }

        EstrategiaActiva = estrategiaInicial;
    }

    public void CambiarEstrategia(string nombreEstrategia)
    {
        if(string.IsNullOrWhiteSpace(nombreEstrategia))
        {
            throw new ArgumentException("El nombre de la estrategia no puede estar vacío", nameof(nombreEstrategia));
        }

        EstrategiaActiva = nombreEstrategia;
        FechaModificacion = DateTime.UtcNow;
    }
}
