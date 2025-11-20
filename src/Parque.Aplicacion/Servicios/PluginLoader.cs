using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Parque.Dominio.Gamificacion;

namespace Parque.Aplicacion.Servicios;
[ExcludeFromCodeCoverage]
public class PluginLoader
{
    private readonly string _rutaPlugins;
    private readonly ILogger<PluginLoader> _logger;

    public PluginLoader(ILogger<PluginLoader> logger, string rutaPlugins = "Plugins")
    {
        _logger = logger;
        _rutaPlugins = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, rutaPlugins);

        if(!Directory.Exists(_rutaPlugins))
        {
            Directory.CreateDirectory(_rutaPlugins);
            _logger.LogInformation("Carpeta de plugins creada en: {RutaPlugins}", _rutaPlugins);
        }
    }

    public List<IEstrategiaPuntuacion> CargarEstrategiasDesdePlugins()
    {
        var estrategias = new List<IEstrategiaPuntuacion>();

        _logger.LogInformation("Buscando plugins en: {RutaPlugins}", _rutaPlugins);

        if(!Directory.Exists(_rutaPlugins))
        {
            _logger.LogWarning("Carpeta de plugins no existe");
            return estrategias;
        }

        var archivos = Directory.GetFiles(_rutaPlugins, "*.dll");
        _logger.LogInformation("Archivos .dll encontrados: {Count}", archivos.Length);

        foreach(var archivo in archivos)
        {
            try
            {
                var nombreArchivo = Path.GetFileName(archivo);
                _logger.LogDebug("Intentando cargar: {NombreArchivo}", nombreArchivo);

                var assembly = Assembly.LoadFrom(archivo);

                var tiposEstrategia = assembly.GetTypes()
                    .Where(t => typeof(IEstrategiaPuntuacion).IsAssignableFrom(t)
                             && !t.IsInterface
                             && !t.IsAbstract
                             && t.IsClass)
                    .ToList();

                if(tiposEstrategia.Count == 0)
                {
                    _logger.LogWarning("No se encontraron estrategias en {NombreArchivo}", nombreArchivo);
                    continue;
                }

                foreach(var tipo in tiposEstrategia)
                {
                    try
                    {
                        var instancia = Activator.CreateInstance(tipo) as IEstrategiaPuntuacion;

                        if(instancia != null)
                        {
                            estrategias.Add(instancia);
                            _logger.LogInformation("Plugin cargado: {NombreEstrategia}", instancia.Nombre);
                        }
                    }
                    catch(Exception ex)
                    {
                        _logger.LogError(ex, "Error al instanciar {TipoNombre}", tipo.Name);
                    }
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error al cargar {NombreArchivo}", Path.GetFileName(archivo));
            }
        }

        _logger.LogInformation("Total de plugins cargados: {Count}", estrategias.Count);
        return estrategias;
    }
}
