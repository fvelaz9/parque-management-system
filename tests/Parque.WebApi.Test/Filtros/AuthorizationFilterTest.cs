using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Parque.Aplicacion.Servicios;
using Parque.Dominio.Excepciones;
using Parque.Dominio.Usuarios;
using Parque.WebApi.Filtros;

namespace Parque.WebApi.Test.Filtros;

[TestClass]
public class AuthorizationFilterTest
{
    private Mock<IServicioSesion>? _servicioSesionMock;
    private AuthorizationFilter? _filter;

    [TestInitialize]
    public void Initialize()
    {
        _servicioSesionMock = new Mock<IServicioSesion>(MockBehavior.Strict);
        _filter = new AuthorizationFilter("Admin");
    }

    [TestMethod]
    public void OnAuthorization_SinToken_DeberiaRetornarUnauthorized()
    {
        var context = CreateAuthorizationContext(null, _servicioSesionMock!.Object);

        _filter!.OnAuthorization(context);

        var result = context.Result as ObjectResult;
        Assert.IsNotNull(result);
        Assert.AreEqual(401, result.StatusCode);
        var response = result.Value as ResponseDto;
        Assert.IsNotNull(response);
        Assert.IsFalse(response.ExecutionSuccessful);
        Assert.AreEqual("No estás autenticado", response.Message);
    }

    [TestMethod]
    public void OnAuthorization_TokenVacio_DeberiaRetornarUnauthorized()
    {
        var context = CreateAuthorizationContext(" ", _servicioSesionMock!.Object);

        _filter!.OnAuthorization(context);

        var result = context.Result as ObjectResult;
        Assert.IsNotNull(result);
        Assert.AreEqual(401, result.StatusCode);
        var response = result.Value as ResponseDto;
        Assert.IsNotNull(response);
        Assert.IsFalse(response.ExecutionSuccessful);
        Assert.AreEqual("No estás autenticado", response.Message);
    }

    [TestMethod]
    public void OnAuthorization_SesionInvalida_DeberiaRetornarForbidden()
    {
        var token = "token123";
        _servicioSesionMock!.Setup(s => s.ValidarSesion(token, "Admin")).Returns(false);

        var context = CreateAuthorizationContext(token, _servicioSesionMock.Object);

        _filter!.OnAuthorization(context);

        var result = context.Result as ObjectResult;
        Assert.IsNotNull(result);
        Assert.AreEqual(403, result.StatusCode);
        var response = result.Value as ResponseDto;
        Assert.IsNotNull(response);
        Assert.IsFalse(response.ExecutionSuccessful);
        Assert.AreEqual("Usuario no autorizado", response.Message);
        _servicioSesionMock.VerifyAll();
    }

    [TestMethod]
    public void OnAuthorization_SesionValida_DeberiaAgregarUsuarioAlContexto()
    {
        var token = "token123";
        var usuario = Cuenta.Crear("Admin", "User", new Email("admin@test.com"), "pass123", Rol.Administrador);

        _servicioSesionMock!.Setup(s => s.ValidarSesion(token, "Admin")).Returns(true);
        _servicioSesionMock.Setup(s => s.ObtenerUsuarioSesion(token)).Returns(usuario);

        var context = CreateAuthorizationContext(token, _servicioSesionMock.Object);

        _filter!.OnAuthorization(context);

        Assert.IsNull(context.Result);
        Assert.IsTrue(context.HttpContext.Items.ContainsKey("user"));
        Assert.AreEqual(usuario, context.HttpContext.Items["user"]);
        _servicioSesionMock.VerifyAll();
    }

    [TestMethod]
    public void OnAuthorization_ExcepcionDominio_DeberiaRetornarUnauthorized()
    {
        var token = "token123";
        _servicioSesionMock!.Setup(s => s.ValidarSesion(token, "Admin"))
            .Throws(new ExcepcionDominio("Token inválido"));

        var context = CreateAuthorizationContext(token, _servicioSesionMock.Object);

        _filter!.OnAuthorization(context);

        var result = context.Result as ObjectResult;
        Assert.IsNotNull(result);
        Assert.AreEqual(401, result.StatusCode);
        var response = result.Value as ResponseDto;
        Assert.IsNotNull(response);
        Assert.IsFalse(response.ExecutionSuccessful);
        Assert.AreEqual("Token inválido", response.Message);
        _servicioSesionMock.VerifyAll();
    }

    [TestMethod]
    public void OnAuthorization_ExcepcionGenerica_DeberiaRetornarInternalServerError()
    {
        var token = "token123";
        _servicioSesionMock!.Setup(s => s.ValidarSesion(token, "Admin"))
            .Throws(new Exception("Error inesperado"));

        var context = CreateAuthorizationContext(token, _servicioSesionMock.Object);

        _filter!.OnAuthorization(context);

        var result = context.Result as ObjectResult;
        Assert.IsNotNull(result);
        Assert.AreEqual(500, result.StatusCode);
        var response = result.Value as ResponseDto;
        Assert.IsNotNull(response);
        Assert.IsFalse(response.ExecutionSuccessful);
        Assert.AreEqual("Ocurrió un error inesperado al procesar la solicitud", response.Message);
        _servicioSesionMock.VerifyAll();
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Constructor_ConRolNulo_DeberiaLanzarExcepcion()
    {
        _ = new AuthorizationFilter(null!);
    }

    private static AuthorizationFilterContext CreateAuthorizationContext(string? token, IServicioSesion? servicioSesion)
    {
        var httpContext = new DefaultHttpContext();

        if(!string.IsNullOrWhiteSpace(token))
        {
            httpContext.Request.Headers["Authorization"] = token;
        }

        if(servicioSesion != null)
        {
            var services = new ServiceCollection();
            services.AddSingleton(servicioSesion);
            httpContext.RequestServices = services.BuildServiceProvider();
        }

        var actionContext = new ActionContext(
            httpContext,
            new RouteData(),
            new ActionDescriptor());

        return new AuthorizationFilterContext(actionContext, []);
    }
}
