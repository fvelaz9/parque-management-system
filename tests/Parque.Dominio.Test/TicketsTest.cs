namespace Parque.Dominio.Test;

[TestClass]
public class TicketsTest
{
    [TestMethod]
    public void Constructor_Ticket_PropiedadesCorrectamenteInicializadas()
    {
        // Arrange
        var fechaVisita = DateTime.Now;
        var fechaEmision = DateTime.Now.AddDays(-1);

        // Act
        var ticket = new Ticket
        {
            Id = 1,
            CuentaId = 123,
            FechaVisita = fechaVisita,
            EventoId = 456,
            Codigo = Guid.NewGuid(),
            FechaEmision = fechaEmision,
            EsValido = true
        };

        // Assert
        Assert.AreEqual(1, ticket.Id);
        Assert.AreEqual(123, ticket.CuentaId);
        Assert.AreEqual(fechaVisita, ticket.FechaVisita);
        Assert.AreEqual(456, ticket.EventoId);
        Assert.IsNotNull(ticket.Codigo);
        Assert.AreEqual(fechaEmision, ticket.FechaEmision);
        Assert.IsTrue(ticket.EsValido);
        Assert.IsTrue(ticket.EstaVigente());
    }
}
