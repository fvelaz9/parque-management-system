using System.Linq.Expressions;
using Moq;
using Parque.Aplicacion.DTOs.RecompensasDtos;
using Parque.Aplicacion.Servicios;
using Parque.Aplicacion.Servicios.Recompensas;
using Parque.Dominio.Gamificacion;
using Parque.Dominio.Usuarios;
using Parque.Infraestructura.Repositorios;

namespace Parque.Aplicacion.Test.Servicios;

[TestClass]
public class ServicioRecompensaTest
{
    private Mock<IRepositorio<Recompensa>> _mockRepoRecompensa = null!;
    private Mock<IRepositorio<HistorialCanje>> _mockRepoHistorial = null!;
    private Mock<IRepositorio<PuntuacionVisitante>> _mockRepoPuntuacion = null!;
    private Mock<IRepositorio<Visitante>> _mockRepoVisitante = null!;
    private Mock<IServicioFechaHora> _mockServicioFechaHora = null!;
    private ServicioRecompensa _servicio = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockRepoRecompensa = new Mock<IRepositorio<Recompensa>>();
        _mockRepoHistorial = new Mock<IRepositorio<HistorialCanje>>();
        _mockRepoPuntuacion = new Mock<IRepositorio<PuntuacionVisitante>>();
        _mockRepoVisitante = new Mock<IRepositorio<Visitante>>();
        _mockServicioFechaHora = new Mock<IServicioFechaHora>();
        _mockServicioFechaHora.Setup(s => s.ObtenerFechaActual())
            .Returns(new DateTime(2025, 11, 11, 22, 0, 0));
        _servicio = new ServicioRecompensa(_mockRepoRecompensa.Object, _mockRepoHistorial.Object, _mockRepoPuntuacion.Object, _mockRepoVisitante.Object, _mockServicioFechaHora.Object);
    }

    [TestMethod]
    public void CrearRecompensa_ConDatosValidos_DebeRetornarRecompensaConId()
    {
        // Arrange
        var dto = new RecompensaDto
        {
            Nombre = "Entrada VIP",
            Descripcion = "Acceso prioritario",
            CostoEnPuntos = 500,
            CantidadDisponible = 10,
            NivelMembresiaRequerido = NivelMembresia.Premium
        };

        // Act
        var resultado = _servicio.CrearRecompensa(dto);

        // Assert
        Assert.IsNotNull(resultado);
        Assert.AreNotEqual(Guid.Empty, resultado.Id);
        Assert.AreEqual("Entrada VIP", resultado.Nombre);
        Assert.AreEqual(500, resultado.CostoEnPuntos);
        Assert.AreEqual(10, resultado.CantidadDisponible);
        Assert.IsNotNull(resultado.FechaCreacion);
        Assert.AreEqual(new DateTime(2025, 11, 11, 22, 0, 0), resultado.FechaCreacion);
        _mockRepoRecompensa.Verify(r => r.Agregar(It.IsAny<Recompensa>()), Times.Once);
    }

    [TestMethod]
    public void CrearRecompensa_ConNombreVacio_DebeLanzarExcepcion()
    {
        // Arrange
        var dto = new RecompensaDto
        {
            Nombre = " ",
            CostoEnPuntos = 100,
            CantidadDisponible = 5
        };

        // Act & Assert
        Assert.ThrowsException<InvalidOperationException>(() => _servicio.CrearRecompensa(dto));
    }

    [TestMethod]
    public void CrearRecompensa_DescripcionLarga()
    {
        // Arrange
        var dto = new RecompensaDto
        {
            Nombre = "joseeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee" +
                     "eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee" +
                     "eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee" +
                     "eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee" +
                     "eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee" +
                     "eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee" +
                     "eeeeeeeeeeeee",
            CostoEnPuntos = 100,
            CantidadDisponible = 5
        };

        // Act & Assert
        Assert.ThrowsException<InvalidOperationException>(() => _servicio.CrearRecompensa(dto));
    }

    [TestMethod]
    public void CrearRecompensa_ConNError_DebeLanzarExcepcion()
    {
        // Arrange
        var dto = new RecompensaDto
        {
            Descripcion = "a",
            CostoEnPuntos = 3,
            CantidadDisponible = -1
        };

        // Act & Assert
        Assert.ThrowsException<InvalidOperationException>(() => _servicio.CrearRecompensa(dto));
    }

    [TestMethod]
    public void CrearRecompensa_ConCostoCero_DebeLanzarExcepcion()
    {
        // Arrange
        var dto = new RecompensaDto
        {
            Nombre = "Test",
            CostoEnPuntos = 0,
            CantidadDisponible = 5
        };

        // Act & Assert
        Assert.ThrowsException<InvalidOperationException>(() => _servicio.CrearRecompensa(dto));
    }

    [TestMethod]
    public void CrearRecompensa_ConCantidadNegativa_DebeLanzarExcepcion()
    {
        // Arrange
        var dto = new RecompensaDto
        {
            Nombre = "Test",
            CostoEnPuntos = 100,
            CantidadDisponible = -1
        };

        // Act & Assert
        Assert.ThrowsException<InvalidOperationException>(() => _servicio.CrearRecompensa(dto));
    }

    [TestMethod]
    public void ActualizarRecompensa_ConDatosValidos_DebeRetornarRecompensaActualizada()
    {
        // Arrange
        var id = Guid.NewGuid();
        var recompensaExistente = new Recompensa
        {
            Id = id,
            Nombre = "Viejo",
            Descripcion = "Descripción vieja",
            CostoEnPuntos = 100,
            CantidadDisponible = 5,
            FechaCreacion = new DateTime(2025, 1, 1)
        };

        _mockRepoRecompensa.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Recompensa, bool>>>()))
            .Returns(recompensaExistente);

        var dtoActualizado = new RecompensaDto
        {
            Nombre = "Nuevo Nombre",
            Descripcion = "Nueva descripción",
            CostoEnPuntos = 200,
            CantidadDisponible = 10,
            NivelMembresiaRequerido = NivelMembresia.Premium
        };

        // Act
        var resultado = _servicio.ActualizarRecompensa(id, dtoActualizado);

        // Assert
        Assert.IsNotNull(resultado);
        Assert.AreEqual(id, resultado.Id);
        Assert.AreEqual("Nuevo Nombre", resultado.Nombre);
        Assert.AreEqual("Nueva descripción", resultado.Descripcion);
        Assert.AreEqual(200, resultado.CostoEnPuntos);
        Assert.AreEqual(10, resultado.CantidadDisponible);
        Assert.AreEqual(NivelMembresia.Premium, resultado.NivelMembresiaRequerido);
        _mockRepoRecompensa.Verify(r => r.Editar(recompensaExistente), Times.Once);
    }

    [TestMethod]
    public void ActualizarRecompensa_ConIdInexistente_DebeLanzarExcepcion()
    {
        // Arrange
        var id = Guid.NewGuid();
        _mockRepoRecompensa.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Recompensa, bool>>>()))
            .Returns((Recompensa)null!);

        var dto = new RecompensaDto
        {
            Nombre = "Test",
            CostoEnPuntos = 100,
            CantidadDisponible = 5
        };

        // Act & Assert
        Assert.ThrowsException<InvalidOperationException>(() =>
            _servicio.ActualizarRecompensa(id, dto));
    }

    [TestMethod]
    public void ActualizarRecompensa_ConNombreVacio_DebeLanzarExcepcion()
    {
        // Arrange
        var id = Guid.NewGuid();
        var recompensaExistente = new Recompensa
        {
            Id = id,
            Nombre = "Existente",
            CostoEnPuntos = 100,
            CantidadDisponible = 5,
            FechaCreacion = DateTime.UtcNow
        };

        _mockRepoRecompensa.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Recompensa, bool>>>()))
            .Returns(recompensaExistente);

        var dto = new RecompensaDto
        {
            Nombre = " ",
            CostoEnPuntos = 100,
            CantidadDisponible = 5
        };

        // Act & Assert
        Assert.ThrowsException<InvalidOperationException>(() =>
            _servicio.ActualizarRecompensa(id, dto));
    }

    [TestMethod]
    public void ObtenerRecompensas_DebeRetornarListaDeRecompensas()
    {
        // Arrange
        var recompensas = new List<Recompensa>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Nombre = "Recompensa 1",
                Descripcion = "Desc 1",
                CostoEnPuntos = 100,
                CantidadDisponible = 5,
                FechaCreacion = DateTime.UtcNow
            },
            new()
            {
                Id = Guid.NewGuid(),
                Nombre = "Recompensa 2",
                Descripcion = "Desc 2",
                CostoEnPuntos = 200,
                CantidadDisponible = 3,
                NivelMembresiaRequerido = NivelMembresia.Premium,
                FechaCreacion = DateTime.UtcNow
            }
        };

        _mockRepoRecompensa.Setup(r => r.ObtenerTodos()).Returns(recompensas);

        // Act
        var resultado = _servicio.ObtenerRecompensas();

        // Assert
        Assert.IsNotNull(resultado);
        Assert.AreEqual(2, resultado.Count);
        Assert.AreEqual("Recompensa 1", resultado[0].Nombre);
        Assert.AreEqual("Recompensa 2", resultado[1].Nombre);
    }

    [TestMethod]
    public void ObtenerRecompensas_SinRecompensas_DebeRetornarListaVacia()
    {
        // Arrange
        _mockRepoRecompensa.Setup(r => r.ObtenerTodos()).Returns([]);

        // Act
        var resultado = _servicio.ObtenerRecompensas();

        // Assert
        Assert.IsNotNull(resultado);
        Assert.AreEqual(0, resultado.Count);
    }

    [TestMethod]
    public void ObtenerRecompensaPorId_ConIdValido_DebeRetornarRecompensa()
    {
        // Arrange
        var id = Guid.NewGuid();
        var recompensa = new Recompensa
        {
            Id = id,
            Nombre = "Test Recompensa",
            Descripcion = "Descripción test",
            CostoEnPuntos = 150,
            CantidadDisponible = 10,
            NivelMembresiaRequerido = NivelMembresia.VIP,
            FechaCreacion = DateTime.UtcNow
        };

        _mockRepoRecompensa.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Recompensa, bool>>>()))
            .Returns(recompensa);

        // Act
        var resultado = _servicio.ObtenerRecompensaPorId(id);

        // Assert
        Assert.IsNotNull(resultado);
        Assert.AreEqual(id, resultado.Id);
        Assert.AreEqual("Test Recompensa", resultado.Nombre);
        Assert.AreEqual(150, resultado.CostoEnPuntos);
    }

    [TestMethod]
    public void ObtenerRecompensaPorId_ConIdInexistente_DebeLanzarExcepcion()
    {
        // Arrange
        var id = Guid.NewGuid();
        _mockRepoRecompensa.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Recompensa, bool>>>()))
            .Returns((Recompensa)null!);

        // Act & Assert
        Assert.ThrowsException<InvalidOperationException>(() =>
            _servicio.ObtenerRecompensaPorId(id));
    }

    [TestMethod]
    public void CanjearRecompensa_ConDatosValidos_DebeRetornarHistorialCanje()
    {
        // Arrange
        var recompensaId = Guid.NewGuid();
        var visitanteId = Guid.NewGuid();

        var visitante = Visitante.Crear(new DateTime(1990, 1, 1));
        visitante.Id = visitanteId;
        visitante.AsignarMembresia(NivelMembresia.Premium);

        var recompensa = new Recompensa
        {
            Id = recompensaId,
            Nombre = "Premio Test",
            CostoEnPuntos = 100,
            CantidadDisponible = 5,
            NivelMembresiaRequerido = NivelMembresia.Estandar,
            FechaCreacion = new DateTime(2025, 11, 10)
        };

        var fechaMock = new DateTime(2025, 11, 11);

        var puntuaciones = new List<PuntuacionVisitante>
        {
            new PuntuacionVisitante(visitanteId, fechaMock.AddDays(-1), 0) { PuntosTotales = 60 },
            new PuntuacionVisitante(visitanteId, fechaMock, 0) { PuntosTotales = 50 }
        };

        _mockRepoVisitante.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Visitante, bool>>>()))
            .Returns(visitante);
        _mockRepoRecompensa.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Recompensa, bool>>>()))
            .Returns(recompensa);
        _mockRepoPuntuacion.Setup(r => r.ObtenerTodos())
            .Returns(puntuaciones);
        _mockRepoHistorial.Setup(r => r.Agregar(It.IsAny<HistorialCanje>()));

        var request = new CanjearRecompensaRequest
        {
            VisitanteId = visitanteId,
            RecompensaId = recompensaId
        };

        // Act
        var resultado = _servicio.CanjearRecompensa(request);

        // Assert
        Assert.IsNotNull(resultado);
        Assert.AreEqual(visitanteId, resultado.VisitanteId);
        Assert.AreEqual(recompensaId, resultado.RecompensaId);
        Assert.AreEqual(100, resultado.PuntosCanjeados);
        Assert.AreEqual(4, recompensa.CantidadDisponible); // Debe haber decrementado

        _mockRepoPuntuacion.Verify(r => r.Editar(It.IsAny<PuntuacionVisitante>()), Times.AtLeastOnce);
        _mockRepoHistorial.Verify(r => r.Agregar(It.IsAny<HistorialCanje>()), Times.Once);
    }

    [TestMethod]
    public void CanjearRecompensa_ConPuntosInsuficientes_DebeLanzarExcepcion()
    {
        var recompensaId = Guid.NewGuid();
        var visitanteId = Guid.NewGuid();

        var visitante = Visitante.Crear(new DateTime(1990, 1, 1));
        visitante.AsignarMembresia(NivelMembresia.Estandar);

        var recompensa = new Recompensa
        {
            Id = recompensaId,
            Nombre = "Premio Caro",
            CostoEnPuntos = 1000,
            CantidadDisponible = 5,
            FechaCreacion = new DateTime(2025, 11, 11)
        };

        var fechaMock = new DateTime(2025, 11, 11);

        var puntuaciones = new List<PuntuacionVisitante>
        {
            new PuntuacionVisitante(visitanteId, fechaMock.AddDays(-1), 0) { PuntosTotales = 400 },
            new PuntuacionVisitante(visitanteId, fechaMock, 0) { PuntosTotales = 50 }
        };

        _mockRepoVisitante.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Visitante, bool>>>()))
            .Returns(visitante);
        _mockRepoRecompensa.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Recompensa, bool>>>()))
            .Returns(recompensa);
        _mockRepoPuntuacion.Setup(r => r.ObtenerTodos())
            .Returns(puntuaciones);
        _mockServicioFechaHora.Setup(s => s.ObtenerFechaActual())
            .Returns(fechaMock);

        var request = new CanjearRecompensaRequest { VisitanteId = visitanteId, RecompensaId = recompensaId };

        var ex = Assert.ThrowsException<InvalidOperationException>(() => _servicio.CanjearRecompensa(request));
        Assert.AreEqual("Puntos insuficientes para canjear esta recompensa", ex.Message);
    }

    // ✅ NUEVO TEST: Sin puntos registrados
    [TestMethod]
    public void CanjearRecompensa_SinPuntosRegistrados_DebeLanzarExcepcion()
    {
        // Arrange
        var recompensaId = Guid.NewGuid();
        var visitanteId = Guid.NewGuid();

        var visitante = Visitante.Crear(new DateTime(1990, 1, 1));
        visitante.AsignarMembresia(NivelMembresia.Estandar);

        var recompensa = new Recompensa
        {
            Id = recompensaId,
            Nombre = "Premio Test",
            CostoEnPuntos = 100,
            CantidadDisponible = 5,
            FechaCreacion = DateTime.UtcNow
        };

        // ✅ Lista vacía de puntuaciones
        var puntuaciones = new List<PuntuacionVisitante>();

        _mockRepoVisitante.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Visitante, bool>>>()))
            .Returns(visitante);
        _mockRepoRecompensa.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Recompensa, bool>>>()))
            .Returns(recompensa);
        _mockRepoPuntuacion.Setup(r => r.ObtenerTodos())
            .Returns(puntuaciones);

        var request = new CanjearRecompensaRequest { VisitanteId = visitanteId, RecompensaId = recompensaId };

        // Act & Assert
        var ex = Assert.ThrowsException<InvalidOperationException>(() => _servicio.CanjearRecompensa(request));
        Assert.AreEqual("No hay puntos registrados para este visitante", ex.Message);
    }

    // ✅ NUEVO TEST: Descuento distribuido en múltiples registros
    [TestMethod]
    public void CanjearRecompensa_DescuentoDistribuidoEnMultiplesRegistros_DebeActualizarCorrectamente()
    {
        var recompensaId = Guid.NewGuid();
        var visitanteId = Guid.NewGuid();

        var visitante = Visitante.Crear(new DateTime(1990, 1, 1));
        visitante.AsignarMembresia(NivelMembresia.Estandar);

        var recompensa = new Recompensa
        {
            Id = recompensaId,
            Nombre = "Premio Test",
            CostoEnPuntos = 150,
            CantidadDisponible = 5,
            FechaCreacion = new DateTime(2025, 11, 11)
        };

        var fechaMock = new DateTime(2025, 11, 11);

        var puntuaciones = new List<PuntuacionVisitante>
        {
            new PuntuacionVisitante(visitanteId, fechaMock.AddDays(-2), 0) { PuntosTotales = 80 },
            new PuntuacionVisitante(visitanteId, fechaMock.AddDays(-1), 0) { PuntosTotales = 50 },
            new PuntuacionVisitante(visitanteId, fechaMock, 0) { PuntosTotales = 40 }
        };

        _mockRepoVisitante.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Visitante, bool>>>()))
            .Returns(visitante);
        _mockRepoRecompensa.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Recompensa, bool>>>()))
            .Returns(recompensa);
        _mockRepoPuntuacion.Setup(r => r.ObtenerTodos())
            .Returns(puntuaciones);
        _mockServicioFechaHora.Setup(s => s.ObtenerFechaActual())
            .Returns(fechaMock);

        var request = new CanjearRecompensaRequest { VisitanteId = visitanteId, RecompensaId = recompensaId };

        var resultado = _servicio.CanjearRecompensa(request);

        Assert.IsNotNull(resultado);
        Assert.AreEqual(150, resultado.PuntosCanjeados);

        _mockRepoPuntuacion.Verify(r => r.Editar(It.IsAny<PuntuacionVisitante>()), Times.AtLeast(2));
    }

    [TestMethod]
    public void CanjearRecompensa_ConNivelInsuficiente_DebeLanzarExcepcion()
    {
        // Arrange
        var recompensaId = Guid.NewGuid();
        var visitanteId = Guid.NewGuid();

        var visitante = Visitante.Crear(new DateTime(1990, 1, 1));
        visitante.AsignarMembresia(NivelMembresia.Estandar);

        var recompensa = new Recompensa
        {
            Id = recompensaId,
            Nombre = "Premio VIP",
            CostoEnPuntos = 100,
            CantidadDisponible = 5,
            NivelMembresiaRequerido = NivelMembresia.VIP, // Requiere VIP
            FechaCreacion = DateTime.UtcNow
        };

        var puntuaciones = new List<PuntuacionVisitante>
    {
        new PuntuacionVisitante(visitanteId, DateTime.UtcNow.Date, 0) { PuntosTotales = 500 }
    };

        _mockRepoVisitante.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Visitante, bool>>>()))
            .Returns(visitante);
        _mockRepoRecompensa.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Recompensa, bool>>>()))
            .Returns(recompensa);
        _mockRepoPuntuacion.Setup(r => r.ObtenerTodos())
            .Returns(puntuaciones);

        var request = new CanjearRecompensaRequest { VisitanteId = visitanteId, RecompensaId = recompensaId };

        // Act & Assert
        var ex = Assert.ThrowsException<InvalidOperationException>(() => _servicio.CanjearRecompensa(request));
        Assert.AreEqual("Nivel de membresía insuficiente para canjear esta recompensa", ex.Message);
    }

    [TestMethod]
    public void CanjearRecompensa_ConVisitanteInexistente_DebeLanzarExcepcion()
    {
        // Arrange
        var visitanteId = Guid.NewGuid();
        var recompensaId = Guid.NewGuid();

        _mockRepoVisitante.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Visitante, bool>>>()))
            .Returns((Visitante)null!);

        var request = new CanjearRecompensaRequest { VisitanteId = visitanteId, RecompensaId = recompensaId };

        // Act & Assert
        var ex = Assert.ThrowsException<InvalidOperationException>(() => _servicio.CanjearRecompensa(request));
        Assert.AreEqual("Usuario no encontrado", ex.Message);
    }

    [TestMethod]
    public void CanjearRecompensa_ConRecompensaInexistente_DebeLanzarExcepcion()
    {
        // Arrange
        var visitanteId = Guid.NewGuid();
        var recompensaId = Guid.NewGuid();

        var visitante = Visitante.Crear(new DateTime(1990, 1, 1));
        visitante.AsignarMembresia(NivelMembresia.Estandar);

        _mockRepoVisitante.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Visitante, bool>>>()))
            .Returns(visitante);
        _mockRepoRecompensa.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Recompensa, bool>>>()))
            .Returns((Recompensa)null!);

        var request = new CanjearRecompensaRequest { VisitanteId = visitanteId, RecompensaId = recompensaId };

        // Act & Assert
        var ex = Assert.ThrowsException<InvalidOperationException>(() => _servicio.CanjearRecompensa(request));
        Assert.AreEqual($"Recompensa con ID {recompensaId} no encontrada", ex.Message);
    }

    [TestMethod]
    public void ObtenerHistorialCanjes_DebeRetornarListaDeDtos()
    {
        // Arrange
        var visitanteId = Guid.NewGuid();
        var recompensaId = Guid.NewGuid();

        var historial = new List<HistorialCanje>
        {
            new()
            {
                Id = Guid.NewGuid(),
                VisitanteId = visitanteId,
                RecompensaId = recompensaId,
                PuntosCanjeados = 100,
                FechaCanje = DateTime.UtcNow
            }
        };

        var recompensa = new Recompensa
        {
            Id = recompensaId,
            Nombre = "Premio Test",
            CostoEnPuntos = 100,
            CantidadDisponible = 5,
            FechaCreacion = DateTime.UtcNow
        };

        _mockRepoHistorial.Setup(r => r.ObtenerTodos()).Returns(historial);
        _mockRepoRecompensa.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Recompensa, bool>>>()))
            .Returns(recompensa);

        // Act
        var resultado = _servicio.ObtenerHistorialCanjes(visitanteId);
        var resultado_obj = resultado.FirstOrDefault(d => d.VisitanteId == visitanteId);

        // Assert
        Assert.IsNotNull(resultado);
        Assert.AreEqual(1, resultado.Count);
        Assert.AreEqual(visitanteId, resultado_obj!.VisitanteId);
        Assert.AreEqual(recompensaId, resultado_obj.RecompensaId);
        Assert.AreEqual("Premio Test", resultado_obj.NombreRecompensa);
        Assert.AreEqual(100, resultado_obj.PuntosCanjeados);
    }

    [TestMethod]
    public void ObtenerHistorialCanjes_SinCanjes_DebeRetornarListaVacia()
    {
        var visitanteId = Guid.NewGuid();
        _mockRepoHistorial.Setup(r => r.ObtenerTodos()).Returns([]);

        var resultado = _servicio.ObtenerHistorialCanjes(visitanteId);

        Assert.IsNotNull(resultado);
        Assert.AreEqual(0, resultado.Count);
    }

    [TestMethod]
    public void EliminarRecompensa_ConIdValido_DebeEliminarRecompensa()
    {
        // Arrange
        var recompensaId = Guid.NewGuid();
        var recompensaExistente = new Recompensa
        {
            Id = recompensaId,
            Nombre = "Recompensa a eliminar",
            Descripcion = "Descripción de prueba",
            CostoEnPuntos = 100,
            CantidadDisponible = 5,
            FechaCreacion = DateTime.UtcNow
        };

        _mockRepoRecompensa.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Recompensa, bool>>>()))
            .Returns(recompensaExistente);

        // Act
        _servicio.EliminarRecompensa(recompensaId);

        // Assert
        _mockRepoRecompensa.Verify(r => r.Eliminar(It.Is<Expression<Func<Recompensa, bool>>>(
            expr => expr.Compile()(recompensaExistente))), Times.Once);
    }

    [TestMethod]
    public void EliminarRecompensa_ConIdInexistente_DebeLanzarExcepcion()
    {
        // Arrange
        var recompensaId = Guid.NewGuid();
        _mockRepoRecompensa.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Recompensa, bool>>>())).Returns((Recompensa)null!);
        var ex = Assert.ThrowsException<InvalidOperationException>(() => _servicio.EliminarRecompensa(recompensaId));
        Assert.AreEqual($"Recompensa con ID {recompensaId} no encontrada", ex.Message);
        _mockRepoRecompensa.Verify(r => r.Eliminar(It.IsAny<Expression<Func<Recompensa, bool>>>()), Times.Never);
    }
}
