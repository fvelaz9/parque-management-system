using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Parque.Dominio.Excepciones;
using Parque.WebApi.Filtros;

namespace Parque.WebApi.Test.Filtros;

[TestClass]
public class ExceptionFilterTest
{
    private ExceptionFilter? _filter;

    [TestInitialize]
    public void Initialize()
    {
        _filter = new ExceptionFilter();
    }

    [TestMethod]
    public void OnException_ConExcepcionDominio_DeberiaRetornarBadRequest()
    {
        var exception = new ExcepcionDominio("Error de dominio");
        var context = CreateExceptionContext(exception);

        _filter!.OnException(context);

        Assert.IsTrue(context.ExceptionHandled);
        var result = context.Result as ObjectResult;
        Assert.IsNotNull(result);
        Assert.AreEqual(400, result.StatusCode);
        var response = result.Value as ResponseDto;
        Assert.IsNotNull(response);
        Assert.IsFalse(response.ExecutionSuccessful);
        Assert.AreEqual("Error de dominio", response.Message);
    }

    [TestMethod]
    public void OnException_ConExcepcionEntidadNoEncontrada_DeberiaRetornarNotFound()
    {
        var exception = new ExcepcionEntidadNoEncontrada("Entidad no encontrada");
        var context = CreateExceptionContext(exception);

        _filter!.OnException(context);

        Assert.IsTrue(context.ExceptionHandled);
        var result = context.Result as ObjectResult;
        Assert.IsNotNull(result);
        Assert.AreEqual(404, result.StatusCode);
        var response = result.Value as ResponseDto;
        Assert.IsNotNull(response);
        Assert.IsFalse(response.ExecutionSuccessful);
        Assert.AreEqual("Entidad no encontrada", response.Message);
    }

    [TestMethod]
    public void OnException_ConArgumentException_DeberiaRetornarBadRequest()
    {
        var exception = new ArgumentException("Argumento inválido");
        var context = CreateExceptionContext(exception);

        _filter!.OnException(context);

        Assert.IsTrue(context.ExceptionHandled);
        var result = context.Result as ObjectResult;
        Assert.IsNotNull(result);
        Assert.AreEqual(400, result.StatusCode);
        var response = result.Value as ResponseDto;
        Assert.IsNotNull(response);
        Assert.IsFalse(response.ExecutionSuccessful);
        Assert.AreEqual("Argumento inválido", response.Message);
    }

    [TestMethod]
    public void OnException_ConInvalidOperationException_DeberiaRetornarBadRequest()
    {
        var exception = new InvalidOperationException("Operación no válida");
        var context = CreateExceptionContext(exception);

        _filter!.OnException(context);

        Assert.IsTrue(context.ExceptionHandled);
        var result = context.Result as ObjectResult;
        Assert.IsNotNull(result);
        Assert.AreEqual(400, result.StatusCode);
        var response = result.Value as ResponseDto;
        Assert.IsNotNull(response);
        Assert.IsFalse(response.ExecutionSuccessful);
        Assert.AreEqual("Operación no válida", response.Message);
    }

    [TestMethod]
    public void OnException_ConExcepcionGenerica_DeberiaRetornarInternalServerError()
    {
        var exception = new Exception("Error inesperado");
        var context = CreateExceptionContext(exception);

        _filter!.OnException(context);

        Assert.IsTrue(context.ExceptionHandled);
        var result = context.Result as ObjectResult;
        Assert.IsNotNull(result);
        Assert.AreEqual(500, result.StatusCode);
        var response = result.Value as ResponseDto;
        Assert.IsNotNull(response);
        Assert.IsFalse(response.ExecutionSuccessful);
        Assert.AreEqual("Ocurrió un error inesperado en el servidor", response.Message);
    }

    private static ExceptionContext CreateExceptionContext(Exception exception)
    {
        var actionContext = new ActionContext(
            new DefaultHttpContext(),
            new RouteData(),
            new ActionDescriptor());

        return new ExceptionContext(actionContext, [])
        {
            Exception = exception
        };
    }
}
