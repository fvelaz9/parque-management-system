namespace Parque.Dominio.Test;

[TestClass]
public class TicketsTest
{
    [TestMethod]
    public void Constructor_Ticket_PropiedadesCorrectamenteInicializadas()
    {
        var fechaVisita = new DateTime(2025, 10, 10, 14, 0, 0);
        var fechaEmision = new DateTime(2025, 10, 8, 10, 0, 0);
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

        var fechaReferencia = new DateTime(2025, 10, 8, 12, 0, 0);

        Assert.AreEqual(1, ticket.Id);
        Assert.AreEqual(cuentaId, ticket.CuentaId);
        Assert.AreEqual(fechaVisita, ticket.FechaVisita);
        Assert.AreEqual(456, ticket.EventoId);
        Assert.IsNotNull(ticket.Codigo);
        Assert.AreEqual(fechaEmision, ticket.FechaEmision);
        Assert.IsTrue(ticket.EsValido);
        Assert.IsTrue(ticket.EstaVigente(fechaReferencia));
    }

    [TestMethod]
    public void MarcarComoUsado_DeberiaInvalidarTicket()
    {
        var cuentaId = new Guid("12345678-1234-1234-1234-123456789abc");
        var fechaActual = new DateTime(2025, 10, 8, 10, 0, 0);
        var fechaVisita = new DateTime(2025, 10, 10, 14, 0, 0);
        var ticket = new Ticket(cuentaId, fechaVisita, 1, TipoTicket.General, fechaActual);
        Assert.IsTrue(ticket.EsValido);

        ticket.MarcarComoUsado();

        Assert.IsFalse(ticket.EsValido);
    }

    [TestMethod]
    public void EstaVigente_TicketValidoYFechaFutura_DeberiaSerTrue()
    {
        var cuentaId = new Guid("12345678-1234-1234-1234-123456789abc");
        var fechaActual = new DateTime(2025, 10, 8, 10, 0, 0);
        var fechaVisita = new DateTime(2025, 10, 10, 14, 0, 0);
        var ticket = new Ticket(cuentaId, fechaVisita, 1, TipoTicket.General, fechaActual)
        {
            EsValido = true
        };

        var fechaReferencia = new DateTime(2025, 10, 9, 12, 0, 0);
        var resultado = ticket.EstaVigente(fechaReferencia);

        Assert.IsTrue(resultado);
    }

    [TestMethod]
    public void EstaVigente_TicketInvalido_DeberiaSerFalse()
    {
        var cuentaId = new Guid("12345678-1234-1234-1234-123456789abc");
        var fechaActual = new DateTime(2025, 10, 8, 10, 0, 0);
        var fechaVisita = new DateTime(2025, 10, 10, 14, 0, 0);
        var ticket = new Ticket(cuentaId, fechaVisita, 1, TipoTicket.General, fechaActual)
        {
            EsValido = false
        };

        var fechaReferencia = new DateTime(2025, 10, 9, 12, 0, 0);
        var resultado = ticket.EstaVigente(fechaReferencia);

        Assert.IsFalse(resultado);
    }

    [TestMethod]
    public void EstaVigente_TicketValidoPeroFechaPasada_DeberiaSerFalse()
    {
        var fechaVisita = new DateTime(2025, 10, 8, 10, 0, 0);
        var fechaReferencia = new DateTime(2025, 10, 10, 12, 0, 0);

        var ticket = new Ticket
        {
            EsValido = true,
            FechaVisita = fechaVisita
        };

        var resultado = ticket.EstaVigente(fechaReferencia);

        Assert.IsFalse(resultado);
    }

    [TestMethod]
    public void EstaVigente_TicketValidoFechaHoy_DeberiaSerTrue()
    {
        var fechaVisita = new DateTime(2025, 10, 10, 10, 0, 0);
        var fechaReferencia = new DateTime(2025, 10, 10, 12, 0, 0);

        var ticket = new Ticket
        {
            EsValido = true,
            FechaVisita = fechaVisita
        };

        var resultado = ticket.EstaVigente(fechaReferencia);

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
        var fechaActual = new DateTime(2025, 10, 8, 10, 0, 0);
        var fechaVisita = new DateTime(2025, 10, 10, 14, 0, 0);
        var ticket1 = new Ticket(cuentaId, fechaVisita, 1, TipoTicket.General, fechaActual);
        var ticket2 = new Ticket(cuentaId2, fechaVisita, 2, TipoTicket.General, fechaActual);

        Assert.AreNotEqual(ticket1.Codigo, ticket2.Codigo);
        Assert.AreNotEqual(Guid.Empty, ticket1.Codigo);
        Assert.AreNotEqual(Guid.Empty, ticket2.Codigo);
    }

    [TestMethod]
    public void FechaEmision_DeberiaSerLaFechaProporcionada()
    {
        var cuentaId = new Guid("12345678-1234-1234-1234-123456789abc");
        var fechaActual = new DateTime(2025, 10, 8, 10, 0, 0);
        var fechaVisita = new DateTime(2025, 10, 10, 14, 0, 0);

        var ticket = new Ticket(cuentaId, fechaVisita, 1, TipoTicket.General, fechaActual);

        Assert.AreEqual(fechaActual, ticket.FechaEmision);
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

    [TestMethod]
    public void Constructor_FechaPasada_DeberiaLanzarExcepcion()
    {
        var cuentaId = Guid.NewGuid();
        var fechaActual = new DateTime(2025, 10, 10, 10, 0, 0);
        var fechaPasada = new DateTime(2025, 10, 8, 10, 0, 0);

        Assert.ThrowsException<ArgumentException>(
            () => new Ticket(cuentaId, fechaPasada, 1, TipoTicket.General, fechaActual));
    }
}
