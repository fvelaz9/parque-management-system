using System.Linq.Expressions;
using Moq;
using Parque.Aplicacion.DTOs;
using Parque.Aplicacion.Servicios;
using Parque.Aplicacion.Servicios.Incidencias;
using Parque.Dominio;
using Parque.Dominio.Atracciones;
using Parque.Dominio.Excepciones;
using Parque.Infraestructura.Repositorios;

namespace Parque.Aplicacion.Test.Servicios;

[TestClass]
public class ServicioIncidenciasTest
{
    private readonly Mock<IRepositorio<Incidencia>> _mockRepoIncidencias;
    private readonly Mock<IRepositorio<AtraccionParque>> _mockRepoAtracciones;
    private readonly Mock<IServicioFechaHora> _mockServicioFechaHora;
    private readonly ServicioIncidencia _servicio;

    public ServicioIncidenciasTest()
    {
        _mockRepoIncidencias = new Mock<IRepositorio<Incidencia>>();
        _mockRepoAtracciones = new Mock<IRepositorio<AtraccionParque>>();
        _mockServicioFechaHora = new Mock<IServicioFechaHora>();
        _servicio = new ServicioIncidencia(_mockRepoIncidencias.Object, _mockRepoAtracciones.Object, _mockServicioFechaHora.Object);
    }

    [TestMethod]
    public void CrearIncidencia_AtraccionExiste_CreaIncidenciaYCambiaEstado()
    {
        var atraccion = new AtraccionParque("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 24, "Test") { Id = 1 };
        var request = new CrearIncidenciaRequest
        {
            AtraccionId = 1,
            Descripcion = "Falla en motor",
            FechaResolucionEstimada = DateTime.Now.AddDays(2)
        };

        _mockRepoAtracciones.Setup(r => r.Encontrar(It.IsAny<Expression<Func<AtraccionParque, bool>>>()))
            .Returns(atraccion);

        var resultado = _servicio.CrearIncidencia(request);

        Assert.IsNotNull(resultado);
        Assert.AreEqual("Falla en motor", resultado.Descripcion);
        Assert.AreEqual(EstadoAtraccion.FueraDeServicio, atraccion.Estado);
        _mockRepoIncidencias.Verify(r => r.Agregar(It.IsAny<Incidencia>()), Times.Once);
        _mockRepoAtracciones.Verify(r => r.Editar(atraccion), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CrearIncidencia_AtraccionNoExiste_LanzaExcepcion()
    {
        var request = new CrearIncidenciaRequest
        {
            AtraccionId = 99,
            Descripcion = "Falla",
            FechaResolucionEstimada = DateTime.Now.AddDays(1)
        };

        _mockRepoAtracciones.Setup(r => r.Encontrar(It.IsAny<Expression<Func<AtraccionParque, bool>>>()))
            .Returns((AtraccionParque?)null);

        _servicio.CrearIncidencia(request);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CrearIncidencia_FechaResolucionPasada_LanzaExcepcion()
    {
        var fechaActual = new DateTime(2025, 10, 8, 12, 0, 0);
        var atraccion = new AtraccionParque("Carrusel", TipoAtraccion.Simulador, 0, 30, "Test") { Id = 1 };
        var request = new CrearIncidenciaRequest
        {
            AtraccionId = 1,
            Descripcion = "Falla",
            FechaResolucionEstimada = fechaActual.AddDays(-1)
        };

        _mockServicioFechaHora.Setup(s => s.ObtenerFechaActual()).Returns(fechaActual);
        _mockRepoAtracciones.Setup(r => r.Encontrar(It.IsAny<Expression<Func<AtraccionParque, bool>>>()))
            .Returns(atraccion);

        _servicio.CrearIncidencia(request);
    }

    [TestMethod]
    public void EstaDisponible_SinIncidenciaActiva_RetornaTrueYCambiaEstado()
    {
        var atraccion = new AtraccionParque("Simulador", TipoAtraccion.Simulador, 8, 12, "Test") { Id = 1 };
        var incidencias = new List<Incidencia>();

        _mockRepoAtracciones.Setup(r => r.Encontrar(It.IsAny<Expression<Func<AtraccionParque, bool>>>()))
            .Returns(atraccion);
        _mockRepoIncidencias.Setup(r => r.ObtenerTodos()).Returns(incidencias);

        var resultado = _servicio.EstaDisponible(1);

        Assert.IsTrue(resultado);
        Assert.AreEqual(EstadoAtraccion.Disponible, atraccion.Estado);
        _mockRepoAtracciones.Verify(r => r.Editar(atraccion), Times.Once);
    }

    [TestMethod]
    public void EstaDisponible_ConIncidenciaActiva_RetornaFalseYCambiaEstado()
    {
        var atraccion = new AtraccionParque("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 24, "Test") { Id = 1 };
        var incidenciaActiva = new Incidencia
        {
            AtraccionId = 1,
            FechaReporte = DateTime.Now.AddDays(-1),
            FechaResolucionEstimada = DateTime.Now.AddDays(1),
            Descripcion = "Falla activa"
        };
        var incidencias = new List<Incidencia> { incidenciaActiva };

        _mockRepoAtracciones.Setup(r => r.Encontrar(It.IsAny<Expression<Func<AtraccionParque, bool>>>()))
            .Returns(atraccion);
        _mockRepoIncidencias.Setup(r => r.ObtenerTodos()).Returns(incidencias);

        var resultado = _servicio.EstaDisponible(1);

        Assert.IsFalse(resultado);
        Assert.AreEqual(EstadoAtraccion.FueraDeServicio, atraccion.Estado);
        _mockRepoAtracciones.Verify(r => r.Editar(atraccion), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void EstaDisponible_AtraccionNoExiste_LanzaExcepcion()
    {
        _mockRepoAtracciones.Setup(r => r.Encontrar(It.IsAny<Expression<Func<AtraccionParque, bool>>>()))
            .Returns((AtraccionParque?)null);

        _servicio.EstaDisponible(99);
    }

    [TestMethod]
    public void ResolverIncidencia_IncidenciaExiste_EliminaIncidenciaYCambiaEstadoAtraccion()
    {
        // Arrange
        var fechaActual = new DateTime(2025, 10, 8, 12, 0, 0);
        var atraccion = new AtraccionParque("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 24, "Test")
        {
            Id = 1,
            Estado = EstadoAtraccion.FueraDeServicio
        };
        var incidencia = new Incidencia
        {
            Id = 1,
            AtraccionId = 1,
            Descripcion = "Falla en motor",
            FechaReporte = fechaActual.AddDays(-1),
            FechaResolucionEstimada = fechaActual.AddDays(1)
        };
        var incidencias = new List<Incidencia>();

        _mockServicioFechaHora.Setup(s => s.ObtenerFechaActual()).Returns(fechaActual);
        _mockRepoIncidencias.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Incidencia, bool>>>()))
            .Returns(incidencia);
        _mockRepoIncidencias.Setup(r => r.Eliminar(It.IsAny<Expression<Func<Incidencia, bool>>>()));
        _mockRepoAtracciones.Setup(r => r.Encontrar(It.IsAny<Expression<Func<AtraccionParque, bool>>>()))
            .Returns(atraccion);
        _mockRepoIncidencias.Setup(r => r.ObtenerTodos()).Returns(incidencias);
        _mockRepoAtracciones.Setup(r => r.Editar(It.IsAny<AtraccionParque>()));

        // Act
        _servicio.ResolverIncidencia(1);

        // Assert
        _mockRepoIncidencias.Verify(r => r.Eliminar(It.IsAny<Expression<Func<Incidencia, bool>>>()), Times.Once);
        _mockRepoAtracciones.Verify(r => r.Editar(It.Is<AtraccionParque>(
            a => a.Estado == EstadoAtraccion.Disponible)), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(ExcepcionEntidadNoEncontrada))]
    public void ResolverIncidencia_IncidenciaNoExiste_LanzaExcepcion()
    {
        // Arrange
        _mockRepoIncidencias.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Incidencia, bool>>>()))
            .Returns((Incidencia?)null);

        // Act
        _servicio.ResolverIncidencia(99);
    }
}
