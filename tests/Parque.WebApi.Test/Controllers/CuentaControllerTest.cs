using Microsoft.AspNetCore.Mvc;
using Moq;
using Parque.Aplicacion.DTOs.Usuarios;
using Parque.Aplicacion.Servicios;
using Parque.Dominio.Usuarios;
using Parque.WebApi.Controllers.Usuarios;
using Parque.WebApi.Filtros;

namespace Parque.WebApi.Test.Controllers;

[TestClass]
public class CuentaControllerTest
{
    private Mock<IServicioCuenta>? _serviceMock;
    private CuentaController? _controller;

    [TestInitialize]
    public void Initialize()
    {
        _serviceMock = new Mock<IServicioCuenta>(MockBehavior.Strict);
        _controller = new CuentaController(_serviceMock.Object);
    }

    [TestMethod]
    public void RegistrarVisitante_CuandoDatosValidos_DeberiaRetornarCreated()
    {
        // Arrange
        var dto = new RegistrarVisitanteDto(
            "Juan",
            "Pérez",
            "juan.perez@email.com",
            "Password123!",
            new DateTime(1990, 5, 15));

        var cuentaCreada = new CuentaDto(
            Guid.NewGuid(),
            "Juan",
            "Pérez",
            "juan.perez@email.com",
            ["Visitante"],
            null);

        _serviceMock!.Setup(s => s.RegistrarVisitante(dto)).Returns(cuentaCreada);

        // Act
        var result = _controller!.RegistrarVisitante(dto);

        // Assert
        Assert.IsInstanceOfType(result, typeof(CreatedResult));
        var createdResult = result as CreatedResult;
        Assert.IsNotNull(createdResult);
        Assert.AreEqual($"/api/cuentas/{cuentaCreada.Id}", createdResult.Location);

        var response = createdResult.Value as ResponseDto;
        Assert.IsNotNull(response);
        Assert.IsTrue(response.ExecutionSuccessful);
        Assert.AreEqual("Visitante registrado exitosamente", response.Message);
        Assert.AreEqual(cuentaCreada, response.Content);

        _serviceMock.VerifyAll();
    }

    [TestMethod]
    public void RegistrarVisitante_DeberiaInvocarServicioConDtoCorrecto()
    {
        // Arrange
        var dto = new RegistrarVisitanteDto(
            "María",
            "González",
            "maria.gonzalez@email.com",
            "SecurePass456!",
            new DateTime(1995, 8, 20));

        var cuentaCreada = new CuentaDto(
            Guid.NewGuid(),
            dto.Nombre,
            dto.Apellido,
            dto.Email,
            ["Visitante"],
            null);

        _serviceMock!.Setup(s => s.RegistrarVisitante(It.Is<RegistrarVisitanteDto>(
            d => d.Nombre == dto.Nombre &&
                 d.Apellido == dto.Apellido &&
                 d.Email == dto.Email &&
                 d.Password == dto.Password &&
                 d.FechaNacimiento == dto.FechaNacimiento)))
            .Returns(cuentaCreada);

        // Act
        var result = _controller!.RegistrarVisitante(dto);

        // Assert
        Assert.IsInstanceOfType(result, typeof(CreatedResult));
        _serviceMock.Verify(s => s.RegistrarVisitante(It.IsAny<RegistrarVisitanteDto>()), Times.Once);
    }

    [TestMethod]
    public void RegistrarVisitante_DeberiaRetornarLocationConIdCorrecto()
    {
        // Arrange
        var expectedId = Guid.NewGuid();
        var dto = new RegistrarVisitanteDto(
            "Carlos",
            "Rodríguez",
            "carlos.rodriguez@email.com",
            "MyPass789!",
            new DateTime(1988, 3, 10));

        var cuentaCreada = new CuentaDto(
            expectedId,
            dto.Nombre,
            dto.Apellido,
            dto.Email,
            ["Visitante"],
            null);

        _serviceMock!.Setup(s => s.RegistrarVisitante(dto)).Returns(cuentaCreada);

        // Act
        var result = _controller!.RegistrarVisitante(dto);

        // Assert
        var createdResult = result as CreatedResult;
        Assert.IsNotNull(createdResult);
        Assert.AreEqual($"/api/cuentas/{expectedId}", createdResult.Location);

        var response = createdResult.Value as ResponseDto;
        var cuentaResponse = response?.Content as CuentaDto;
        Assert.IsNotNull(cuentaResponse);
        Assert.AreEqual(expectedId, cuentaResponse.Id);
    }

    #region CrearCuenta Tests

    [TestMethod]
    public void CrearCuenta_CuandoDatosValidos_DeberiaRetornarCreated()
    {
        // Arrange
        var dto = new RegistrarCuentaDto(
            "Admin",
            "Sistema",
            "admin@parque.com",
            "AdminPass123!",
            Rol.Administrador,
            null,
            null);

        var cuentaCreada = new CuentaDto(
            Guid.NewGuid(),
            "Admin",
            "Sistema",
            "admin@parque.com",
            ["Administrador"],
            null);

        _serviceMock!.Setup(s => s.CrearCuentaPorAdmin(dto)).Returns(cuentaCreada);

        // Act
        var result = _controller!.CrearCuenta(dto);

        // Assert
        Assert.IsInstanceOfType(result, typeof(CreatedResult));
        var createdResult = result as CreatedResult;
        Assert.IsNotNull(createdResult);
        Assert.AreEqual($"/api/cuentas/{cuentaCreada.Id}", createdResult.Location);

        var response = createdResult.Value as ResponseDto;
        Assert.IsNotNull(response);
        Assert.IsTrue(response.ExecutionSuccessful);
        Assert.AreEqual("Cuenta creada exitosamente", response.Message);
        Assert.AreEqual(cuentaCreada, response.Content);

        _serviceMock.VerifyAll();
    }

    [TestMethod]
    public void CrearCuenta_ConRolOperador_DeberiaCrearCuentaCorrectamente()
    {
        // Arrange
        var dto = new RegistrarCuentaDto(
            "Pedro",
            "Martínez",
            "pedro.martinez@parque.com",
            "OperadorPass456!",
            Rol.Operador,
            new DateTime(1985, 7, 20),
            null);

        var cuentaCreada = new CuentaDto(
            Guid.NewGuid(),
            dto.Nombre,
            dto.Apellido,
            dto.Email,
            ["Operador"],
            null);

        _serviceMock!.Setup(s => s.CrearCuentaPorAdmin(It.Is<RegistrarCuentaDto>(
            d => d.Nombre == dto.Nombre &&
                 d.Apellido == dto.Apellido &&
                 d.Email == dto.Email &&
                 d.Password == dto.Password &&
                 d.Rol == dto.Rol &&
                 d.FechaNacimiento == dto.FechaNacimiento &&
                 d.NivelMembresia == dto.NivelMembresia)))
            .Returns(cuentaCreada);

        // Act
        var result = _controller!.CrearCuenta(dto);

        // Assert
        Assert.IsInstanceOfType(result, typeof(CreatedResult));
        var createdResult = result as CreatedResult;
        var response = createdResult?.Value as ResponseDto;
        var cuenta = response?.Content as CuentaDto;

        Assert.IsNotNull(cuenta);
        Assert.IsTrue(cuenta.Roles.Contains("Operador"));
        _serviceMock.Verify(s => s.CrearCuentaPorAdmin(It.IsAny<RegistrarCuentaDto>()), Times.Once);
    }

        [TestMethod]
    public void CrearCuenta_VisitanteConMembresiaPremium_DeberiaCrearCuentaConVisitante()
    {
        // Arrange
        var dto = new RegistrarCuentaDto(
            "Laura",
            "Fernández",
            "laura.fernandez@email.com",
            "VisitantePass789!",
            Rol.Visitante,
            new DateTime(1992, 11, 5),
            NivelMembresia.Premium);

        var fechaNacimiento = new DateTime(1992, 11, 5);
        var edad = DateTime.Now.Year - fechaNacimiento.Year;

        var visitanteDto = new VisitanteDto(
            Guid.NewGuid(),
            fechaNacimiento,
            edad,
            "Premium",
            0,
            0);

        var cuentaCreada = new CuentaDto(
            Guid.NewGuid(),
            dto.Nombre,
            dto.Apellido,
            dto.Email,
            ["Visitante"],
            visitanteDto);

        _serviceMock!.Setup(s => s.CrearCuentaPorAdmin(dto)).Returns(cuentaCreada);

        // Act
        var result = _controller!.CrearCuenta(dto);

        // Assert
        var createdResult = result as CreatedResult;
        Assert.IsNotNull(createdResult);

        var response = createdResult.Value as ResponseDto;
        var cuenta = response?.Content as CuentaDto;

        Assert.IsNotNull(cuenta);
        Assert.IsNotNull(cuenta.Visitante);
        Assert.AreEqual(NivelMembresia.Premium.ToString(), cuenta.Visitante.NivelMembresia.ToString());
        Assert.AreEqual(dto.FechaNacimiento, cuenta.Visitante.FechaNacimiento);
        Assert.AreEqual("Cuenta creada exitosamente", response!.Message);

        _serviceMock.Verify(s => s.CrearCuentaPorAdmin(dto), Times.Once);
    }

    [TestMethod]
    public void CrearCuenta_DeberiaRetornarLocationConIdCorrecto()
    {
        // Arrange
        var expectedId = Guid.NewGuid();
        var dto = new RegistrarCuentaDto(
            "Roberto",
            "Silva",
            "roberto.silva@parque.com",
            "StrongPass999!",
            Rol.Administrador,
            null,
            null);

        var cuentaCreada = new CuentaDto(
            expectedId,
            dto.Nombre,
            dto.Apellido,
            dto.Email,
            ["Administrador"],
            null);

        _serviceMock!.Setup(s => s.CrearCuentaPorAdmin(dto)).Returns(cuentaCreada);

        // Act
        var result = _controller!.CrearCuenta(dto);

        // Assert
        var createdResult = result as CreatedResult;
        Assert.IsNotNull(createdResult);
        Assert.AreEqual($"/api/cuentas/{expectedId}", createdResult.Location);

        var response = createdResult.Value as ResponseDto;
        var cuenta = response?.Content as CuentaDto;
        Assert.IsNotNull(cuenta);
        Assert.AreEqual(expectedId, cuenta.Id);
    }

    #endregion
}
