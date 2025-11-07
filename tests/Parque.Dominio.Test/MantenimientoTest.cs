namespace Parque.Dominio.Test;

[TestClass]
public class MantenimientoTest
{
    [TestMethod]
    public void Id_DeberiaPoderAsignarYRecuperar()
    {
        var mantenimiento = new MantenimientoPreventivo();
        var idEsperado = 123;
        mantenimiento.Id = idEsperado;
        Assert.AreEqual(idEsperado, mantenimiento.Id);
    }

    [TestMethod]
    public void Descripcion_DeberiaPoderAsignarYRecuperar()
    {
        var mantenimiento = new MantenimientoPreventivo();
        var descripcionEsperada = "Falla en el sistema de frenos";
        mantenimiento.Descripcion = descripcionEsperada;
        Assert.AreEqual(descripcionEsperada, mantenimiento.Descripcion);
    }

    [TestMethod]
    public void ConstructorConParametros_DeberiaAsignarPropiedades()
    {
        var descripcion = "Falla";
        var fechaInicio = new DateTime(2025, 10, 8, 10, 0, 0);
        var duracion = new TimeSpan(9, 30, 0);
        var horaInicio = new TimeSpan(9, 30, 0);
        var atraccionId = 5;
        var incidencia = new Incidencia();
        var incidenciaId = 4;

        var mantenimiento = new MantenimientoPreventivo();
        mantenimiento.Descripcion = descripcion;
        mantenimiento.FechaProgramada = fechaInicio;
        mantenimiento.HoraInicio = horaInicio;
        mantenimiento.DuracionEstimada = duracion;
        mantenimiento.AtraccionId = atraccionId;
        mantenimiento.IncidenciaAsociada = incidencia;
        mantenimiento.IncidenciaId = incidenciaId;

        Assert.AreEqual(descripcion, mantenimiento.Descripcion);
        Assert.AreEqual(fechaInicio, mantenimiento.FechaProgramada);
        Assert.AreEqual(duracion, mantenimiento.DuracionEstimada);
        Assert.AreEqual(horaInicio, mantenimiento.HoraInicio);
        Assert.AreEqual(atraccionId, mantenimiento.AtraccionId);
        Assert.AreEqual(incidencia, mantenimiento.IncidenciaAsociada);
        Assert.AreEqual(incidenciaId, mantenimiento.IncidenciaId);
    }

    [TestMethod]
    public void Probar_Metodos_Fechas()
    {
        var mantenimiento = new MantenimientoPreventivo();
        var fechaInicio = new DateTime(2025, 10, 8, 10, 0, 0);
        var horaInicio = new TimeSpan(9, 30, 0);
        var duracion = new TimeSpan(9, 30, 0);
        mantenimiento.FechaProgramada = fechaInicio;
        mantenimiento.HoraInicio = horaInicio;
        mantenimiento.DuracionEstimada = duracion;
        _ = mantenimiento.FechaHoraInicio();
        _ = mantenimiento.FechaHoraFin();
    }
}
