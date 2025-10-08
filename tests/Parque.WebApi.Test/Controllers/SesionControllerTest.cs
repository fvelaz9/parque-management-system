using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Parque.Aplicacion.Servicios;
using Parque.Dominio;
using Parque.Dominio.Excepciones;
using Parque.Dominio.Usuarios;
using Parque.WebApi.Controllers.Sesiones;
using Parque.WebApi.Controllers.Sesiones.Models;
using Parque.WebApi.Filtros;

namespace Parque.WebApi.Test.Controllers;

[TestClass]
public class SesionControllerTest
{
    private Mock<IServicioSesion>? _servicioMock;
    private SesionController? _controller;

    [TestInitialize]
    public void Initialize()
    {
        _servicioMock = new Mock<IServicioSesion>(MockBehavior.Strict);
        _controller = new SesionController(_servicioMock.Object);
    }

    #region Login Tests

    [TestMethod]
    public void Login_CredencialesValidas_DeberiaRetornarOkConToken()
    {
        // Arrange
        var loginRequest = new LoginRequest
        {
            Email = "test@example.com",
            Password = "Password123!"
        };

        var token = Guid.NewGuid().ToString();
        var sesion = new Sesion
        {
            Token = token,
            UsuarioId = Guid.NewGuid()
        };

        var cuenta = Cuenta.Crear("Juan", "Pérez", new Email(loginRequest.Email), loginRequest.Password, Rol.Visitante);

        _servicioMock!.Setup(s => s.AgregarSesion(loginRequest.Email, loginRequest.Password))
            .Returns(sesion);
        _servicioMock.Setup(s => s.ObtenerUsuarioSesion(token))
            .Returns(cuenta);

        // Act
        var result = _controller!.Login(loginRequest);

        // Assert
        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);

        var responseDto = okResult.Value as ResponseDto;
        Assert.IsNotNull(responseDto);
        Assert.IsTrue(responseDto.ExecutionSuccessful);
        Assert.AreEqual("Login exitoso", responseDto.Message);

        var loginResponse = responseDto.Content as LoginResponse;
        Assert.IsNotNull(loginResponse);
        Assert.AreEqual(token, loginResponse.Token);
        Assert.IsNotNull(loginResponse.Cuenta);
        Assert.AreEqual(cuenta.Email.Valor, loginResponse.Cuenta.Email);

        _servicioMock.Verify(s => s.AgregarSesion(loginRequest.Email, loginRequest.Password), Times.Once);
        _servicioMock.Verify(s => s.ObtenerUsuarioSesion(token), Times.Once);
    }

    [TestMethod]
    public void Login_EmailIncorrecto_DeberiaLanzarExcepcion()
    {
        // Arrange
        var loginRequest = new LoginRequest
        {
            Email = "noexiste@example.com",
            Password = "Password123!"
        };

        _servicioMock!.Setup(s => s.AgregarSesion(loginRequest.Email, loginRequest.Password))
            .Throws(new ExcepcionDominio("Email no encontrado"));

        // Act & Assert
        Assert.ThrowsException<ExcepcionDominio>(() => _controller!.Login(loginRequest));
        _servicioMock.Verify(s => s.AgregarSesion(loginRequest.Email, loginRequest.Password), Times.Once);
    }

    [TestMethod]
    public void Login_PasswordIncorrecto_DeberiaLanzarExcepcion()
    {
        // Arrange
        var loginRequest = new LoginRequest
        {
            Email = "test@example.com",
            Password = "WrongPassword"
        };

        _servicioMock!.Setup(s => s.AgregarSesion(loginRequest.Email, loginRequest.Password))
            .Throws(new ExcepcionDominio("Password incorrecto"));

        // Act & Assert
        Assert.ThrowsException<ExcepcionDominio>(() => _controller!.Login(loginRequest));
        _servicioMock.Verify(s => s.AgregarSesion(loginRequest.Email, loginRequest.Password), Times.Once);
    }

    [TestMethod]
    public void Login_DeberiaRetornarCuentaConRoles()
    {
        // Arrange
        var loginRequest = new LoginRequest
        {
            Email = "admin@example.com",
            Password = "AdminPass123!"
        };

        var token = Guid.NewGuid().ToString();
        var sesion = new Sesion
        {
            Token = token,
            UsuarioId = Guid.NewGuid()
        };

        var cuenta = Cuenta.Crear("Admin", "User", new Email(loginRequest.Email), loginRequest.Password, Rol.Administrador);

        _servicioMock!.Setup(s => s.AgregarSesion(loginRequest.Email, loginRequest.Password))
            .Returns(sesion);
        _servicioMock.Setup(s => s.ObtenerUsuarioSesion(token))
            .Returns(cuenta);

        // Act
        var result = _controller!.Login(loginRequest);

        // Assert
        var okResult = result as OkObjectResult;
        var responseDto = okResult?.Value as ResponseDto;
        var loginResponse = responseDto?.Content as LoginResponse;

        Assert.IsNotNull(loginResponse);
        Assert.IsNotNull(loginResponse.Cuenta);
        Assert.IsTrue(loginResponse.Cuenta.Roles.Any());
        Assert.IsTrue(loginResponse.Cuenta.Roles.Contains("Administrador"));
    }

    #endregion

    #region Logout Tests

    [TestMethod]
    public void Logout_TokenValido_DeberiaRetornarOk()
    {
        // Arrange
        var token = Guid.NewGuid().ToString();
        _servicioMock!.Setup(s => s.EliminarSesion(token));

        // Simular el header Authorization
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers["Authorization"] = token;
        _controller!.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        // Act
        var result = _controller.Logout();

        // Assert
        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);

        var responseDto = okResult.Value as ResponseDto;
        Assert.IsNotNull(responseDto);
        Assert.IsTrue(responseDto.ExecutionSuccessful);
        Assert.AreEqual("Sesión cerrada correctamente", responseDto.Message);

        _servicioMock.Verify(s => s.EliminarSesion(token), Times.Once);
    }

    [TestMethod]
    public void Logout_TokenInvalido_DeberiaLanzarExcepcion()
    {
        // Arrange
        var token = "token-invalido";
        _servicioMock!.Setup(s => s.EliminarSesion(token))
            .Throws(new ExcepcionDominio("Token inválido"));

        // Simular el header Authorization
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers["Authorization"] = token;
        _controller!.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        // Act & Assert
        Assert.ThrowsException<ExcepcionDominio>(_controller.Logout);
        _servicioMock.Verify(s => s.EliminarSesion(token), Times.Once);
    }

    [TestMethod]
    public void Logout_SinToken_DeberiaUsarTokenVacio()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        _controller!.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        _servicioMock!.Setup(s => s.EliminarSesion(string.Empty))
            .Throws(new ExcepcionDominio("Token inválido"));

        // Act & Assert
        Assert.ThrowsException<ExcepcionDominio>(_controller.Logout);
    }

    #endregion
}
