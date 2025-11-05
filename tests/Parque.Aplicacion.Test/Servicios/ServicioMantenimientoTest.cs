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
            _mockRepoIncidencias.Object,
            _mockRepoAtracciones.Object,
            _mockServicioFechaHora.Object);
    }

    [TestMethod]
    public void CrearMantenimiento_AtraccionExiste_CreaMantenimientoEIncidenciaAsociada()
    {
        // Arrange
        var atraccion = new AtraccionParque("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 50, "Test") { Id = 1 };
        var fechaActual = new DateTime(2025, 11, 10, 12, 0, 0);
        var request = new CrearMantenimientoRequest
        {
            AtraccionId = 1,
            FechaProgramada = new DateTime(2025, 11, 15),
            HoraInicio = new TimeSpan(14, 30, 0),
            DuracionEstimada = new TimeSpan(2, 0, 0),
            Descripcion = "Revisión de motores"
        };

        _mockServicioFechaHora.Setup(f => f.ObtenerFechaActual()).Returns(fechaActual);
        _mockRepoAtracciones.Setup(r => r.Encontrar(It.IsAny<Expression<Func<AtraccionParque, bool>>>()))
            .Returns(atraccion);

        Incidencia? incidenciaCapturada = null;
        _mockRepoIncidencias.Setup(r => r.Agregar(It.IsAny<Incidencia>()))
            .Callback<Incidencia>(i =>
            {
                i.Id = 1;
                incidenciaCapturada = i;
            });

        MantenimientoPreventivo mantenimientiCapturado = null;
        _mockRepoMantenimientos.Setup(r => r.Agregar(It.IsAny<MantenimientoPreventivo>()))
            .Callback<MantenimientoPreventivo>(m =>
            {
                m.Id = 1;
                mantenimientiCapturado = m;
            });

        // Act
        var resultado = _servicio.CrearMantenimiento(request);
    }
}
