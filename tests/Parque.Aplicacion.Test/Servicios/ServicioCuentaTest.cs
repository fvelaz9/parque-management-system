using System.Linq.Expressions;
using Moq;
using Parque.Aplicacion.DTOs.Usuarios;
using Parque.Aplicacion.Servicios;
using Parque.Dominio.Excepciones;
using Parque.Dominio.Usuarios;
using Parque.Infraestructura.Repositorios;

namespace Parque.Aplicacion.Test.Servicios;

[TestClass]
public class ServicioCuentaTest
{
    [TestMethod]
    public void RegistrarVisitante_DatosValidos_CreaCuentaCorrecta()
    {
        // Arrange
        var mockRepo = new Mock<IRepositorio<Cuenta>>();
        mockRepo.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Cuenta, bool>>>()))
            .Returns((Cuenta)null!);

        var cuentaServicio = new ServicioCuenta(mockRepo.Object);

        var dto = new RegistrarVisitanteDto(
            "Juan",
            "Pérez",
            "juan@test.com",
            "password123",
            new DateTime(1990, 1, 1));

        // Act
        var resultado = cuentaServicio.RegistrarVisitante(dto);

        // Assert
        Assert.IsNotNull(resultado);
        Assert.AreEqual(dto.Nombre, resultado.Nombre);
        Assert.AreEqual(dto.Apellido, resultado.Apellido);
        Assert.AreEqual(dto.Email, resultado.Email);
        Assert.IsNotNull(resultado.Visitante);
        Assert.AreEqual(NivelMembresia.Estandar.ToString(), resultado.Visitante.NivelMembresia);

        mockRepo.Verify(r => r.Agregar(It.IsAny<Cuenta>()), Times.Once);
    }

    [TestMethod]
    public void RegistrarVisitante_EmailDuplicado_LanzaExcepcion()
    {
        // Arrange
        var mockRepo = new Mock<IRepositorio<Cuenta>>();
        var cuentaExistente = Cuenta.Crear("Juan", "Pérez", new Email("juan@test.com"), "password123");
        mockRepo.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Cuenta, bool>>>()))
            .Returns(cuentaExistente);

        var servicio = new ServicioCuenta(mockRepo.Object);
        var dto = new RegistrarVisitanteDto("Juan", "Pérez", "juan@test.com", "pass", DateTime.Now);

        // Act & Assert
        var ex = Assert.ThrowsException<ExcepcionDominio>(() => servicio.RegistrarVisitante(dto));
        Assert.AreEqual("Ya existe una cuenta con este email.", ex.Message);
    }

    [TestMethod]
    public void RegistrarVisitante_EmailInvalido_LanzaExcepcion()
    {
        // Arrange
        var mockRepo = new Mock<IRepositorio<Cuenta>>();
        mockRepo.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Cuenta, bool>>>()))
            .Returns((Cuenta)null!);

        var servicio = new ServicioCuenta(mockRepo.Object);
        var dto = new RegistrarVisitanteDto("Juan", "Pérez", "email-invalido", "password123", DateTime.Now);

        // Act & Assert
        var ex = Assert.ThrowsException<ExcepcionDominio>(() => servicio.RegistrarVisitante(dto));
    }

    [TestMethod]
    public void RegistrarVisitante_NombreVacio_LanzaExcepcion()
    {
        // Arrange
        var mockRepo = new Mock<IRepositorio<Cuenta>>();
        var servicio = new ServicioCuenta(mockRepo.Object);
        var dto = new RegistrarVisitanteDto(string.Empty, "Pérez", "juan@test.com", "password123", DateTime.Now);

        // Act & Assert
        Assert.ThrowsException<ExcepcionDominio>(() => servicio.RegistrarVisitante(dto));
    }

    [TestMethod]
    public void RegistrarVisitante_ApellidoVacio_LanzaExcepcion()
    {
        // Arrange
        var mockRepo = new Mock<IRepositorio<Cuenta>>();
        var servicio = new ServicioCuenta(mockRepo.Object);
        var dto = new RegistrarVisitanteDto("Juan", string.Empty, "juan@test.com", "password123", DateTime.Now);

        // Act & Assert
        Assert.ThrowsException<ExcepcionDominio>(() => servicio.RegistrarVisitante(dto));
    }

    [TestMethod]
    public void RegistrarVisitante_PasswordVacio_LanzaExcepcion()
    {
        // Arrange
        var mockRepo = new Mock<IRepositorio<Cuenta>>();
        var servicio = new ServicioCuenta(mockRepo.Object);
        var dto = new RegistrarVisitanteDto("Juan", "Pérez", "juan@test.com", string.Empty, DateTime.Now);

        // Act & Assert
        Assert.ThrowsException<ExcepcionDominio>(() => servicio.RegistrarVisitante(dto));
    }

    [TestMethod]
    public void ModificarPerfil_TodosLosCampos_ActualizaCorrectamente()
    {
        // Arrange
        var cuenta = Cuenta.Crear("Juan", "Pérez", new Email("juan@test.com"), "password123");
        cuenta.AsignarVisitante(new DateTime(1990, 1, 1));

        var mockRepo = new Mock<IRepositorio<Cuenta>>();
        mockRepo.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Cuenta, bool>>>()))
            .Returns(cuenta);

        var servicio = new ServicioCuenta(mockRepo.Object);
        var dto = new ModificarPerfilDto("Carlos", "Gómez", "carlos@test.com", new DateTime(1985, 5, 15));

        // Act
        servicio.ModificarPerfil(cuenta.Id, dto);

        // Assert
        Assert.AreEqual("Carlos", cuenta.Nombre);
        Assert.AreEqual("Gómez", cuenta.Apellido);
        Assert.AreEqual("carlos@test.com", cuenta.Email.Valor);
        Assert.AreEqual(new DateTime(1985, 5, 15), cuenta.Visitante!.FechaNacimiento);
        mockRepo.Verify(r => r.Editar(cuenta), Times.Once);
    }

    [TestMethod]
    public void ModificarPerfil_SoloNombre_ActualizaSoloNombre()
    {
        // Arrange
        var cuenta = Cuenta.Crear("Juan", "Pérez", new Email("juan@test.com"), "password123");
        cuenta.AsignarVisitante(new DateTime(1990, 1, 1));
        var apellidoOriginal = cuenta.Apellido;
        var emailOriginal = cuenta.Email.Valor;
        var fechaOriginal = cuenta.Visitante!.FechaNacimiento;

        var mockRepo = new Mock<IRepositorio<Cuenta>>();
        mockRepo.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Cuenta, bool>>>()))
            .Returns(cuenta);

        var servicio = new ServicioCuenta(mockRepo.Object);
        var dto = new ModificarPerfilDto("Carlos", null, null, null);

        // Act
        servicio.ModificarPerfil(cuenta.Id, dto);

        // Assert
        Assert.AreEqual("Carlos", cuenta.Nombre);
        Assert.AreEqual(apellidoOriginal, cuenta.Apellido);
        Assert.AreEqual(emailOriginal, cuenta.Email.Valor);
        Assert.AreEqual(fechaOriginal, cuenta.Visitante.FechaNacimiento);
    }
}
