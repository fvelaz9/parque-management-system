using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Parque.Aplicacion.DTOs.Usuarios;
using Parque.Aplicacion.Servicios;
using Parque.Dominio.Excepciones;
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

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
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
            [Rol.Administrador],
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
            [Rol.Operador],
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
                 d.Roles.SequenceEqual(dto.Roles) &&
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
            [Rol.Visitante],
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
            [Rol.Administrador],
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

    #region ModificarPerfil Tests

    [TestMethod]
    public void ModificarPerfil_UsuarioModificaSuPropioPerfil_DeberiaRetornarOk()
    {
        // Arrange
        var cuentaId = Guid.NewGuid();
        var cuenta = Cuenta.Crear("Juan", "Pérez", new Email("juan@test.com"), "pass123", Rol.Visitante);
        typeof(Cuenta).GetProperty("Id")!.SetValue(cuenta, cuentaId);

        // Simular que el usuario autenticado es el mismo que quiere modificar
        _controller!.HttpContext.Items["user"] = cuenta;

        var dto = new ModificarPerfilDto("Juan Carlos", "Pérez González", null, null);
        _serviceMock!.Setup(s => s.ModificarPerfil(cuentaId, dto));

        // Act
        var result = _controller.ModificarPerfil(dto);

        // Assert
        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        var okResult = result as OkObjectResult;
        var response = okResult?.Value as ResponseDto;

        Assert.IsNotNull(response);
        Assert.IsTrue(response.ExecutionSuccessful);
        Assert.AreEqual("Perfil modificado exitosamente", response.Message);

        _serviceMock.Verify(s => s.ModificarPerfil(cuentaId, dto), Times.Once);
    }

    [TestMethod]
    public void ModificarPerfil_UsuarioNoAutenticado_DeberiaRetornarUnauthorized()
    {
        // Arrange
        var cuentaId = Guid.NewGuid();
        _controller!.HttpContext.Items["user"] = null; // Usuario no autenticado

        var dto = new ModificarPerfilDto("Test", null, null, null);

        // Act
        var result = _controller.ModificarPerfil(dto);

        // Assert
        Assert.IsInstanceOfType(result, typeof(UnauthorizedObjectResult));
        var unauthorizedResult = result as UnauthorizedObjectResult;
        var response = unauthorizedResult?.Value as ResponseDto;

        Assert.IsNotNull(response);
        Assert.IsFalse(response.ExecutionSuccessful);
        Assert.AreEqual("No se pudo identificar al usuario autenticado", response.Message);

        _serviceMock!.Verify(s => s.ModificarPerfil(It.IsAny<Guid>(), It.IsAny<ModificarPerfilDto>()), Times.Never);
    }

    #endregion

    [TestMethod]
    public void CambiarNivelMembresia_APremium_RetornaOk()
    {
        // Arrange
        var mockServicio = new Mock<IServicioCuenta>();
        var controller = new CuentaController(mockServicio.Object);

        var cuentaId = Guid.NewGuid();
        var nuevoNivel = NivelMembresia.Premium;

        // Act
        var resultado = controller.CambiarNivelMembresia(cuentaId, nuevoNivel) as OkObjectResult;

        // Assert
        Assert.IsNotNull(resultado);
        Assert.AreEqual(200, resultado.StatusCode);

        var response = resultado.Value as ResponseDto;
        Assert.IsNotNull(response);
        Assert.IsTrue(response.ExecutionSuccessful);
        Assert.AreEqual("Nivel de membresía actualizado exitosamente", response.Message);

        mockServicio.Verify(s => s.CambiarNivelMembresia(cuentaId, nuevoNivel), Times.Once);
    }

    [TestMethod]
    public void CambiarNivelMembresia_AVIP_RetornaOk()
    {
        // Arrange
        var mockServicio = new Mock<IServicioCuenta>();
        var controller = new CuentaController(mockServicio.Object);

        var cuentaId = Guid.NewGuid();
        var nuevoNivel = NivelMembresia.VIP;

        // Act
        var resultado = controller.CambiarNivelMembresia(cuentaId, nuevoNivel) as OkObjectResult;

        // Assert
        Assert.IsNotNull(resultado);
        Assert.AreEqual(200, resultado.StatusCode);
        mockServicio.Verify(s => s.CambiarNivelMembresia(cuentaId, nuevoNivel), Times.Once);
    }

    [TestMethod]
    public void CambiarNivelMembresia_AEstandar_RetornaOk()
    {
        // Arrange
        var mockServicio = new Mock<IServicioCuenta>();
        var controller = new CuentaController(mockServicio.Object);

        var cuentaId = Guid.NewGuid();
        var nuevoNivel = NivelMembresia.Estandar;

        // Act
        var resultado = controller.CambiarNivelMembresia(cuentaId, nuevoNivel) as OkObjectResult;

        // Assert
        Assert.IsNotNull(resultado);
        Assert.AreEqual(200, resultado.StatusCode);
        mockServicio.Verify(s => s.CambiarNivelMembresia(cuentaId, nuevoNivel), Times.Once);
    }

    [TestMethod]
    public void CambiarNivelMembresia_CuentaSinVisitante_LanzaExcepcion()
    {
        // Arrange
        var mockServicio = new Mock<IServicioCuenta>();
        mockServicio.Setup(s => s.CambiarNivelMembresia(It.IsAny<Guid>(), It.IsAny<NivelMembresia>()))
            .Throws(new ExcepcionDominio("Solo las cuentas con perfil de visitante tienen nivel de membresía."));

        var controller = new CuentaController(mockServicio.Object);
        var cuentaId = Guid.NewGuid();

        // Act & Assert
        Assert.ThrowsException<ExcepcionDominio>(
            () => controller.CambiarNivelMembresia(cuentaId, NivelMembresia.Premium));
    }

    [TestMethod]
    public void CambiarNivelMembresia_CuentaNoExiste_LanzaExcepcion()
    {
        // Arrange
        var mockServicio = new Mock<IServicioCuenta>();
        mockServicio.Setup(s => s.CambiarNivelMembresia(It.IsAny<Guid>(), It.IsAny<NivelMembresia>()))
            .Throws(new ExcepcionEntidadNoEncontrada("Cuenta no encontrada"));

        var controller = new CuentaController(mockServicio.Object);
        var cuentaId = Guid.NewGuid();

        // Act & Assert
        Assert.ThrowsException<ExcepcionEntidadNoEncontrada>(
            () => controller.CambiarNivelMembresia(cuentaId, NivelMembresia.Premium));
    }

    [TestMethod]
    public void RegistrarVisitante_DatosValidos_RetornaCreated()
    {
        // Arrange
        var mockServicio = new Mock<IServicioCuenta>();
        var cuentaDto = new CuentaDto(
            Guid.NewGuid(),
            "Juan",
            "Pérez",
            "juan@test.com",
            ["Visitante"],
            null);

        mockServicio.Setup(s => s.RegistrarVisitante(It.IsAny<RegistrarVisitanteDto>()))
            .Returns(cuentaDto);

        var controller = new CuentaController(mockServicio.Object);
        var dto = new RegistrarVisitanteDto("Juan", "Pérez", "juan@test.com", "pass123", DateTime.Now);

        // Act
        var resultado = controller.RegistrarVisitante(dto) as CreatedResult;

        // Assert
        Assert.IsNotNull(resultado);
        Assert.AreEqual(201, resultado.StatusCode);
        Assert.AreEqual($"/api/cuentas/{cuentaDto.Id}", resultado.Location);
    }

    [TestMethod]
    public void CrearCuenta_Administrador_RetornaCreated()
    {
        // Arrange
        var mockServicio = new Mock<IServicioCuenta>();
        var cuentaDto = new CuentaDto(
            Guid.NewGuid(),
            "Juan",
            "Pérez",
            "juan.perez@email.com",
            ["Visitante"],
            null);

        mockServicio.Setup(s => s.CrearCuentaPorAdmin(It.IsAny<RegistrarCuentaDto>()))
            .Returns(cuentaDto);

        var controller = new CuentaController(mockServicio.Object);
        var dto = new RegistrarCuentaDto("Admin", "Sistema", "admin@test.com", "pass123", [Rol.Administrador], null, null);

        // Act
        var resultado = controller.CrearCuenta(dto) as CreatedResult;

        // Assert
        Assert.IsNotNull(resultado);
        Assert.AreEqual(201, resultado.StatusCode);
    }

    #region Tests ObtenerTodas

    [TestMethod]
    public void ObtenerTodas_ConCuentasExistentes_RetornaOkConListaDeCuentas()
    {
        // Arrange
        var cuentas = new List<CuentaDto>
        {
            new CuentaDto(
                Guid.NewGuid(),
                "Juan",
                "Pérez",
                "juan@test.com",
                ["Visitante"],
                new VisitanteDto(Guid.NewGuid(), new DateTime(1990, 1, 1), 34, "Estandar", 0, 0)),
            new CuentaDto(
                Guid.NewGuid(),
                "María",
                "García",
                "maria@admin.com",
                ["Administrador"],
                null),
            new CuentaDto(
                Guid.NewGuid(),
                "Carlos",
                "López",
                "carlos@operador.com",
                ["Operador"],
                null)
        };

        _serviceMock!.Setup(s => s.ObtenerTodas()).Returns(cuentas);

        // Act
        var result = _controller!.ObtenerTodas();

        // Assert
        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);

        var response = okResult.Value as ResponseDto;
        Assert.IsNotNull(response);
        Assert.IsTrue(response.ExecutionSuccessful);
        Assert.AreEqual("Cuentas obtenidas exitosamente", response.Message);

        var cuentasResponse = response.Content as IEnumerable<CuentaDto>;
        Assert.IsNotNull(cuentasResponse);
        Assert.AreEqual(3, cuentasResponse.Count());

        _serviceMock.Verify(s => s.ObtenerTodas(), Times.Once);
    }

    [TestMethod]
    public void ObtenerTodas_SinCuentas_RetornaOkConListaVacia()
    {
        // Arrange
        var cuentasVacias = new List<CuentaDto>();
        _serviceMock!.Setup(s => s.ObtenerTodas()).Returns(cuentasVacias);

        // Act
        var result = _controller!.ObtenerTodas();

        // Assert
        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        var okResult = result as OkObjectResult;
        var response = okResult?.Value as ResponseDto;

        Assert.IsNotNull(response);
        Assert.IsTrue(response.ExecutionSuccessful);

        var cuentasResponse = response.Content as IEnumerable<CuentaDto>;
        Assert.IsNotNull(cuentasResponse);
        Assert.AreEqual(0, cuentasResponse.Count());

        _serviceMock.Verify(s => s.ObtenerTodas(), Times.Once);
    }

    [TestMethod]
    public void ObtenerTodas_ConVisitantesConDiferentesMembresias_RetornaTodasCorrectamente()
    {
        // Arrange
        var cuentas = new List<CuentaDto>
        {
            new CuentaDto(
                Guid.NewGuid(),
                "Ana",
                "Martínez",
                "ana@test.com",
                ["Visitante"],
                new VisitanteDto(Guid.NewGuid(), new DateTime(1992, 5, 15), 32, "Premium", 5, 150)),
            new CuentaDto(
                Guid.NewGuid(),
                "Luis",
                "Fernández",
                "luis@test.com",
                ["Visitante"],
                new VisitanteDto(Guid.NewGuid(), new DateTime(1985, 8, 20), 39, "VIP", 10, 300))
        };

        _serviceMock!.Setup(s => s.ObtenerTodas()).Returns(cuentas);

        // Act
        var result = _controller!.ObtenerTodas();

        // Assert
        var okResult = result as OkObjectResult;
        var response = okResult?.Value as ResponseDto;
        var cuentasResponse = (response?.Content as IEnumerable<CuentaDto>)?.ToList();

        Assert.IsNotNull(cuentasResponse);
        Assert.AreEqual(2, cuentasResponse.Count);
        Assert.AreEqual("Premium", cuentasResponse[0].Visitante?.NivelMembresia);
        Assert.AreEqual("VIP", cuentasResponse[1].Visitante?.NivelMembresia);
    }

    [TestMethod]
    public void ObtenerTodas_VerificaEstructuraDeRespuesta()
    {
        // Arrange
        var cuentas = new List<CuentaDto>
        {
            new CuentaDto(
                Guid.NewGuid(),
                "Test",
                "User",
                "test@test.com",
                ["Visitante"],
                null)
        };

        _serviceMock!.Setup(s => s.ObtenerTodas()).Returns(cuentas);

        // Act
        var result = _controller!.ObtenerTodas();

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);

        var response = okResult.Value as ResponseDto;
        Assert.IsNotNull(response);
        Assert.IsNotNull(response.Content);
        Assert.IsTrue(response.ExecutionSuccessful);
        Assert.IsFalse(string.IsNullOrWhiteSpace(response.Message));
    }

    [TestMethod]
    public void ObtenerTodas_InvocaServicioUnaVez()
    {
        // Arrange
        var cuentas = new List<CuentaDto>();
        _serviceMock!.Setup(s => s.ObtenerTodas()).Returns(cuentas);

        // Act
        _controller!.ObtenerTodas();

        // Assert
        _serviceMock.Verify(s => s.ObtenerTodas(), Times.Once);
        _serviceMock.VerifyNoOtherCalls();
    }

    [TestMethod]
    public void ObtenerTodas_ConCuentasConMultiplesRoles_RetornaRolesCorrectamente()
    {
        // Arrange
        var cuentas = new List<CuentaDto>
        {
            new CuentaDto(
                Guid.NewGuid(),
                "Multi",
                "Role",
                "multi@test.com",
                ["Administrador", "Operador"],
                null)
        };

        _serviceMock!.Setup(s => s.ObtenerTodas()).Returns(cuentas);

        // Act
        var result = _controller!.ObtenerTodas();

        // Assert
        var okResult = result as OkObjectResult;
        var response = okResult?.Value as ResponseDto;
        var cuentasResponse = (response?.Content as IEnumerable<CuentaDto>)?.ToList();

        Assert.IsNotNull(cuentasResponse);
        var cuenta = cuentasResponse.First();
        Assert.AreEqual(2, cuenta.Roles.Count());
        Assert.IsTrue(cuenta.Roles.Contains("Administrador"));
        Assert.IsTrue(cuenta.Roles.Contains("Operador"));
    }

    #endregion
}
