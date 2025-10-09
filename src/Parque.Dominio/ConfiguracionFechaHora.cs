namespace Parque.Dominio;

public class ConfiguracionFechaHora
{
    public int Id { get; set; }
    public DateTime FechaHoraConfigurada { get; set; }
    public ConfiguracionFechaHora()
    {
    }

    public ConfiguracionFechaHora(DateTime fechaHora)
    {
        FechaHoraConfigurada = fechaHora;
    }
}
