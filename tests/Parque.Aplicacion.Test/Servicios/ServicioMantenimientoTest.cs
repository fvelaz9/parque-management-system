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
            _mockRepoAtracciones.Object
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
}
