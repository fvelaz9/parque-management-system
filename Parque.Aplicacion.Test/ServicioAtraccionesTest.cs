using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Parque.Aplicacion.Servicios.Atracciones;
using Parque.Dominio.Atracciones;
using Parque.Infraestructura.Repositorios;
using Xunit;

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

        [Fact]
        public void CrearAtraccion_CapacidadValida_CreaAtraccion()
        {
            // Arrange
            string nombre = "Montaña Rusa";
            var tipo = TipoAtraccion.Simulador;
            int edadMinima = 12;
            int capacidad = 20;
            string descripcion = "Atracción rápida";

            // Act
            var atraccion = _servicio.CrearAtraccion(nombre, tipo, edadMinima, capacidad, descripcion);

            // Assert
            Assert.NotNull(atraccion);
            Assert.Equal(nombre, atraccion.Nombre);
            _mockRepo.Verify(r => r.Agregar(It.IsAny<AtraccionParque>()), Times.Once);
        }

        [Fact]
        public void CrearAtraccion_CapacidadInvalida_LanzaExcepcion()
        {
            // Act + Assert
            Assert.Throws<ArgumentException>(() =>
                _servicio.CrearAtraccion("Tobogán", TipoAtraccion.Familiar, 5, 0, "No debería funcionar"));
        }

        [Fact]
        public void ListarAtracciones_DevuelveTodas()
        {
            // Arrange
            var lista = new List<AtraccionParque>
            {
                new AtraccionParque("Montaña Rusa", TipoAtraccion.Adrenalina, 12, 20, "Rápida"),
                new AtraccionParque("Carrusel", TipoAtraccion.Familiar, 0, 30, "Clásico")
            };
            _mockRepo.Setup(r => r.ObtenerTodos()).Returns(lista);

            // Act
            var resultado = _servicio.ListarAtracciones();

            // Assert
            Assert.Equal(2, resultado.Count());
        }

        [Fact]
        public void BuscarAtraccion_Existe_RetornaAtraccion()
        {
            // Arrange
            var atraccion = new AtraccionParque("Carrusel", TipoAtraccion.Familiar, 0, 30, "Clásico") { Id = 1 };
            _mockRepo.Setup(r => r.Encontrar(It.IsAny<Func<AtraccionParque, bool>>())).Returns(atraccion);

            // Act
            var resultado = _servicio.BuscarAtraccion(1);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(1, resultado.Id);
        }

        [Fact]
        public void BuscarAtraccion_NoExiste_RetornaNull()
        {
            // Arrange
            _mockRepo.Setup(r => r.Encontrar(It.IsAny<Func<AtraccionParque, bool>>())).Returns((AtraccionParque?)null);

            // Act
            var resultado = _servicio.BuscarAtraccion(99);

            // Assert
            Assert.Null(resultado);
        }

        [Fact]
        public void ModificarAtraccion_Existe_ModificaCorrectamente()
        {
            // Arrange
            var atraccion = new AtraccionParque("Carrusel", TipoAtraccion.Familiar, 0, 30, "Clásico") { Id = 1 };
            _mockRepo.Setup(r => r.Encontrar(It.IsAny<Func<AtraccionParque, bool>>())).Returns(atraccion);

            // Act
            _servicio.ModificarAtraccion(1, "Nuevo Carrusel", TipoAtraccion.Familiar, 0, 25, "Actualizado");

            // Assert
            Assert.Equal("Nuevo Carrusel", atraccion.Nombre);
            Assert.Equal(25, atraccion.Capacidad);
            _mockRepo.Verify(r => r.Editar(It.IsAny<AtraccionParque>()), Times.Once);
        }

        [Fact]
        public void ModificarAtraccion_NoExiste_LanzaExcepcion()
        {
            // Arrange
            _mockRepo.Setup(r => r.Encontrar(It.IsAny<Func<AtraccionParque, bool>>())).Returns((AtraccionParque?)null);

            // Act + Assert
            Assert.Throws<ArgumentException>(() =>
                _servicio.ModificarAtraccion(99, "Fantasma", TipoAtraccion.Adrenalina, 10, 10, "No existe"));
        }

        [Fact]
        public void EliminarAtraccion_InvocaRepositorio()
        {
            // Act
            _servicio.EliminarAtraccion(1);

            // Assert
            _mockRepo.Verify(r => r.Eliminar(It.IsAny<Func<AtraccionParque, bool>>()), Times.Once);
        }

        public class FactAttribute : Attribute
        {
        }
}


