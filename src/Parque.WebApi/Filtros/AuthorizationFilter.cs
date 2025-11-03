using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Parque.Aplicacion.Servicios;
using Parque.Dominio.Excepciones;

namespace Parque.WebApi.Filtros;

public class AuthorizationFilter(string rol) : Attribute, IAuthorizationFilter
{
    private const string AUTHORIZATION_HEADER = "Authorization";
    private readonly string _rol = rol ?? throw new ArgumentNullException(nameof(rol));

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        // ✅ AGREGAR: Verificar si el endpoint tiene [AllowAnonymous]
        var hasAllowAnonymous = context.ActionDescriptor.EndpointMetadata
            .Any(m => m is IAllowAnonymous);

        if(hasAllowAnonymous)
        {
            return; // Permitir acceso sin autenticación
        }

        var token = context.HttpContext.Request.Headers[AUTHORIZATION_HEADER].ToString();

        if(string.IsNullOrWhiteSpace(token))
        {
            context.Result = new ObjectResult(new ResponseDto
            {
                Message = "No estás autenticado",
                ExecutionSuccessful = false
            })
            {
                StatusCode = (int)HttpStatusCode.Unauthorized
            };
            return;
        }

        var servicioSesion = GetServicioSesion(context);
        if(servicioSesion == null)
        {
            context.Result = new ObjectResult(new ResponseDto
            {
                Message = "Error interno: Servicio de sesión no disponible",
                ExecutionSuccessful = false
            })
            {
                StatusCode = (int)HttpStatusCode.InternalServerError
            };
            return;
        }

        try
        {
            if(!servicioSesion.ValidarSesion(token, _rol))
            {
                context.Result = new ObjectResult(new ResponseDto
                {
                    Message = "Usuario no autorizado",
                    ExecutionSuccessful = false
                })
                {
                    StatusCode = (int)HttpStatusCode.Forbidden
                };
                return;
            }

            var usuario = servicioSesion.ObtenerUsuarioSesion(token);
            context.HttpContext.Items["user"] = usuario;
        }
        catch(ExcepcionDominio ex)
        {
            context.Result = new ObjectResult(new ResponseDto
            {
                Message = ex.Message,
                ExecutionSuccessful = false
            })
            {
                StatusCode = (int)HttpStatusCode.Unauthorized
            };
        }
        catch(Exception)
        {
            context.Result = new ObjectResult(new ResponseDto
            {
                Message = "Ocurrió un error inesperado al procesar la solicitud",
                ExecutionSuccessful = false
            })
            {
                StatusCode = (int)HttpStatusCode.InternalServerError
            };
        }
    }

    private IServicioSesion? GetServicioSesion(AuthorizationFilterContext context)
    {
        return context.HttpContext.RequestServices.GetService<IServicioSesion>();
    }
}
