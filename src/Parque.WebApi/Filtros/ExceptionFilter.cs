using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Parque.Dominio.Excepciones;

namespace Parque.WebApi.Filtros;

public class ExceptionFilter : Attribute, IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        var response = new ResponseDto
        {
            ExecutionSuccessful = false
        };

        var statusCode = context.Exception switch
        {
            ExcepcionDominio ex => HandleDomainException(ex, response),
            ExcepcionEntidadNoEncontrada ex => HandleNotFoundExceptions(ex, response),
            ArgumentException ex => HandleArgumentException(ex, response),
            InvalidOperationException ex => HandleInvalidOperationException(ex, response),
            _ => HandleGenericException(response)
        };

        context.Result = new ObjectResult(response)
        {
            StatusCode = statusCode
        };

        context.ExceptionHandled = true;
    }

    private static int HandleDomainException(ExcepcionDominio ex, ResponseDto response)
    {
        response.Message = ex.Message;
        return (int)HttpStatusCode.BadRequest;
    }

    private static int HandleNotFoundExceptions(ExcepcionEntidadNoEncontrada ex, ResponseDto response)
    {
        response.Message = ex.Message;
        return (int)HttpStatusCode.NotFound;
    }

    private static int HandleArgumentException(ArgumentException ex, ResponseDto response)
    {
        response.Message = ex.Message ?? "Datos inválidos";
        return (int)HttpStatusCode.BadRequest;
    }

    private static int HandleInvalidOperationException(InvalidOperationException ex, ResponseDto response)
    {
        response.Message = ex.Message ?? "Operación inválida";
        return (int)HttpStatusCode.BadRequest;
    }

    private static int HandleGenericException(ResponseDto response)
    {
        response.Message = "Ocurrió un error inesperado en el servidor";
        return (int)HttpStatusCode.InternalServerError;
    }
}
