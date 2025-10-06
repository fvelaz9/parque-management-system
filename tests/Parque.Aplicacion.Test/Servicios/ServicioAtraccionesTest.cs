using System.Linq.Expressions;
using Moq;
using Parque.Dominio.Atracciones;
using Parque.Infraestructura.Repositorios;

namespace Parque.Aplicacion.Test.Servicios;

[TestClass]
public class ServicioAtraccionesTest
{
    private readonly Mock<IRepositorio<AtraccionParque>> _mockRepo;
    private readonly Mock<IRepositorio<RegistroVisita>> _mockRepoRegistro;
    private readonly ServicioAtracciones _servicio;

    public ServicioAtraccionesTest()
    {
        _mockRepo = new Mock<IRepositorio<AtraccionParque>>();
        _mockRepoRegistro = new Mock<IRepositorio<RegistroVisita>>();
        _servicio = new ServicioAtracciones(_mockRepo.Object, _mockRepoRegistro.Object);
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
}
