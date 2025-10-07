using Microsoft.AspNetCore.Mvc;
using Moq;
using Parque.Aplicacion.DTOs.Usuarios;
using Parque.Aplicacion.Servicios;
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
}
