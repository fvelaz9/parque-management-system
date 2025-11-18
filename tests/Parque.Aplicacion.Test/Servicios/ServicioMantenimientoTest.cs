using System.Linq.Expressions;
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
            _mockRepoIncidencias.Object);
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
            .Returns(null as AtraccionParque);

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
            .Returns([]);
        _mockRepoAtracciones.Setup(r => r.Encontrar(It.IsAny<Expression<Func<AtraccionParque, bool>>>()))
            .Returns(atraccion);

        _servicio.EliminarMantenimiento(1);

        _mockRepoMantenimientos.Verify(r => r.Eliminar(It.IsAny<Expression<Func<MantenimientoPreventivo, bool>>>()), Times.Once);
        _mockRepoAtracciones.Verify(r => r.Editar(It.Is<AtraccionParque>(a => a.Estado == EstadoAtraccion.Disponible)), Times.Once);
    }

    [TestMethod]
    public void ListarMantenimientos_RetornaListaCorrecta()
    {
        var lista = new List<MantenimientoPreventivo>
        {
            new MantenimientoPreventivo { Id = 1, AtraccionId = 1, Descripcion = "Mantenimiento 1" },
            new MantenimientoPreventivo { Id = 2, AtraccionId = 2, Descripcion = "Mantenimiento 2" }
        };

        var atracciones = new List<AtraccionParque>
        {
            new AtraccionParque("Atraccion1", TipoAtraccion.MontañaRusa, 10, 20, "desc") { Id = 1 },
            new AtraccionParque("Atraccion2", TipoAtraccion.Simulador, 8, 15, "desc") { Id = 2 }
        };

        _mockRepoMantenimientos.Setup(r => r.ObtenerTodos()).Returns(lista);
        _mockRepoAtracciones.Setup(r => r.ObtenerTodos()).Returns(atracciones);

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

    [TestMethod]
    public void CrearMantenimiento_HorariosSolapados_LanzaArgumentException()
    {
        // Arrange
        var fechaActual = DateTime.Now;
        _mockServicioFechaHora.Setup(s => s.ObtenerFechaActual()).Returns(fechaActual);

        var atraccion = new AtraccionParque("Atraccion1", TipoAtraccion.MontañaRusa, 10, 20, "desc") { Id = 1 };
        _mockRepoAtracciones.Setup(r => r.Encontrar(It.IsAny<Expression<Func<AtraccionParque, bool>>>()))
            .Returns(atraccion);

        // Mantenimiento existente de 14:00 a 16:00
        var mantenimientoExistente = new MantenimientoPreventivo
        {
            Id = 1,
            AtraccionId = 1,
            FechaProgramada = fechaActual.AddDays(1).Date,
            HoraInicio = TimeSpan.FromHours(14),
            DuracionEstimada = TimeSpan.FromHours(2)
        };

        _mockRepoMantenimientos.Setup(r => r.ObtenerTodos())
            .Returns([mantenimientoExistente]);

        // Nuevo mantenimiento de 15:00 a 17:00 (se solapa)
        var request = new CrearMantenimientoRequest
        {
            AtraccionId = 1,
            FechaProgramada = fechaActual.AddDays(1).Date,
            HoraInicio = TimeSpan.FromHours(15),
            DuracionEstimada = TimeSpan.FromHours(2),
            Descripcion = "Mantenimiento nuevo"
        };

        // Act & Assert
        var ex = Assert.ThrowsException<ArgumentException>(() => _servicio.CrearMantenimiento(request));
        Assert.IsTrue(ex.Message.Contains("Ya existe un mantenimiento programado"));
    }

    [TestMethod]
    public void CrearMantenimiento_HorariosSinSolapamiento_CreaExitosamente()
    {
        // Arrange
        var fechaActual = DateTime.Now;
        _mockServicioFechaHora.Setup(s => s.ObtenerFechaActual()).Returns(fechaActual);

        var atraccion = new AtraccionParque("Atraccion1", TipoAtraccion.MontañaRusa, 10, 20, "desc") { Id = 1 };
        _mockRepoAtracciones.Setup(r => r.Encontrar(It.IsAny<Expression<Func<AtraccionParque, bool>>>()))
            .Returns(atraccion);

        // Mantenimiento existente de 14:00 a 16:00
        var mantenimientoExistente = new MantenimientoPreventivo
        {
            Id = 1,
            AtraccionId = 1,
            FechaProgramada = fechaActual.AddDays(1).Date,
            HoraInicio = TimeSpan.FromHours(14),
            DuracionEstimada = TimeSpan.FromHours(2)
        };

        _mockRepoMantenimientos.Setup(r => r.ObtenerTodos())
            .Returns([mantenimientoExistente]);

        _mockRepoIncidencias.Setup(r => r.Agregar(It.IsAny<Incidencia>()))
            .Callback<Incidencia>(i => i.Id = 2);

        _mockRepoMantenimientos.Setup(r => r.Agregar(It.IsAny<MantenimientoPreventivo>()))
            .Callback<MantenimientoPreventivo>(m => m.Id = 2);

        // Nuevo mantenimiento de 17:00 a 19:00 (NO se solapa)
        var request = new CrearMantenimientoRequest
        {
            AtraccionId = 1,
            FechaProgramada = fechaActual.AddDays(1).Date,
            HoraInicio = TimeSpan.FromHours(17),
            DuracionEstimada = TimeSpan.FromHours(2),
            Descripcion = "Mantenimiento sin solapamiento"
        };

        // Act
        var resultado = _servicio.CrearMantenimiento(request);

        // Assert
        Assert.IsNotNull(resultado);
        Assert.AreEqual(1, resultado.AtraccionId);
        _mockRepoMantenimientos.Verify(r => r.Agregar(It.IsAny<MantenimientoPreventivo>()), Times.Once);
    }

    [TestMethod]
    public void CrearMantenimiento_SolapamientoCompleto_LanzaArgumentException()
    {
        // Arrange
        var fechaActual = DateTime.Now;
        _mockServicioFechaHora.Setup(s => s.ObtenerFechaActual()).Returns(fechaActual);

        var atraccion = new AtraccionParque("Atraccion1", TipoAtraccion.MontañaRusa, 10, 20, "desc") { Id = 1 };
        _mockRepoAtracciones.Setup(r => r.Encontrar(It.IsAny<Expression<Func<AtraccionParque, bool>>>()))
            .Returns(atraccion);

        // Mantenimiento existente de 14:00 a 16:00
        var mantenimientoExistente = new MantenimientoPreventivo
        {
            Id = 1,
            AtraccionId = 1,
            FechaProgramada = fechaActual.AddDays(1).Date,
            HoraInicio = TimeSpan.FromHours(14),
            DuracionEstimada = TimeSpan.FromHours(2)
        };

        _mockRepoMantenimientos.Setup(r => r.ObtenerTodos())
            .Returns([mantenimientoExistente]);

        // Nuevo mantenimiento de 13:00 a 17:00 (contiene completamente al existente)
        var request = new CrearMantenimientoRequest
        {
            AtraccionId = 1,
            FechaProgramada = fechaActual.AddDays(1).Date,
            HoraInicio = TimeSpan.FromHours(13),
            DuracionEstimada = TimeSpan.FromHours(4),
            Descripcion = "Mantenimiento que contiene al existente"
        };

        // Act & Assert
        var ex = Assert.ThrowsException<ArgumentException>(() => _servicio.CrearMantenimiento(request));
        Assert.IsTrue(ex.Message.Contains("Ya existe un mantenimiento programado"));
    }

    [TestMethod]
    public void CrearMantenimiento_DiferenteAtraccion_NoValidaSolapamiento()
    {
        // Arrange
        var fechaActual = DateTime.Now;
        _mockServicioFechaHora.Setup(s => s.ObtenerFechaActual()).Returns(fechaActual);

        var atraccion1 = new AtraccionParque("Atraccion1", TipoAtraccion.MontañaRusa, 10, 20, "desc") { Id = 1 };
        var atraccion2 = new AtraccionParque("Atraccion2", TipoAtraccion.Simulador, 8, 15, "desc") { Id = 2 };

        _mockRepoAtracciones.Setup(r => r.Encontrar(It.IsAny<Expression<Func<AtraccionParque, bool>>>()))
            .Returns<Expression<Func<AtraccionParque, bool>>>(expr =>
            {
                var compiled = expr.Compile();
                return compiled(atraccion1) ? atraccion1 : compiled(atraccion2) ? atraccion2 : null;
            });

        // Mantenimiento existente en atraccion 1
        var mantenimientoExistente = new MantenimientoPreventivo
        {
            Id = 1,
            AtraccionId = 1,
            FechaProgramada = fechaActual.AddDays(1).Date,
            HoraInicio = TimeSpan.FromHours(14),
            DuracionEstimada = TimeSpan.FromHours(2)
        };

        _mockRepoMantenimientos.Setup(r => r.ObtenerTodos())
            .Returns([mantenimientoExistente]);

        _mockRepoIncidencias.Setup(r => r.Agregar(It.IsAny<Incidencia>()))
            .Callback<Incidencia>(i => i.Id = 2);

        _mockRepoMantenimientos.Setup(r => r.Agregar(It.IsAny<MantenimientoPreventivo>()))
            .Callback<MantenimientoPreventivo>(m => m.Id = 2);

        // Nuevo mantenimiento en atraccion 2 (mismo horario pero diferente atracción)
        var request = new CrearMantenimientoRequest
        {
            AtraccionId = 2,
            FechaProgramada = fechaActual.AddDays(1).Date,
            HoraInicio = TimeSpan.FromHours(14),
            DuracionEstimada = TimeSpan.FromHours(2),
            Descripcion = "Mantenimiento en diferente atracción"
        };

        // Act
        var resultado = _servicio.CrearMantenimiento(request);

        // Assert
        Assert.IsNotNull(resultado);
        Assert.AreEqual(2, resultado.AtraccionId);
        _mockRepoMantenimientos.Verify(r => r.Agregar(It.IsAny<MantenimientoPreventivo>()), Times.Once);
    }

    // ========== TESTS PARA ACTUALIZAR MANTENIMIENTO ==========

    [TestMethod]
    public void ActualizarMantenimiento_MantenimientoNoExiste_LanzaArgumentException()
    {
        var fechaActual = DateTime.Now;
        _mockServicioFechaHora.Setup(s => s.ObtenerFechaActual()).Returns(fechaActual);
        var atraccion = new AtraccionParque("Atraccion1", TipoAtraccion.MontañaRusa, 10, 20, "desc") { Id = 1 };
        _mockRepoAtracciones.Setup(r => r.Encontrar(It.IsAny<Expression<Func<AtraccionParque, bool>>>()))
            .Returns(atraccion);
        _mockRepoMantenimientos.Setup(r => r.Encontrar(It.IsAny<Expression<Func<MantenimientoPreventivo, bool>>>()))
            .Returns((MantenimientoPreventivo?)null);
        var request = new CrearMantenimientoRequest
        {
            AtraccionId = 1,
            FechaProgramada = fechaActual.AddDays(1),
            HoraInicio = TimeSpan.FromHours(14),
            DuracionEstimada = TimeSpan.FromHours(2),
            Descripcion = "Actualización"
        };
        var ex = Assert.ThrowsException<ArgumentException>(() => _servicio.ActualizarMantenimiento(999, request));
        Assert.AreEqual("Mantenimiento no encontrado", ex.Message);
    }

    [TestMethod]
    public void ActualizarMantenimiento_DatosValidos_ActualizaCorrectamente()
    {
        // Arrange
        var fechaActual = DateTime.Now;
        _mockServicioFechaHora.Setup(s => s.ObtenerFechaActual()).Returns(fechaActual);

        var atraccion = new AtraccionParque("Atraccion1", TipoAtraccion.MontañaRusa, 10, 20, "desc") { Id = 1 };
        _mockRepoAtracciones.Setup(r => r.Encontrar(It.IsAny<Expression<Func<AtraccionParque, bool>>>()))
            .Returns(atraccion);

        var mantenimientoExistente = new MantenimientoPreventivo
        {
            Id = 1,
            AtraccionId = 1,
            FechaProgramada = fechaActual.AddDays(1).Date,
            HoraInicio = TimeSpan.FromHours(10),
            DuracionEstimada = TimeSpan.FromHours(2),
            Descripcion = "Descripción antigua",
            IncidenciaId = 1
        };

        _mockRepoMantenimientos.Setup(r => r.Encontrar(It.IsAny<Expression<Func<MantenimientoPreventivo, bool>>>()))
            .Returns(mantenimientoExistente);

        _mockRepoMantenimientos.Setup(r => r.ObtenerTodos())
            .Returns([mantenimientoExistente]);

        var request = new CrearMantenimientoRequest
        {
            AtraccionId = 1,
            FechaProgramada = fechaActual.AddDays(2).Date,
            HoraInicio = TimeSpan.FromHours(15),
            DuracionEstimada = TimeSpan.FromHours(3),
            Descripcion = "Descripción actualizada"
        };

        // Act
        var resultado = _servicio.ActualizarMantenimiento(1, request);

        // Assert
        Assert.IsNotNull(resultado);
        Assert.AreEqual("Descripción actualizada", resultado.Descripcion);
        Assert.AreEqual(TimeSpan.FromHours(15), resultado.HoraInicio);
        Assert.AreEqual(TimeSpan.FromHours(3), resultado.DuracionEstimada);
        _mockRepoMantenimientos.Verify(r => r.Editar(It.IsAny<MantenimientoPreventivo>()), Times.Once);
    }

    [TestMethod]
    public void ActualizarMantenimiento_SolapamientoConOtro_LanzaArgumentException()
    {
        // Arrange
        var fechaActual = DateTime.Now;
        _mockServicioFechaHora.Setup(s => s.ObtenerFechaActual()).Returns(fechaActual);

        var atraccion = new AtraccionParque("Atraccion1", TipoAtraccion.MontañaRusa, 10, 20, "desc") { Id = 1 };
        _mockRepoAtracciones.Setup(r => r.Encontrar(It.IsAny<Expression<Func<AtraccionParque, bool>>>()))
            .Returns(atraccion);

        var mantenimientoActual = new MantenimientoPreventivo
        {
            Id = 1,
            AtraccionId = 1,
            FechaProgramada = fechaActual.AddDays(1).Date,
            HoraInicio = TimeSpan.FromHours(10),
            DuracionEstimada = TimeSpan.FromHours(2)
        };

        var otroMantenimiento = new MantenimientoPreventivo
        {
            Id = 2,
            AtraccionId = 1,
            FechaProgramada = fechaActual.AddDays(1).Date,
            HoraInicio = TimeSpan.FromHours(14),
            DuracionEstimada = TimeSpan.FromHours(2)
        };

        _mockRepoMantenimientos.Setup(r => r.Encontrar(It.IsAny<Expression<Func<MantenimientoPreventivo, bool>>>()))
            .Returns(mantenimientoActual);

        _mockRepoMantenimientos.Setup(r => r.ObtenerTodos())
            .Returns([mantenimientoActual, otroMantenimiento]);

        // Intentar actualizar para que se solape con el otro
        var request = new CrearMantenimientoRequest
        {
            AtraccionId = 1,
            FechaProgramada = fechaActual.AddDays(1).Date,
            HoraInicio = TimeSpan.FromHours(15), // Se solapará con el de 14:00-16:00
            DuracionEstimada = TimeSpan.FromHours(2),
            Descripcion = "Actualización con solapamiento"
        };

        // Act & Assert
        var ex = Assert.ThrowsException<ArgumentException>(() => _servicio.ActualizarMantenimiento(1, request));
        Assert.IsTrue(ex.Message.Contains("Ya existe un mantenimiento programado"));
    }

    // ========== TEST PARA VERIFICAR QUE SE CAMBIA ESTADO DE ATRACCIÓN ==========

    [TestMethod]
    public void CrearMantenimiento_CambiaEstadoAtraccionAFueraDeServicio()
    {
        // Arrange
        var fechaActual = DateTime.Now;
        _mockServicioFechaHora.Setup(s => s.ObtenerFechaActual()).Returns(fechaActual);

        var atraccion = new AtraccionParque("Atraccion1", TipoAtraccion.MontañaRusa, 10, 20, "desc") { Id = 1 };
        atraccion.Estado = EstadoAtraccion.Disponible;

        _mockRepoAtracciones.Setup(r => r.Encontrar(It.IsAny<Expression<Func<AtraccionParque, bool>>>()))
            .Returns(atraccion);

        _mockRepoMantenimientos.Setup(r => r.ObtenerTodos())
            .Returns([]);

        _mockRepoIncidencias.Setup(r => r.Agregar(It.IsAny<Incidencia>()))
            .Callback<Incidencia>(i => i.Id = 1);

        _mockRepoMantenimientos.Setup(r => r.Agregar(It.IsAny<MantenimientoPreventivo>()))
            .Callback<MantenimientoPreventivo>(m => m.Id = 1);

        var request = new CrearMantenimientoRequest
        {
            AtraccionId = 1,
            FechaProgramada = fechaActual.AddDays(1),
            HoraInicio = TimeSpan.FromHours(14),
            DuracionEstimada = TimeSpan.FromHours(2),
            Descripcion = "Mantenimiento preventivo"
        };

        // Act
        var resultado = _servicio.CrearMantenimiento(request);

        // Assert
        Assert.AreEqual(EstadoAtraccion.FueraDeServicio, atraccion.Estado);
        _mockRepoIncidencias.Verify(r => r.Agregar(It.IsAny<Incidencia>()), Times.Once);
        _mockRepoMantenimientos.Verify(r => r.Agregar(It.IsAny<MantenimientoPreventivo>()), Times.Once);
    }
}
