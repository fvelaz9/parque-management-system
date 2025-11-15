using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Parque.Aplicacion.DTOs;
using Parque.Aplicacion.Servicios.Mantenimiento;
using Parque.WebApi.Filtros;

namespace Parque.WebApi.Controllers;

[ApiController]
[Route("api/mantenimientos")]
[AllowAnonymous]
public class MantenimientosController(IServicioMantenimiento servicio) : ControllerBase
{
    [HttpGet]

    [AuthorizationFilter("Administrador")]
    public IActionResult GetAll()
    {
        var mantenimientos = servicio.ListarMantenimientos();
        return Ok(mantenimientos);
    }

    [HttpPost]

    [AuthorizationFilter("Administrador")]
    public IActionResult Create([FromBody] CrearMantenimientoRequest request)
    {
        try
        {
            var mantenimiento = servicio.CrearMantenimiento(request);
            return CreatedAtAction(nameof(GetAll), new { id = mantenimiento.Id }, mantenimiento);
        }
        catch(ArgumentException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    [HttpDelete("{id}")]

    [AuthorizationFilter("Administrador")]
    public IActionResult Delete(int id)
    {
        try
        {
            servicio.EliminarMantenimiento(id);
            return NoContent();
        }
        catch(ArgumentException ex)
        {
            return NotFound(new { mensaje = ex.Message });
        }
    }
    
    [HttpPut("{id}")]
    [AuthorizationFilter("Administrador")]
    public IActionResult ActualizarMantenimiento(int id, [FromBody] CrearMantenimientoRequest dto)
    {
        try
        {
            var mantenimiento = servicio.ActualizarMantenimiento(id, dto);
            return Ok(new
            {
                mensaje = "Mantenimiento actualizado exitosamente",
                mantenimiento = new
                {
                    mantenimiento.Id,
                    mantenimiento.AtraccionId,
                    mantenimiento.FechaProgramada,
                    mantenimiento.HoraInicio,
                    mantenimiento.DuracionEstimada,
                    mantenimiento.Descripcion
                }
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }
}
