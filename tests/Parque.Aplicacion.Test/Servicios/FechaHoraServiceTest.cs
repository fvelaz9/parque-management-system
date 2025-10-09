using Moq;
using Parque.Aplicacion.Servicios;
using Parque.Dominio;
using Parque.Infraestructura.Repositorios;

namespace Parque.Aplicacion.Test.Servicios;

[TestClass]
public class FechaHoraServiceTest
{
    private Mock<IRepositorio<ConfiguracionFechaHora>>? _mockRepositorio;
    private ServicioFechaHora? _servicio;

    [TestInitialize]
    public void Setup()
    {
        _mockRepositorio = new Mock<IRepositorio<ConfiguracionFechaHora>>();
        _servicio = new ServicioFechaHora(_mockRepositorio.Object);
    }

    [TestMethod]
    public void ObtenerFechaActual_SinConfiguracion_DeberiaRetornarFechaSistema()
    {
        // Arrange
        _mockRepositorio!.Setup(r => r.ObtenerTodos()).Returns([]);

        // Act
        var resultado = _servicio!.ObtenerFechaActual();

        // Assert
        Assert.IsTrue((DateTime.Now - resultado).TotalSeconds < 1);
    }

    [TestMethod]
    public void ConfigurarFecha_PrimeraVez_DeberiaAgregarConfiguracion()
    {
        // Arrange
        var nuevaFecha = new DateTime(2025, 9, 2, 14, 45, 0);
        var configuracionInicial = new ConfiguracionFechaHora(new DateTime(2025, 9, 1, 10, 0, 0)) { Id = 1 };

        _mockRepositorio!.SetupSequence(r => r.ObtenerTodos())
            .Returns([configuracionInicial])
            .Returns([]);

        // Act
        _servicio!.ConfigurarFecha(nuevaFecha);

        // Assert
        _mockRepositorio.Verify(r => r.Agregar(It.IsAny<ConfiguracionFechaHora>()), Times.Once);
    }

    [TestMethod]
    public void ConfigurarFecha_YaExiste_DeberiaEditarConfiguracion()
    {
        // Arrange
        var fechaAnterior = new DateTime(2025, 9, 2, 14, 45, 0);
        var nuevaFecha = new DateTime(2025, 9, 2, 16, 30, 0);
        var configuracionExistente = new ConfiguracionFechaHora(fechaAnterior) { Id = 1 };

        _mockRepositorio!.Setup(r => r.ObtenerTodos()).Returns([configuracionExistente]);

        // Act
        _servicio!.ConfigurarFecha(nuevaFecha);

        // Assert
        _mockRepositorio.Verify(r => r.Editar(It.Is<ConfiguracionFechaHora>(c => c.FechaHoraConfigurada == nuevaFecha)), Times.Once);
        _mockRepositorio.Verify(r => r.Agregar(It.IsAny<ConfiguracionFechaHora>()), Times.Never);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void ConfigurarFecha_FechaAnterior_DeberiaLanzarExcepcion()
    {
        // Arrange
        var fechaActual = new DateTime(2025, 9, 2, 14, 45, 0);
        var fechaAnterior = new DateTime(2025, 9, 1, 10, 0, 0);
        var configuracionExistente = new ConfiguracionFechaHora(fechaActual) { Id = 1 };

        _mockRepositorio!.Setup(r => r.ObtenerTodos()).Returns([configuracionExistente]);

        // Act
        _servicio!.ConfigurarFecha(fechaAnterior);

        // Assert se maneja por ExpectedException
    }

    [TestMethod]
    public void UsaFechaPersonalizada_ConConfiguracion_DeberiaRetornarTrue()
    {
        // Arrange
        var configuracion = new ConfiguracionFechaHora(DateTime.Now) { Id = 1 };
        _mockRepositorio!.Setup(r => r.ObtenerTodos()).Returns([configuracion]);

        // Act
        var resultado = _servicio!.UsaFechaPersonalizada();

        // Assert
        Assert.IsTrue(resultado);
    }

    [TestMethod]
    public void UsaFechaPersonalizada_SinConfiguracion_DeberiaRetornarFalse()
    {
        // Arrange
        _mockRepositorio!.Setup(r => r.ObtenerTodos()).Returns([]);

        // Act
        var resultado = _servicio!.UsaFechaPersonalizada();

        // Assert
        Assert.IsFalse(resultado);
    }

    [TestMethod]
    public void ResetearAFechaSistema_DeberiaEliminarConfiguracion()
    {
        // Arrange & Act
        _servicio!.ResetearAFechaSistema();

        // Assert
        _mockRepositorio!.Verify(r => r.Eliminar(It.IsAny<System.Linq.Expressions.Expression<Func<ConfiguracionFechaHora, bool>>>()), Times.Once);
    }
}
