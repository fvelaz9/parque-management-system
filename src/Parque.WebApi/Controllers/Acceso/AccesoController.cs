using Microsoft.AspNetCore.Mvc;
using Parque.Aplicacion.DTOs;
using Parque.Aplicacion.Servicios.Acceso;
using Parque.WebApi.Controllers.Acceso.Models;
using Parque.WebApi.Filtros;

namespace Parque.WebApi.Controllers.Acceso;

[ApiController]
[Route("api/acceso")]
public class AccesoController(IServicioAcceso servicio) : ControllerBase
{
    [HttpPost("validar")]
    [AuthorizationFilter("Operador")]
    public IActionResult ValidarAcceso([FromBody] ValidarAccesoRequest request)
    {
        var resultado = servicio.ValidarAcceso(request);

        if(resultado.AccesoPermitido)
        {
            return Ok(resultado);
        }

        return BadRequest(resultado);
    }

    [HttpPost("atraccion/{atraccionId}/ingreso")]
    [AuthorizationFilter("Operador")]
    public IActionResult RegistrarIngreso(int atraccionId, [FromBody] RegistrarIngresoRequest request)
    {
        var registro = servicio.RegistrarIngreso(request.CodigoTicket, atraccionId, request.CuentaVisitante);

        return Ok(new
        {
            mensaje = "Ingreso registrado exitosamente",
            registro,
            fechaIngreso = registro.FechaIngreso
        });
    }

    [HttpPost("atraccion/{atraccionId}/egreso")]
    [AuthorizationFilter("Operador")]
    public IActionResult RegistrarEgreso(int atraccionId, [FromBody] RegistrarEgresoRequest request)
    {
        var registro = servicio.RegistrarEgreso(request.CodigoTicket, atraccionId);

        var tiempoVisita = registro.FechaEgreso.HasValue
            ? (registro.FechaEgreso.Value - registro.FechaIngreso).TotalMinutes
            : 0;

        return Ok(new
        {
            mensaje = "Egreso registrado exitosamente. Puntos calculados.",
            registro,
            tiempoVisitaMinutos = Math.Round(tiempoVisita, 2)
        });
    }

    [HttpGet("atraccion/{atraccionId}/aforo")]
    [AuthorizationFilter("Operador")]
    public IActionResult ObtenerAforo(int atraccionId)
    {
        var aforo = servicio.ObtenerAforoAtraccion(atraccionId);
        return Ok(aforo);
    }
}
