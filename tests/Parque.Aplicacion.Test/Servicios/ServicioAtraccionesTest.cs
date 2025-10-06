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

    [TestMethod]
    public void RegistrarIngreso_AtraccionExiste_DeberiaCrearRegistro()
    {
        // Arrange
        var atraccion = new AtraccionParque("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 24, "Test");
        var guid = Guid.NewGuid();

        _mockRepo.Setup(r => r.Encontrar(It.IsAny<Expression<Func<AtraccionParque, bool>>>()))
            .Returns(atraccion);

        // Act
        var resultado = _servicio.RegistrarIngreso(guid, 1);

        // Assert
        Assert.IsNotNull(resultado);
        Assert.AreEqual(guid, resultado.Identificador);
        Assert.AreEqual(1, resultado.AtraccionId);
        Assert.IsNotNull(resultado.FechaIngreso);
        Assert.IsNull(resultado.FechaEgreso);
        _mockRepoRegistros.Verify(r => r.Agregar(It.IsAny<RegistroVisita>()), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void RegistrarIngreso_AtraccionNoExiste_DeberiaLanzarExcepcion()
    {
        // Arrange
        _mockRepo.Setup(r => r.Encontrar(It.IsAny<Expression<Func<AtraccionParque, bool>>>()))
            .Returns((AtraccionParque?)null);

        // Act
        _servicio.RegistrarIngreso(Guid.NewGuid(), 999);
    }

    [TestMethod]
    public void RegistrarEgreso_IngresoExisteSinEgreso_DeberiaActualizarFechaEgreso()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var registro = new RegistroVisita
        {
            Id = 1,
            AtraccionId = 1,
            Identificador = guid,
            FechaIngreso = DateTime.Now.AddHours(-2),
            FechaEgreso = null
        };

        _mockRepoRegistros.Setup(r => r.Encontrar(It.IsAny<Expression<Func<RegistroVisita, bool>>>()))
            .Returns(registro);

        // Act
        var resultado = _servicio.RegistrarEgreso(guid, 1);

        // Assert
        Assert.IsNotNull(resultado.FechaEgreso);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void RegistrarEgreso_NoHayIngresoRegistrado_DeberiaLanzarExcepcion()
    {
        // Arrange
        _mockRepoRegistros.Setup(r => r.Encontrar(It.IsAny<Expression<Func<RegistroVisita, bool>>>()))
            .Returns((RegistroVisita?)null);

        // Act
        _servicio.RegistrarEgreso(Guid.NewGuid(), 1);
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
}
