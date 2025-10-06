using Microsoft.VisualStudio.TestTools.UnitTesting;
using Parque.Dominio.Atracciones;
using System;

namespace Parque.Dominio.Test.Atracciones;

[TestClass]
public class RegistroVisitaTest
{
    [TestMethod]
    public void Constructor_WithValidParameters_ShouldCreateInstance()
    {
        // Arrange
        var atraccionId = 1;
        var identificador = Guid.NewGuid();
        var fechaIngreso = DateTime.Now;

        // Act
        var registro = new RegistroVisita(atraccionId, identificador, fechaIngreso);

        // Assert
        Assert.AreEqual(atraccionId, registro.AtraccionId);
        Assert.AreEqual(identificador, registro.Identificador);
        Assert.AreEqual(fechaIngreso, registro.FechaIngreso);
        Assert.IsNull(registro.FechaEgreso);
        Assert.AreEqual(0, registro.Id); // Id debería ser 0 por defecto
    }

    [TestMethod]
    public void Constructor_WithMinimumValidData_ShouldCreateInstance()
    {
        // Arrange
        var atraccionId = 1;
        var identificador = Guid.NewGuid();
        var fechaIngreso = DateTime.Now;

        // Act
        var registro = new RegistroVisita(atraccionId, identificador, fechaIngreso);

        // Assert
        Assert.IsNotNull(registro);
        Assert.AreEqual(atraccionId, registro.AtraccionId);
        Assert.AreEqual(identificador, registro.Identificador);
    }

    [TestMethod]
    public void FechaEgreso_WhenSet_ShouldStoreValue()
    {
        // Arrange
        var registro = new RegistroVisita(1, Guid.NewGuid(), DateTime.Now);
        var fechaEgreso = DateTime.Now.AddHours(1);

        // Act
        registro.FechaEgreso = fechaEgreso;

        // Assert
        Assert.AreEqual(fechaEgreso, registro.FechaEgreso);
    }

    [TestMethod]
    public void FechaEgreso_WhenNotSet_ShouldBeNull()
    {
        // Arrange & Act
        var registro = new RegistroVisita(1, Guid.NewGuid(), DateTime.Now);

        // Assert
        Assert.IsNull(registro.FechaEgreso);
    }

    [TestMethod]
    public void Id_WhenSet_ShouldStoreValue()
    {
        // Arrange
        var registro = new RegistroVisita(1, Guid.NewGuid(), DateTime.Now);

        // Act
        registro.Id = 100;

        // Assert
        Assert.AreEqual(100, registro.Id);
    }

    [TestMethod]
    public void AtraccionId_WhenSet_ShouldStoreValue()
    {
        // Arrange
        var registro = new RegistroVisita(1, Guid.NewGuid(), DateTime.Now);

        // Act
        registro.AtraccionId = 2;

        // Assert
        Assert.AreEqual(2, registro.AtraccionId);
    }

    [TestMethod]
    public void Identificador_WhenSet_ShouldStoreValue()
    {
        // Arrange
        var registro = new RegistroVisita(1, Guid.NewGuid(), DateTime.Now);
        var nuevoIdentificador = Guid.NewGuid();

        // Act
        registro.Identificador = nuevoIdentificador;

        // Assert
        Assert.AreEqual(nuevoIdentificador, registro.Identificador);
    }

    [TestMethod]
    public void FechaIngreso_WhenSet_ShouldStoreValue()
    {
        // Arrange
        var registro = new RegistroVisita(1, Guid.NewGuid(), DateTime.Now);
        var nuevaFechaIngreso = DateTime.Now.AddDays(1);

        // Act
        registro.FechaIngreso = nuevaFechaIngreso;

        // Assert
        Assert.AreEqual(nuevaFechaIngreso, registro.FechaIngreso);
    }

    [TestMethod]
    public void Constructor_WithEmptyGuid_ShouldCreateInstance()
    {
        // Arrange & Act
        var registro = new RegistroVisita(1, Guid.Empty, DateTime.Now);

        // Assert
        Assert.AreEqual(Guid.Empty, registro.Identificador);
    }

    [TestMethod]
    public void Constructor_WithFutureFechaIngreso_ShouldCreateInstance()
    {
        // Arrange
        var fechaFutura = DateTime.Now.AddDays(1);

        // Act
        var registro = new RegistroVisita(1, Guid.NewGuid(), fechaFutura);

        // Assert
        Assert.AreEqual(fechaFutura, registro.FechaIngreso);
    }

    [TestMethod]
    public void Constructor_WithPastFechaIngreso_ShouldCreateInstance()
    {
        // Arrange
        var fechaPasada = DateTime.Now.AddDays(-1);

        // Act
        var registro = new RegistroVisita(1, Guid.NewGuid(), fechaPasada);

        // Assert
        Assert.AreEqual(fechaPasada, registro.FechaIngreso);
    }
}
