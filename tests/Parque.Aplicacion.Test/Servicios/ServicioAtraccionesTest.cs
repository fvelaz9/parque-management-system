using System.Linq.Expressions;
using Moq;
using Parque.Dominio.Atracciones;
using Parque.Infraestructura.Repositorios;

namespace Parque.Aplicacion.Test.Servicios;

[TestClass]
public class ServicioAtraccionesTest
{
    private readonly Mock<IRepositorio<AtraccionParque>> _mockRepo;
    private readonly Mock<IRepositorio<RegistroVisita>> _mockRepoRegistros;
    private readonly ServicioAtracciones _servicio;

    public ServicioAtraccionesTest()
    {
        _mockRepo = new Mock<IRepositorio<AtraccionParque>>();
        _mockRepoRegistros = new Mock<IRepositorio<RegistroVisita>>();
        _servicio = new ServicioAtracciones(_mockRepo.Object, _mockRepoRegistros.Object);
    }

    [TestMethod]
    public void CrearAtraccion_CapacidadValida_CreaAtraccion()
    {
        var nombre = "Montaña Rusa";
        var tipo = TipoAtraccion.Simulador;
        var edadMinima = 12;
        var capacidad = 20;
        var descripcion = "Atracción rápida";
        var atraccion = _servicio.CrearAtraccion(nombre, tipo, edadMinima, capacidad, descripcion);
        Assert.IsNotNull(atraccion);
        Assert.AreEqual(nombre, atraccion.Nombre);
        _mockRepo.Verify(r => r.Agregar(It.IsAny<AtraccionParque>()), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CrearAtraccion_CapacidadInvalida_LanzaExcepcion()
    {
        _servicio.CrearAtraccion("Tobogán", TipoAtraccion.Simulador, 5, 0, "No debería funcionar");
    }

    [TestMethod]
    public void ListarAtracciones_DevuelveTodas()
    {
        var lista = new List<AtraccionParque>
        {
            new AtraccionParque("Montaña Rusa", TipoAtraccion.Simulador, 12, 20, "Rápida"),
            new AtraccionParque("Carrusel", TipoAtraccion.Simulador, 0, 30, "Clásico")
        };
        _mockRepo.Setup(r => r.ObtenerTodos()).Returns(lista);
        var resultado = _servicio.ListarAtracciones();
        Assert.AreEqual(2, resultado.Count());
    }

    [TestMethod]
    public void BuscarAtraccion_Existe_RetornaAtraccion()
    {
        var atraccion = new AtraccionParque("Carrusel", TipoAtraccion.Simulador, 0, 30, "Clásico") { Id = 1 };
        _mockRepo.Setup(r => r.Encontrar(It.IsAny<Expression<Func<AtraccionParque, bool>>>())).Returns(atraccion);
        var resultado = _servicio.BuscarAtraccion(1);
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

        _mockRepo.Setup(r => r.Encontrar(It.Is<Expression<Func<AtraccionParque, bool>>>(expr =>
            expr.ToString().Contains("Id"))))
            .Returns(atraccion);

        _mockRepo.Setup(r => r.Encontrar(It.Is<Expression<Func<AtraccionParque, bool>>>(expr =>
            expr.ToString().Contains("Nombre"))))
            .Returns((AtraccionParque?)null);

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

    [TestMethod]
    public void ObtenerAforoActual_AtraccionExiste_DeberiaRetornarAforoCorrectamente()
    {
        // Arrange
        var atraccion = new AtraccionParque("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 24, "Test");
        var registros = new List<RegistroVisita>
        {
            new() { AtraccionId = 1, FechaIngreso = DateTime.Now, FechaEgreso = null },
            new() { AtraccionId = 1, FechaIngreso = DateTime.Now, FechaEgreso = null },
            new() { AtraccionId = 1, FechaIngreso = DateTime.Now, FechaEgreso = DateTime.Now },
            new() { AtraccionId = 2, FechaIngreso = DateTime.Now, FechaEgreso = null }
        };

        _mockRepo.Setup(r => r.Encontrar(It.IsAny<Expression<Func<AtraccionParque, bool>>>()))
            .Returns(atraccion);
        _mockRepoRegistros.Setup(r => r.ObtenerTodos()).Returns(registros);

        // Act
        var resultado = _servicio.ObtenerAforoActual(1);

        // Assert
        Assert.AreEqual("Montaña Rusa", resultado.NombreAtraccion);
        Assert.AreEqual(2, resultado.AforoActual);
        Assert.AreEqual(24, resultado.CapacidadMaxima);
        Assert.AreEqual(22, resultado.Disponible);
    }

    [TestMethod]
    public void ObtenerAforoActual_SinVisitantes_DeberiaRetornarAforoCero()
    {
        // Arrange
        var atraccion = new AtraccionParque("Simulador VR", TipoAtraccion.Simulador, 8, 12, "Test");
        _mockRepo.Setup(r => r.Encontrar(It.IsAny<Expression<Func<AtraccionParque, bool>>>()))
            .Returns(atraccion);
        _mockRepoRegistros.Setup(r => r.ObtenerTodos()).Returns([]);

        // Act
        var resultado = _servicio.ObtenerAforoActual(1);

        // Assert
        Assert.AreEqual(0, resultado.AforoActual);
        Assert.AreEqual(12, resultado.Disponible);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void ObtenerAforoActual_AtraccionNoExiste_LanzaExcepcion()
    {
        _mockRepo.Setup(r => r.Encontrar(It.IsAny<Expression<Func<AtraccionParque, bool>>>()))
            .Returns((AtraccionParque?)null);

        _servicio.ObtenerAforoActual(99);
    }

    [TestMethod]
    public void ObtenerReporteUso_ConRegistros_RetornaReporteOrdenado()
    {
        var fechaInicio = new DateTime(2025, 1, 1);
        var fechaFin = new DateTime(2025, 12, 31);

        var atraccion1 = new AtraccionParque("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 24, "Test") { Id = 1 };
        var atraccion2 = new AtraccionParque("Carrusel", TipoAtraccion.Simulador, 0, 30, "Test") { Id = 2 };

        var registros = new List<RegistroVisita>
        {
            new() { AtraccionId = 1, FechaIngreso = new DateTime(2025, 6, 1), FechaEgreso = new DateTime(2025, 6, 1, 1, 0, 0) },
            new() { AtraccionId = 1, FechaIngreso = new DateTime(2025, 6, 2), FechaEgreso = new DateTime(2025, 6, 2, 1, 0, 0) },
            new() { AtraccionId = 3, FechaIngreso = new DateTime(2025, 6, 3), FechaEgreso = new DateTime(2025, 6, 3, 1, 0, 0) }
        };

        _mockRepoRegistros.Setup(r => r.ObtenerTodos()).Returns(registros);
        _mockRepo.Setup(r => r.Encontrar(It.Is<Expression<Func<AtraccionParque, bool>>>(expr => true)))
            .Returns<Expression<Func<AtraccionParque, bool>>>(expr =>
            {
                var func = expr.Compile();
                if(func(atraccion1))
                {
                    return atraccion1;
                }

                if(func(atraccion2))
                {
                    return atraccion2;
                }

                return null;
            });

        var resultado = _servicio.ObtenerReporteUso(fechaInicio, fechaFin);

        Assert.AreEqual(2, resultado.Count);
        Assert.AreEqual("Montaña Rusa", resultado[0].NombreAtraccion);
        Assert.AreEqual("Desconocida", resultado[1].NombreAtraccion);
    }

    [TestMethod]
    public void ObtenerPorIds_IdsValidos_RetornaAtraccionesCorrespondientes()
    {
        // Arrange
        var atracciones = new List<AtraccionParque>
        {
            new AtraccionParque("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 24, "Rápida") { Id = 1 },
            new AtraccionParque("Carrusel", TipoAtraccion.Simulador, 0, 30, "Clásico") { Id = 2 },
            new AtraccionParque("Simulador VR", TipoAtraccion.Simulador, 8, 12, "Virtual") { Id = 3 },
            new AtraccionParque("Rueda", TipoAtraccion.Simulador, 0, 40, "Familiar") { Id = 4 }
        };

        _mockRepo.Setup(r => r.ObtenerTodos()).Returns(atracciones);

        var idsABuscar = new List<int> { 1, 3 };

        // Act
        var resultado = _servicio.ObtenerPorIds(idsABuscar);

        // Assert
        Assert.AreEqual(2, resultado.Count());
        Assert.IsTrue(resultado.Any(a => a.Id == 1 && a.Nombre == "Montaña Rusa"));
        Assert.IsTrue(resultado.Any(a => a.Id == 3 && a.Nombre == "Simulador VR"));
        Assert.IsFalse(resultado.Any(a => a.Id == 2 || a.Id == 4));
    }
}
