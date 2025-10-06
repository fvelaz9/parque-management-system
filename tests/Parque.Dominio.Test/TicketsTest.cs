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

    [TestMethod]
    public void MarcarComoUsado_DeberiaInvalidarTicket()
    {
        // Arrange
        var ticket = new Ticket(1, DateTime.Now.AddDays(1), 1, TipoTicket.General);
        Assert.IsTrue(ticket.EsValido);

        // Act
        ticket.MarcarComoUsado();

        // Assert
        Assert.IsFalse(ticket.EsValido);
    }

    [TestMethod]
    public void EstaVigente_TicketValidoYFechaFutura_DeberiaSerTrue()
    {
        // Arrange
        var ticket = new Ticket(1, DateTime.Now.AddDays(1), 1, TipoTicket.General)
        {
            EsValido = true
        };

        // Act
        var resultado = ticket.EstaVigente();

        // Assert
        Assert.IsTrue(resultado);
    }

    [TestMethod]
    public void EstaVigente_TicketInvalido_DeberiaSerFalse()
    {
        // Arrange
        var ticket = new Ticket(1, DateTime.Now.AddDays(1), 1, TipoTicket.General)
        {
            EsValido = false
        };

        // Act
        var resultado = ticket.EstaVigente();

        // Assert
        Assert.IsFalse(resultado);
    }

    [TestMethod]
    public void EstaVigente_TicketValidoPeroFechaPasada_DeberiaSerFalse()
    {
        // Arrange
        var ticket = new Ticket
        {
            EsValido = true,
            FechaVisita = DateTime.Now.AddDays(-1) // Fecha pasada
        };

        // Act
        var resultado = ticket.EstaVigente();

        // Assert
        Assert.IsFalse(resultado);
    }

    [TestMethod]
    public void EstaVigente_TicketValidoFechaHoy_DeberiaSerTrue()
    {
        // Arrange
        var ticket = new Ticket
        {
            EsValido = true,
            FechaVisita = DateTime.Now.Date // Solo la fecha de hoy
        };

        // Act
        var resultado = ticket.EstaVigente();

        // Assert
        Assert.IsTrue(resultado);
    }

    [TestMethod]
    public void TipoEntrada_DeberiaPoderAsignarYRecuperar()
    {
        // Arrange
        var ticket = new Ticket();
        var tipoEsperado = TipoTicket.General;

        // Act
        ticket.TipoEntrada = tipoEsperado;

        // Assert
        Assert.AreEqual(tipoEsperado, ticket.TipoEntrada);
    }

    [TestMethod]
    public void Codigo_DeberiaSerUnicoPorTicket()
    {
        // Arrange
        var ticket1 = new Ticket(1, DateTime.Now.AddDays(1), 1, TipoTicket.General);
        var ticket2 = new Ticket(2, DateTime.Now.AddDays(1), 2, TipoTicket.General);

        // Assert
        Assert.AreNotEqual(ticket1.Codigo, ticket2.Codigo);
        Assert.AreNotEqual(Guid.Empty, ticket1.Codigo);
        Assert.AreNotEqual(Guid.Empty, ticket2.Codigo);
    }

    [TestMethod]
    public void FechaEmision_DeberiaSerAutomatica()
    {
        // Arrange
        var antes = DateTime.Now.AddSeconds(-1);

        // Act
        var ticket = new Ticket(1, DateTime.Now.AddDays(1), 1, TipoTicket.General);

        // Assert
        Assert.IsTrue(ticket.FechaEmision >= antes && ticket.FechaEmision <= DateTime.Now.AddSeconds(1));
    }

    [TestMethod]
    public void EventoId_PuedeSerNulo()
    {
        // Arrange
        var ticket = new Ticket();

        // Act
        ticket.EventoId = null;

        // Assert
        Assert.IsNull(ticket.EventoId);
    }

    [TestMethod]
    public void EventoId_PuedeSerCero()
    {
        // Arrange
        var ticket = new Ticket();

        // Act
        ticket.EventoId = 0;

        // Assert
        Assert.AreEqual(0, ticket.EventoId);
    }
}
