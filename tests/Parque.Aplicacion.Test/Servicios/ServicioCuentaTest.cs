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

        var mockPasswordHashService = new Mock<IPasswordHashService>();
        mockPasswordHashService
            .Setup(s => s.HashPassword("password123"))
            .Returns(new PasswordHash("hash123"));

        var cuentaServicio = new ServicioCuenta(mockRepo.Object, mockPasswordHashService.Object);

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
        var cuentaExistente = Cuenta.Crear("Juan", "Pérez", new Email("juan@test.com"), new PasswordHash("hash1234"));
        mockRepo.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Cuenta, bool>>>()))
            .Returns(cuentaExistente);

        var mockPasswordHashService = new Mock<IPasswordHashService>();

        var servicio = new ServicioCuenta(mockRepo.Object, mockPasswordHashService.Object);
        var dto = new RegistrarVisitanteDto("Juan", "Pérez", "juan@test.com", "pass", DateTime.Now);

        // Act & Assert
        var ex = Assert.ThrowsException<ExcepcionDominio>(() => servicio.RegistrarVisitante(dto));
        Assert.AreEqual("Ya existe una cuenta con este email.", ex.Message);
    }
}
