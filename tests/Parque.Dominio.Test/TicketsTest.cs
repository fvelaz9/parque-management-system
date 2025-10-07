namespace Parque.Dominio.Test;

[TestClass]
public class TicketsTest
{
    [TestMethod]
    public void Constructor_Ticket_PropiedadesCorrectamenteInicializadas()
    {
        var fechaVisita = DateTime.Now;
        var fechaEmision = DateTime.Now.AddDays(-1);
        var cuentaId = new Guid("12345678-1234-1234-1234-123456789abc");

        var ticket = new Ticket
        {
            Id = 1,
            CuentaId = cuentaId,
            FechaVisita = fechaVisita,
            EventoId = 456,
            Codigo = Guid.NewGuid(),
            FechaEmision = fechaEmision,
            EsValido = true
        };

        Assert.AreEqual(1, ticket.Id);
        Assert.AreEqual(cuentaId, ticket.CuentaId);
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
        var cuentaId = new Guid("12345678-1234-1234-1234-123456789abc");
        var ticket = new Ticket(cuentaId, DateTime.Now.AddDays(1), 1, TipoTicket.General);
        Assert.IsTrue(ticket.EsValido);

        ticket.MarcarComoUsado();

        Assert.IsFalse(ticket.EsValido);
    }

    [TestMethod]
    public void EstaVigente_TicketValidoYFechaFutura_DeberiaSerTrue()
    {
        var cuentaId = new Guid("12345678-1234-1234-1234-123456789abc");
        var ticket = new Ticket(cuentaId, DateTime.Now.AddDays(1), 1, TipoTicket.General)
        {
            EsValido = true
        };

        var resultado = ticket.EstaVigente();

        Assert.IsTrue(resultado);
    }

    [TestMethod]
    public void EstaVigente_TicketInvalido_DeberiaSerFalse()
    {
        var cuentaId = new Guid("12345678-1234-1234-1234-123456789abc");
        var ticket = new Ticket(cuentaId, DateTime.Now.AddDays(1), 1, TipoTicket.General)
        {
            EsValido = false
        };

        var resultado = ticket.EstaVigente();

        Assert.IsFalse(resultado);
    }

    [TestMethod]
    public void EstaVigente_TicketValidoPeroFechaPasada_DeberiaSerFalse()
    {
        var ticket = new Ticket
        {
            EsValido = true,
            FechaVisita = DateTime.Now.AddDays(-1) // Fecha pasada
        };

        var resultado = ticket.EstaVigente();

        Assert.IsFalse(resultado);
    }

    [TestMethod]
    public void EstaVigente_TicketValidoFechaHoy_DeberiaSerTrue()
    {
        var ticket = new Ticket
        {
            EsValido = true,
            FechaVisita = DateTime.Now.Date // Solo la fecha de hoy
        };

        var resultado = ticket.EstaVigente();

        Assert.IsTrue(resultado);
    }

    [TestMethod]
    public void TipoEntrada_DeberiaPoderAsignarYRecuperar()
    {
        var ticket = new Ticket();
        var tipoEsperado = TipoTicket.General;

        ticket.TipoEntrada = tipoEsperado;

        Assert.AreEqual(tipoEsperado, ticket.TipoEntrada);
    }

    [TestMethod]
    public void Codigo_DeberiaSerUnicoPorTicket()
    {
        var cuentaId = new Guid("12345678-1234-1234-1234-123456789abc");
        var cuentaId2 = new Guid("12345678-1234-1234-1234-123456777abc");
        var ticket1 = new Ticket(cuentaId, DateTime.Now.AddDays(1), 1, TipoTicket.General);
        var ticket2 = new Ticket(cuentaId2, DateTime.Now.AddDays(1), 2, TipoTicket.General);

        Assert.AreNotEqual(ticket1.Codigo, ticket2.Codigo);
        Assert.AreNotEqual(Guid.Empty, ticket1.Codigo);
        Assert.AreNotEqual(Guid.Empty, ticket2.Codigo);
    }

    [TestMethod]
    public void FechaEmision_DeberiaSerAutomatica()
    {
        var cuentaId = new Guid("12345678-1234-1234-1234-123456789abc");
        var antes = DateTime.Now.AddSeconds(-1);

        var ticket = new Ticket(cuentaId, DateTime.Now.AddDays(1), 1, TipoTicket.General);

        Assert.IsTrue(ticket.FechaEmision >= antes && ticket.FechaEmision <= DateTime.Now.AddSeconds(1));
    }

    [TestMethod]
    public void EventoId_PuedeSerNulo()
    {
        var ticket = new Ticket();

        ticket.EventoId = null;

        Assert.IsNull(ticket.EventoId);
    }

    [TestMethod]
    public void EventoId_PuedeSerCero()
    {
        var ticket = new Ticket();

        ticket.EventoId = 0;

        Assert.AreEqual(0, ticket.EventoId);
    }
}
