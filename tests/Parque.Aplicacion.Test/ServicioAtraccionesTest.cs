using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Parque.Aplicacion.Servicios.Atracciones;
using Parque.Dominio.Atracciones;
using Parque.Infraestructura.Repositorios;

namespace Parque.Aplicacion.Test;

public class ServicioAtraccionesTest
{
    private readonly Mock<IRepositorio<AtraccionParque>> _mockRepo;
    private readonly ServicioAtracciones _servicio;

    public ServicioAtraccionesTest()
    {
        _mockRepo = new Mock<IRepositorio<AtraccionParque>>();
        _servicio = new ServicioAtracciones(_mockRepo.Object);
    }

    [TestMethod]
    public void CrearAtraccion_CapacidadValida_CreaAtraccion()
    {
        // Arrange
        var nombre = "Montaña Rusa";
        var tipo = TipoAtraccion.Simulador;
        var edadMinima = 12;
        var capacidad = 20;
        var descripcion = "Atracción rápida";

        // Act
        var atraccion = _servicio.CrearAtraccion(nombre, tipo, edadMinima, capacidad, descripcion);

        // Assert
        Assert.IsNotNull(atraccion);
        Assert.AreEqual(nombre, atraccion.Nombre);
        _mockRepo.Verify(r => r.Agregar(It.IsAny<AtraccionParque>()), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CrearAtraccion_CapacidadInvalida_LanzaExcepcion()
    {
        // Act
        _servicio.CrearAtraccion("Tobogán", TipoAtraccion.Simulador, 5, 0, "No debería funcionar");
    }

    [TestMethod]
    public void ListarAtracciones_DevuelveTodas()
    {
        // Arrange
        var lista = new List<AtraccionParque>
        {
            new AtraccionParque("Montaña Rusa", TipoAtraccion.Simulador, 12, 20, "Rápida"),
            new AtraccionParque("Carrusel", TipoAtraccion.Simulador, 0, 30, "Clásico")
        };
        _mockRepo.Setup(r => r.ObtenerTodos()).Returns(lista);

        // Act
        var resultado = _servicio.ListarAtracciones();

        // Assert
        Assert.AreEqual(2, resultado.Count());
    }

    [TestMethod]
    public void BuscarAtraccion_Existe_RetornaAtraccion()
    {
        // Arrange
        var atraccion = new AtraccionParque("Carrusel", TipoAtraccion.Simulador, 0, 30, "Clásico") { Id = 1 };
        _mockRepo.Setup(r => r.Encontrar(It.IsAny<Expression<Func<AtraccionParque, bool>>>())).Returns(atraccion);

        // Act
        var resultado = _servicio.BuscarAtraccion(1);

        // Assert
        Assert.IsNotNull(resultado);
        Assert.AreEqual(1, resultado.Id);
    }

    [TestMethod]
    public void BuscarAtraccion_NoExiste_RetornaNull()
    {
        _mockRepo.Setup(r => r.Encontrar(It.IsAny<Expression<Func<AtraccionParque, bool>>>())).Returns((AtraccionParque?)null);

        var resultado = _servicio.BuscarAtraccion(99);

        Assert.IsNull(resultado);
    }

    [TestMethod]
    public void ModificarAtraccion_Existe_ModificaCorrectamente()
    {
        var atraccion = new AtraccionParque("Carrusel", TipoAtraccion.Simulador, 0, 30, "Clásico") { Id = 1 };
        _mockRepo.Setup(r => r.Encontrar(It.IsAny<Expression<Func<AtraccionParque, bool>>>())).Returns(atraccion);
        _servicio.ModificarAtraccion(1, "Nuevo Carrusel", TipoAtraccion.Simulador, 0, 25, "Actualizado");
        
        Assert.AreEqual("Nuevo Carrusel", atraccion.Nombre);
        Assert.AreEqual(25, atraccion.Capacidad);
        
        _mockRepo.Verify(r => r.Editar(It.IsAny<AtraccionParque>()), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void ModificarAtraccion_NoExiste_LanzaExcepcion()
    {
        _mockRepo.Setup(r => r.Encontrar(It.IsAny<Expression<Func<AtraccionParque, bool>>>())).Returns((AtraccionParque?)null);
        _servicio.ModificarAtraccion(99, "Fantasma", TipoAtraccion.Simulador, 10, 10, "No existe");
    }

    [TestMethod]
    public void EliminarAtraccion_InvocaRepositorio()
    { 
        _servicio.EliminarAtraccion(1);
        _mockRepo.Verify(r => r.Eliminar(It.IsAny<Expression<Func<AtraccionParque, bool>>>()), Times.Once);
    }
}
