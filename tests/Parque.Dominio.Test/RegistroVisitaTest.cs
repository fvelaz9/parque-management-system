using Parque.Dominio.Atracciones;

namespace Parque.Dominio.Test;

[TestClass]
public class RegistroVisitaTest
{
    [TestMethod]
    public void Constructor_WithValidParameters_ShouldCreateInstance()
    {
        var atraccionId = 1;
        var identificador = Guid.NewGuid();
        var fechaIngreso = DateTime.Now;
        var registro = new RegistroVisita(atraccionId, identificador, fechaIngreso);
        Assert.AreEqual(atraccionId, registro.AtraccionId);
        Assert.AreEqual(identificador, registro.Identificador);
        Assert.AreEqual(fechaIngreso, registro.FechaIngreso);
        Assert.IsNull(registro.FechaEgreso);
        Assert.AreEqual(0, registro.Id);
    }

    [TestMethod]
    public void Constructor_WithMinimumValidData_ShouldCreateInstance()
    {
        var atraccionId = 1;
        var identificador = Guid.NewGuid();
        var fechaIngreso = DateTime.Now;
        var registro = new RegistroVisita(atraccionId, identificador, fechaIngreso);
        Assert.IsNotNull(registro);
        Assert.AreEqual(atraccionId, registro.AtraccionId);
        Assert.AreEqual(identificador, registro.Identificador);
    }

    [TestMethod]
    public void FechaEgreso_WhenSet_ShouldStoreValue()
    {
        var registro = new RegistroVisita(1, Guid.NewGuid(), DateTime.Now);
        var fechaEgreso = DateTime.Now.AddHours(1);
        registro.FechaEgreso = fechaEgreso;
        Assert.AreEqual(fechaEgreso, registro.FechaEgreso);
    }

    [TestMethod]
    public void FechaEgreso_WhenNotSet_ShouldBeNull()
    {
        var registro = new RegistroVisita(1, Guid.NewGuid(), DateTime.Now);
        Assert.IsNull(registro.FechaEgreso);
    }

    [TestMethod]
    public void Id_WhenSet_ShouldStoreValue()
    {
        var registro = new RegistroVisita(1, Guid.NewGuid(), DateTime.Now);
        registro.Id = 100;
        Assert.AreEqual(100, registro.Id);
    }

    [TestMethod]
    public void AtraccionId_WhenSet_ShouldStoreValue()
    {
        var registro = new RegistroVisita(1, Guid.NewGuid(), DateTime.Now);
        registro.AtraccionId = 2;
        Assert.AreEqual(2, registro.AtraccionId);
    }

    [TestMethod]
    public void Identificador_WhenSet_ShouldStoreValue()
    {
        var registro = new RegistroVisita(1, Guid.NewGuid(), DateTime.Now);
        var nuevoIdentificador = Guid.NewGuid();
        registro.Identificador = nuevoIdentificador;
        Assert.AreEqual(nuevoIdentificador, registro.Identificador);
    }

    [TestMethod]
    public void FechaIngreso_WhenSet_ShouldStoreValue()
    {
        var registro = new RegistroVisita(1, Guid.NewGuid(), DateTime.Now);
        var nuevaFechaIngreso = DateTime.Now.AddDays(1);
        registro.FechaIngreso = nuevaFechaIngreso;
        Assert.AreEqual(nuevaFechaIngreso, registro.FechaIngreso);
    }

    [TestMethod]
    public void Constructor_WithEmptyGuid_ShouldCreateInstance()
    {
        var registro = new RegistroVisita(1, Guid.Empty, DateTime.Now);
        Assert.AreEqual(Guid.Empty, registro.Identificador);
    }

    [TestMethod]
    public void Constructor_WithFutureFechaIngreso_ShouldCreateInstance()
    {
        var fechaFutura = DateTime.Now.AddDays(1);
        var registro = new RegistroVisita(1, Guid.NewGuid(), fechaFutura);
        Assert.AreEqual(fechaFutura, registro.FechaIngreso);
    }

    [TestMethod]
    public void Constructor_WithPastFechaIngreso_ShouldCreateInstance()
    {
        var fechaPasada = DateTime.Now.AddDays(-1);
        var registro = new RegistroVisita(1, Guid.NewGuid(), fechaPasada);
        Assert.AreEqual(fechaPasada, registro.FechaIngreso);
    }
}
