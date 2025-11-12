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
        
        // Mock fecha actual
        _mockServicioFechaHora.Setup(s => s.ObtenerFechaActual())
            .Returns(new DateTime(2025, 11, 11, 22, 0, 0));
        
        _servicio = new ServicioRecompensa();
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
}
