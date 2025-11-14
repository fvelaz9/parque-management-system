using System.Linq.Expressions;
using Moq;
using Parque.Aplicacion.Servicios;
using Parque.Dominio;
using Parque.Dominio.Excepciones;
using Parque.Dominio.Usuarios;
using Parque.Infraestructura.Repositorios;

namespace Parque.Aplicacion.Test.Servicios;

[TestClass]
public class ServicioSesionTest
{
    private Mock<IRepositorio<Sesion>>? _sesionRepoMock;
    private Mock<IRepositorio<Cuenta>>? _cuentaRepoMock;
    private ServicioSesion? _servicio;

    [TestInitialize]
    public void Initialize()
    {
        _sesionRepoMock = new Mock<IRepositorio<Sesion>>(MockBehavior.Strict);
        _cuentaRepoMock = new Mock<IRepositorio<Cuenta>>(MockBehavior.Strict);
        _servicio = new ServicioSesion(_sesionRepoMock.Object, _cuentaRepoMock.Object);
    }

    [TestMethod]
    public void AgregarSesion_CredencialesValidas_DeberiaCrearSesion()
    {
        // Arrange
        var email = "test@example.com";
        var password = "Password123!";
        var cuenta = Cuenta.Crear("Juan", "Pérez", new Email(email), password, Rol.Visitante);

        _cuentaRepoMock!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Cuenta, bool>>>()))
            .Returns(cuenta);
        _sesionRepoMock!.Setup(r => r.Agregar(It.IsAny<Sesion>()));

        // Act
        var sesion = _servicio!.AgregarSesion(email, password);

        // Assert
        Assert.IsNotNull(sesion);
        Assert.IsNotNull(sesion.Token);
        Assert.AreEqual(cuenta.Id, sesion.UsuarioId);
        _cuentaRepoMock.Verify(r => r.Encontrar(It.IsAny<Expression<Func<Cuenta, bool>>>()), Times.Once);
        _sesionRepoMock.Verify(r => r.Agregar(It.IsAny<Sesion>()), Times.Once);
    }

    [TestMethod]
    public void AgregarSesionEmailNoExisteDeberiaLanzarExcepcion()
    {
        // Arrange
        var email = "noexiste@example.com";
        var password = "Password123!";

        _cuentaRepoMock!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Cuenta, bool>>>()))
            .Returns((Cuenta?)null);

        // Act & Assert
        var exception = Assert.ThrowsException<ExcepcionDominio>(() =>
            _servicio!.AgregarSesion(email, password));

        Assert.AreEqual("Email no encontrado", exception.Message);
        _cuentaRepoMock.Verify(r => r.Encontrar(It.IsAny<Expression<Func<Cuenta, bool>>>()), Times.Once);
    }

    [TestMethod]
    public void AgregarSesionPasswordIncorrectoDeberiaLanzarExcepcion()
    {
        // Arrange
        var email = "test@example.com";
        var passwordCorrecto = "Password123!";
        var passwordIncorrecto = "WrongPassword";
        var cuenta = Cuenta.Crear("Juan", "Pérez", new Email(email), passwordCorrecto, Rol.Visitante);

        _cuentaRepoMock!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Cuenta, bool>>>()))
            .Returns(cuenta);

        // Act & Assert
        var exception = Assert.ThrowsException<ExcepcionDominio>(() =>
            _servicio!.AgregarSesion(email, passwordIncorrecto));

        Assert.AreEqual("Password incorrecto", exception.Message);
        _cuentaRepoMock.Verify(r => r.Encontrar(It.IsAny<Expression<Func<Cuenta, bool>>>()), Times.Once);
    }

    [TestMethod]
    public void AgregarSesionDeberiaGenerarTokenUnico()
    {
        // Arrange
        var email = "test@example.com";
        var password = "Password123!";
        var cuenta = Cuenta.Crear("Juan", "Pérez", new Email(email), password, Rol.Visitante);

        _cuentaRepoMock!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Cuenta, bool>>>()))
            .Returns(cuenta);
        _sesionRepoMock!.Setup(r => r.Agregar(It.IsAny<Sesion>()));

        // Act
        var sesion1 = _servicio!.AgregarSesion(email, password);
        var sesion2 = _servicio!.AgregarSesion(email, password);

        // Assert
        Assert.AreNotEqual(sesion1.Token, sesion2.Token);
    }

    [TestMethod]
    public void ObtenerUsuarioSesionTokenValidoDeberiaRetornarCuenta()
    {
        // Arrange
        var token = Guid.NewGuid().ToString();
        var cuenta = Cuenta.Crear("María", "González", new Email("maria@example.com"), "Pass123!", Rol.Visitante);
        var sesion = new Sesion
        {
            Token = token,
            UsuarioId = cuenta.Id
        };

        _sesionRepoMock!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Sesion, bool>>>()))
            .Returns(sesion);
        _cuentaRepoMock!.Setup(r => r.EncontrarConRelaciones(It.IsAny<Expression<Func<Cuenta, bool>>>(), It.IsAny<string[]>()))
            .Returns(cuenta);

        // Act
        var resultado = _servicio!.ObtenerUsuarioSesion(token);

        // Assert
        Assert.IsNotNull(resultado);
        Assert.AreEqual(cuenta.Id, resultado.Id);
        Assert.AreEqual(cuenta.Email.Valor, resultado.Email.Valor);
        _sesionRepoMock.Verify(r => r.Encontrar(It.IsAny<Expression<Func<Sesion, bool>>>()), Times.Once);
        _cuentaRepoMock.Verify(r => r.EncontrarConRelaciones(It.IsAny<Expression<Func<Cuenta, bool>>>(), It.IsAny<string[]>()), Times.Once);
    }

    [TestMethod]
    public void AgregarSesion_EmailNoExiste_DeberiaLanzarExcepcion()
    {
        // Arrange
        var email = "noexiste@example.com";
        var password = "Password123!";

        _cuentaRepoMock!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Cuenta, bool>>>()))
            .Returns((Cuenta?)null);

        // Act & Assert
        var exception = Assert.ThrowsException<ExcepcionDominio>(() =>
            _servicio!.AgregarSesion(email, password));

        Assert.AreEqual("Email no encontrado", exception.Message);
        _cuentaRepoMock.Verify(r => r.Encontrar(It.IsAny<Expression<Func<Cuenta, bool>>>()), Times.Once);
    }

    [TestMethod]
    public void AgregarSesion_PasswordIncorrecto_DeberiaLanzarExcepcion()
    {
        // Arrange
        var email = "test@example.com";
        var passwordCorrecto = "Password123!";
        var passwordIncorrecto = "WrongPassword";
        var cuenta = Cuenta.Crear("Juan", "Pérez", new Email(email), passwordCorrecto, Rol.Visitante);

        _cuentaRepoMock!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Cuenta, bool>>>()))
            .Returns(cuenta);

        // Act & Assert
        var exception = Assert.ThrowsException<ExcepcionDominio>(() =>
            _servicio!.AgregarSesion(email, passwordIncorrecto));

        Assert.AreEqual("Password incorrecto", exception.Message);
        _cuentaRepoMock.Verify(r => r.Encontrar(It.IsAny<Expression<Func<Cuenta, bool>>>()), Times.Once);
    }

    [TestMethod]
    public void AgregarSesion_DeberiaGenerarTokenUnico()
    {
        // Arrange
        var email = "test@example.com";
        var password = "Password123!";
        var cuenta = Cuenta.Crear("Juan", "Pérez", new Email(email), password, Rol.Visitante);

        _cuentaRepoMock!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Cuenta, bool>>>()))
            .Returns(cuenta);
        _sesionRepoMock!.Setup(r => r.Agregar(It.IsAny<Sesion>()));

        // Act
        var sesion1 = _servicio!.AgregarSesion(email, password);
        var sesion2 = _servicio!.AgregarSesion(email, password);

        // Assert
        Assert.AreNotEqual(sesion1.Token, sesion2.Token);
    }

    [TestMethod]
    public void ObtenerUsuarioSesion_TokenValido_DeberiaRetornarCuenta()
    {
        // Arrange
        var token = Guid.NewGuid().ToString();
        var cuenta = Cuenta.Crear("María", "González", new Email("maria@example.com"), "Pass123!", Rol.Visitante);
        var sesion = new Sesion
        {
            Token = token,
            UsuarioId = cuenta.Id
        };

        _sesionRepoMock!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Sesion, bool>>>()))
            .Returns(sesion);
        _cuentaRepoMock!.Setup(r => r.EncontrarConRelaciones(It.IsAny<Expression<Func<Cuenta, bool>>>(), It.IsAny<string[]>()))
            .Returns(cuenta);

        // Act
        var resultado = _servicio!.ObtenerUsuarioSesion(token);

        // Assert
        Assert.IsNotNull(resultado);
        Assert.AreEqual(cuenta.Id, resultado.Id);
        Assert.AreEqual(cuenta.Email.Valor, resultado.Email.Valor);
        _sesionRepoMock.Verify(r => r.Encontrar(It.IsAny<Expression<Func<Sesion, bool>>>()), Times.Once);
        _cuentaRepoMock.Verify(r => r.EncontrarConRelaciones(It.IsAny<Expression<Func<Cuenta, bool>>>(), It.IsAny<string[]>()), Times.Once);
    }

    [TestMethod]
    public void ObtenerUsuarioSesion_UsuarioNoExiste_DeberiaLanzarExcepcion()
    {
        // Arrange
        var token = Guid.NewGuid().ToString();
        var sesion = new Sesion
        {
            Token = token,
            UsuarioId = Guid.NewGuid()
        };

        _sesionRepoMock!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Sesion, bool>>>()))
            .Returns(sesion);
        _cuentaRepoMock!.Setup(r => r.EncontrarConRelaciones(It.IsAny<Expression<Func<Cuenta, bool>>>(), It.IsAny<string[]>()))
            .Returns((Cuenta?)null);

        // Act & Assert
        var exception = Assert.ThrowsException<ExcepcionDominio>(() =>
            _servicio!.ObtenerUsuarioSesion(token));

        Assert.AreEqual("Usuario no encontrado", exception.Message);
        _sesionRepoMock.Verify(r => r.Encontrar(It.IsAny<Expression<Func<Sesion, bool>>>()), Times.Once);
        _cuentaRepoMock.Verify(r => r.EncontrarConRelaciones(It.IsAny<Expression<Func<Cuenta, bool>>>(), It.IsAny<string[]>()), Times.Once);
    }

    [TestMethod]
    public void ValidarSesion_TokenValidoYRolAny_DeberiaRetornarTrue()
    {
        // Arrange
        var token = Guid.NewGuid().ToString();
        var cuenta = Cuenta.Crear("Carlos", "Rodríguez", new Email("carlos@example.com"), "Pass123!", Rol.Visitante);
        var sesion = new Sesion
        {
            Token = token,
            UsuarioId = cuenta.Id
        };

        _sesionRepoMock!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Sesion, bool>>>()))
            .Returns(sesion);
        _cuentaRepoMock!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Cuenta, bool>>>()))
            .Returns(cuenta);

        // Act
        var resultado = _servicio!.ValidarSesion(token, "any");

        // Assert
        Assert.IsTrue(resultado);
    }

    [TestMethod]
    public void ValidarSesion_TokenValidoYRolCorrecto_DeberiaRetornarTrue()
    {
        // Arrange
        var token = Guid.NewGuid().ToString();
        var cuenta = Cuenta.Crear("Admin", "User", new Email("admin@example.com"), "Pass123!", Rol.Administrador);
        var sesion = new Sesion
        {
            Token = token,
            UsuarioId = cuenta.Id
        };

        _sesionRepoMock!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Sesion, bool>>>()))
            .Returns(sesion);
        _cuentaRepoMock!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Cuenta, bool>>>()))
            .Returns(cuenta);

        // Act
        var resultado = _servicio!.ValidarSesion(token, "Administrador");

        // Assert
        Assert.IsTrue(resultado);
    }

    [TestMethod]
    public void ValidarSesion_TokenValidoPeroRolIncorrecto_DeberiaRetornarFalse()
    {
        // Arrange
        var token = Guid.NewGuid().ToString();
        var cuenta = Cuenta.Crear("Visitante", "User", new Email("visitante@example.com"), "Pass123!", Rol.Visitante);
        var sesion = new Sesion
        {
            Token = token,
            UsuarioId = cuenta.Id
        };

        _sesionRepoMock!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Sesion, bool>>>()))
            .Returns(sesion);
        _cuentaRepoMock!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Cuenta, bool>>>()))
            .Returns(cuenta);

        // Act
        var resultado = _servicio!.ValidarSesion(token, "Administrador");

        // Assert
        Assert.IsFalse(resultado);
    }

    [TestMethod]
    public void ValidarSesion_TokenInvalido_DeberiaLanzarExcepcion()
    {
        // Arrange
        var token = "token-invalido";

        _sesionRepoMock!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Sesion, bool>>>()))
            .Returns((Sesion?)null);

        // Act & Assert
        var exception = Assert.ThrowsException<ExcepcionDominio>(() =>
            _servicio!.ValidarSesion(token, "Visitante"));

        Assert.AreEqual("Token inválido", exception.Message);
    }

    [TestMethod]
    public void ValidarSesion_RolInvalido_DeberiaRetornarFalse()
    {
        // Arrange
        var token = Guid.NewGuid().ToString();
        var cuenta = Cuenta.Crear("User", "Test", new Email("user@example.com"), "Pass123!", Rol.Visitante);
        var sesion = new Sesion
        {
            Token = token,
            UsuarioId = cuenta.Id
        };

        _sesionRepoMock!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Sesion, bool>>>()))
            .Returns(sesion);
        _cuentaRepoMock!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Cuenta, bool>>>()))
            .Returns(cuenta);

        // Act
        var resultado = _servicio!.ValidarSesion(token, "RolInexistente");

        // Assert
        Assert.IsFalse(resultado);
    }

    [TestMethod]
    public void EliminarSesion_TokenValido_DeberiaEliminarSesion()
    {
        // Arrange
        var token = Guid.NewGuid().ToString();
        var sesionId = Guid.NewGuid();
        var sesion = new Sesion
        {
            Id = sesionId,
            Token = token,
            UsuarioId = Guid.NewGuid()
        };

        _sesionRepoMock!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Sesion, bool>>>()))
            .Returns(sesion);
        _sesionRepoMock!.Setup(r => r.Eliminar(It.IsAny<Expression<Func<Sesion, bool>>>()));

        // Act
        _servicio!.EliminarSesion(token);

        // Assert
        _sesionRepoMock.Verify(r => r.Encontrar(It.IsAny<Expression<Func<Sesion, bool>>>()), Times.Once);
        _sesionRepoMock.Verify(r => r.Eliminar(It.IsAny<Expression<Func<Sesion, bool>>>()), Times.Once);
    }

    [TestMethod]
    public void EliminarSesion_TokenInvalido_DeberiaLanzarExcepcion()
    {
        // Arrange
        var token = "token-invalido";

        _sesionRepoMock!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Sesion, bool>>>()))
            .Returns((Sesion?)null);

        // Act & Assert
        var exception = Assert.ThrowsException<ExcepcionDominio>(() =>
            _servicio!.EliminarSesion(token));

        Assert.AreEqual("Token inválido", exception.Message);
        _sesionRepoMock.Verify(r => r.Encontrar(It.IsAny<Expression<Func<Sesion, bool>>>()), Times.Once);
    }
}
