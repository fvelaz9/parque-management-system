using Parque.Aplicacion.Servicios;

namespace Parque.Aplicacion.Test.Servicios;

[TestClass]
public class FechaHoraServiceTest
{
    [TestMethod]
    public void ObtenerHora_SinConfigurar_DeberiaRetornarTiempoDelSistema()
    {
        // Arrange
        var servicio = new ServicioFechaHora();
        var antes = DateTime.Now;

        // Act
        var resultado = servicio.ObtenerFechaActual();

        // Assert
        Assert.IsTrue(resultado >= antes && resultado <= DateTime.Now.AddSeconds(1));
        Assert.IsFalse(servicio.UsaFechaPersonalizada());
    }

    [TestMethod]
    public void AsignarFechaPersonalizada_DeberiaConfigurarTiempoPersonalizado()
    {
        // Arrange
        var service = new ServicioFechaHora();
        var customTime = new DateTime(2025, 9, 2, 14, 45, 0);

        // Act
        service.AsignarFechaPersonalizada(customTime);
        var result = service.ObtenerFechaActual();

        // Assert
        Assert.AreEqual(customTime, result);
        Assert.IsTrue(service.UsaFechaPersonalizada());
    }

    public void ResetToSystemTime_DeberiaBorrarTiempoPersonalizado()
    {
        // Arrange
        var service = new ServicioFechaHora();
        var customTime = new DateTime(2025, 9, 2, 14, 45, 0);
        service.AsignarFechaPersonalizada(customTime);

        // Act
        service.ResetearAFechaSistema();
        var result = service.ObtenerFechaActual();

        // Assert
        Assert.AreNotEqual(customTime, result);
        Assert.IsFalse(service.UsaFechaPersonalizada());
    }
}
