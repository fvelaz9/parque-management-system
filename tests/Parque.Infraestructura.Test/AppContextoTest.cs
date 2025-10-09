using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;

namespace Parque.Infraestructura.test;

[TestClass]
public class AppContextoTest
{
    private AppContexto? _contexto;

    [TestInitialize]
    public void Initialize()
    {
        var options = new DbContextOptionsBuilder<AppContexto>().Options;
        _contexto = new AppContexto(options);
    }

    [TestMethod]
    public void Cuentas_Get_RetornaDbSet()
    {
        var property = typeof(AppContexto).GetProperty("Cuentas");
        var result = property!.GetValue(_contexto);

        Assert.IsNotNull(result);
    }

    [TestMethod]
    public void Visitantes_Get_RetornaDbSet()
    {
        var property = typeof(AppContexto).GetProperty("Visitantes");
        var result = property!.GetValue(_contexto);

        Assert.IsNotNull(result);
    }

    [TestMethod]
    public void Eventos_Get_RetornaDbSet()
    {
        var property = typeof(AppContexto).GetProperty("Eventos");
        var result = property!.GetValue(_contexto);

        Assert.IsNotNull(result);
    }

    [TestMethod]
    public void Tickets_Get_RetornaDbSet()
    {
        var property = typeof(AppContexto).GetProperty("Tickets");
        var result = property!.GetValue(_contexto);

        Assert.IsNotNull(result);
    }

    [TestMethod]
    public void Atracciones_Get_RetornaDbSet()
    {
        var property = typeof(AppContexto).GetProperty("Atracciones");
        var result = property!.GetValue(_contexto);

        Assert.IsNotNull(result);
    }

    [TestMethod]
    public void RegistrosVisitas_Get_RetornaDbSet()
    {
        var property = typeof(AppContexto).GetProperty("RegistrosVisitas");
        var result = property!.GetValue(_contexto);

        Assert.IsNotNull(result);
    }

    [TestMethod]
    public void Sesiones_Get_RetornaDbSet()
    {
        var property = typeof(AppContexto).GetProperty("Sesiones");
        var result = property!.GetValue(_contexto);

        Assert.IsNotNull(result);
    }

    [TestMethod]
    public void PuntuacionesVisitantes_Get_RetornaDbSet()
    {
        var property = typeof(AppContexto).GetProperty("PuntuacionesVisitantes");
        var result = property!.GetValue(_contexto);

        Assert.IsNotNull(result);
    }

    [TestMethod]
    public void ConfiguracionesEstrategia_Get_RetornaDbSet()
    {
        var property = typeof(AppContexto).GetProperty("ConfiguracionesEstrategia");
        var result = property!.GetValue(_contexto);

        Assert.IsNotNull(result);
    }

    [TestCleanup]
    public void Cleanup()
    {
        _contexto?.Dispose();
    }
}
