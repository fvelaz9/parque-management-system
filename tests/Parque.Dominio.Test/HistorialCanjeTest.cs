using Parque.Dominio.Gamificacion;

namespace Parque.Dominio.Test;

[TestClass]
public class HistorialCanjeTest
{
    [TestMethod]
    public void Constructor_ConDatosValidos_DebeCrearHistorialConPropiedadesCorrectas()
    {
        // Arrange
        var id = Guid.NewGuid();
        var visitanteId = Guid.NewGuid();
        var recompensaId = Guid.NewGuid();
        var puntos = 500;
        var fecha = DateTime.UtcNow;

        // Act
        var historial = new HistorialCanje
        {
            Id = id,
            VisitanteId = visitanteId,
            RecompensaId = recompensaId,
            PuntosCanjeados = puntos,
            FechaCanje = fecha
        };

        // Assert
        Assert.AreEqual(id, historial.Id);
        Assert.AreEqual(visitanteId, historial.VisitanteId);
        Assert.AreEqual(recompensaId, historial.RecompensaId);
        Assert.AreEqual(puntos, historial.PuntosCanjeados);
        Assert.AreEqual(fecha, historial.FechaCanje);
    }
}
