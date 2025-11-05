using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Parque.Aplicacion.DTOs;
using Parque.Aplicacion.Servicios;
using Parque.Aplicacion.Servicios.Mantenimiento;
using Parque.Dominio;
using Parque.Dominio.Atracciones;
using Parque.Infraestructura.Repositorios;

namespace Parque.Aplicacion.Test.Servicios;

[TestClass]
public class ServicioMantenimientoTest
{
    private readonly Mock<IRepositorio<MantenimientoPreventivo>> _mockRepoMantenimientos;
    private readonly Mock<IRepositorio<Incidencia>> _mockRepoIncidencias;
    private readonly Mock<IRepositorio<AtraccionParque>> _mockRepoAtracciones;
    private readonly Mock<IServicioFechaHora> _mockServicioFechaHora;
    private readonly IServicioMantenimiento _servicio;

    public ServicioMantenimientoTest()
    {
        _mockRepoMantenimientos = new Mock<IRepositorio<MantenimientoPreventivo>>();
        _mockRepoIncidencias = new Mock<IRepositorio<Incidencia>>();
        _mockRepoAtracciones = new Mock<IRepositorio<AtraccionParque>>();
        _mockServicioFechaHora = new Mock<IServicioFechaHora>();
        _servicio = new ServicioMantenimiento(
            _mockRepoMantenimientos.Object,
            _mockRepoAtracciones.Object,
            _mockServicioFechaHora.Object,
            _mockRepoIncidencias.Object
            );
    }

    [TestMethod]
    public void CrearMantenimiento_DescripcionVacia_LanzaArgumentException()
    {
        var request = new CrearMantenimientoRequest
        {
            AtraccionId = 1,
            FechaProgramada = DateTime.Now.AddDays(1),
            HoraInicio = TimeSpan.FromHours(14),
            DuracionEstimada = TimeSpan.FromHours(2),
            Descripcion = " "
        };

        var ex = Assert.ThrowsException<ArgumentException>(() => _servicio.CrearMantenimiento(request));
        Assert.AreEqual("La descripción del mantenimiento es requerida", ex.Message);
    }

    [TestMethod]
    public void CrearMantenimiento_AtraccionNoExiste_LanzaArgumentException()
    {
        _mockRepoAtracciones.Setup(r => r.Encontrar(It.IsAny<Expression<Func<AtraccionParque, bool>>>()))
            .Returns((AtraccionParque)null);

        var request = new CrearMantenimientoRequest
        {
            AtraccionId = 999,
            FechaProgramada = DateTime.Now.AddDays(1),
            HoraInicio = TimeSpan.FromHours(14),
            DuracionEstimada = TimeSpan.FromHours(2),
            Descripcion = "Mantenimiento valid"
        };

        var ex = Assert.ThrowsException<ArgumentException>(() => _servicio.CrearMantenimiento(request));
        Assert.AreEqual("Atracción no encontrada", ex.Message);
    }

    [TestMethod]
    public void CrearMantenimiento_FechaInicioEnPasado_LanzaArgumentException()
    {
        var fechaActual = DateTime.Now;
        _mockServicioFechaHora.Setup(s => s.ObtenerFechaActual()).Returns(fechaActual);

        var atraccion = new AtraccionParque("Atraccion1", TipoAtraccion.MontañaRusa, 10, 20, "desc") { Id = 1 };
        _mockRepoAtracciones.Setup(r => r.Encontrar(It.IsAny<Expression<Func<AtraccionParque, bool>>>()))
            .Returns(atraccion);

        var request = new CrearMantenimientoRequest
        {
            AtraccionId = 1,
            FechaProgramada = fechaActual.AddDays(-1), // en pasado
            HoraInicio = TimeSpan.Zero,
            DuracionEstimada = TimeSpan.FromHours(2),
            Descripcion = "Mantenimiento valid"
        };

        var ex = Assert.ThrowsException<ArgumentException>(() => _servicio.CrearMantenimiento(request));
        Assert.AreEqual("La fecha y hora de inicio del mantenimiento debe ser futura", ex.Message);
    }

    [TestMethod]
    public void CrearMantenimiento_DatosValidos_CreaMantenimientoEIncidencia()
    {
        var fechaActual = DateTime.Now;
        _mockServicioFechaHora.Setup(s => s.ObtenerFechaActual()).Returns(fechaActual);

        var atraccion = new AtraccionParque("Atraccion1", TipoAtraccion.MontañaRusa, 10, 20, "desc") { Id = 1 };
        _mockRepoAtracciones.Setup(r => r.Encontrar(It.IsAny<Expression<Func<AtraccionParque, bool>>>()))
            .Returns(atraccion);

        Incidencia incidenciaCapturada = null;
        _mockRepoIncidencias.Setup(r => r.Agregar(It.IsAny<Incidencia>()))
            .Callback<Incidencia>(i => { i.Id = 1; incidenciaCapturada = i; });

        MantenimientoPreventivo mantenimientoCapturado = null;
        _mockRepoMantenimientos.Setup(r => r.Agregar(It.IsAny<MantenimientoPreventivo>()))
            .Callback<MantenimientoPreventivo>(m => { m.Id = 1; mantenimientoCapturado = m; });

        var request = new CrearMantenimientoRequest
        {
            AtraccionId = 1,
            FechaProgramada = fechaActual.AddDays(1),
            HoraInicio = TimeSpan.FromHours(14),
            DuracionEstimada = TimeSpan.FromHours(2),
            Descripcion = "Prueba mantenimiento"
        };

        var resultado = _servicio.CrearMantenimiento(request);

        Assert.IsNotNull(resultado);
        Assert.AreEqual(1, resultado.AtraccionId);
        Assert.AreEqual("Prueba mantenimiento", resultado.Descripcion);
        Assert.AreEqual(incidenciaCapturada.Id, resultado.IncidenciaId);
        _mockRepoIncidencias.Verify(r => r.Agregar(It.IsAny<Incidencia>()), Times.Once);
        _mockRepoMantenimientos.Verify(r => r.Agregar(It.IsAny<MantenimientoPreventivo>()), Times.Once);
    }

    [TestMethod]
    public void EliminarMantenimiento_NoExiste_LanzaExcepcion()
    {
        _mockRepoMantenimientos.Setup(r => r.Encontrar(It.IsAny<Expression<Func<MantenimientoPreventivo, bool>>>()))
            .Returns((MantenimientoPreventivo?)null);

        Assert.ThrowsException<ArgumentException>(() => _servicio.EliminarMantenimiento(1));
    }

    [TestMethod]
    public void EliminarMantenimiento_Existe_EliminaYActualizaEstado()
    {
        var mantenimiento = new MantenimientoPreventivo
        {
            Id = 1,
            AtraccionId = 1,
            IncidenciaId = 1
        };
        var atraccion = new AtraccionParque("Atraccion", TipoAtraccion.MontañaRusa, 12, 50, "desc") { Id = 1 };

        _mockRepoMantenimientos.Setup(r => r.Encontrar(It.IsAny<Expression<Func<MantenimientoPreventivo, bool>>>()))
            .Returns(mantenimiento);
        _mockRepoIncidencias.Setup(r => r.ObtenerTodos())
            .Returns(new List<Incidencia>());
        _mockRepoAtracciones.Setup(r => r.Encontrar(It.IsAny<Expression<Func<AtraccionParque, bool>>>()))
            .Returns(atraccion);

        _servicio.EliminarMantenimiento(1);

        _mockRepoIncidencias.Verify(r => r.Eliminar(It.IsAny<Expression<Func<Incidencia, bool>>>()), Times.Once);
        _mockRepoMantenimientos.Verify(r => r.Eliminar(It.IsAny<Expression<Func<MantenimientoPreventivo, bool>>>()), Times.Once);
        _mockRepoAtracciones.Verify(r => r.Editar(It.Is<AtraccionParque>(a => a.Estado == EstadoAtraccion.Disponible)), Times.Once);
    }

    [TestMethod]
    public void ListarMantenimientos_RetornaListaCorrecta()
    {
        var lista = new List<MantenimientoPreventivo>
        {
            new MantenimientoPreventivo { Id = 1, Descripcion = "Mantenimiento 1" },
            new MantenimientoPreventivo { Id = 2, Descripcion = "Mantenimiento 2" }
        };

        _mockRepoMantenimientos.Setup(r => r.ObtenerTodos()).Returns(lista);

        var resultado = _servicio.ListarMantenimientos();

        Assert.IsNotNull(resultado);
        Assert.AreEqual(2, resultado.Count());
        Assert.AreEqual("Mantenimiento 1", resultado.ElementAt(0).Descripcion);
        Assert.AreEqual("Mantenimiento 2", resultado.ElementAt(1).Descripcion);

        _mockRepoMantenimientos.Verify(r => r.ObtenerTodos(), Times.Once);
    }

    [TestMethod]
    public void ListarMantenimientos_Vacia_RetornaListaVacia()
    {
        // Arrange
        _mockRepoMantenimientos.Setup(r => r.ObtenerTodos()).Returns(Enumerable.Empty<MantenimientoPreventivo>().ToList());

        // Act
        var resultados = _servicio.ListarMantenimientos();

        // Assert
        Assert.IsNotNull(resultados);
        Assert.AreEqual(0, resultados.Count());
        _mockRepoMantenimientos.Verify(r => r.ObtenerTodos(), Times.Once);
    }
}
