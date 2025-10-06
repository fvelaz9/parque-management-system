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
        var cuentaExistente = Cuenta.Crear("Juan", "Pérez", new Email("juan@test.com"), "password123", Rol.Visitante);
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
        var cuenta = Cuenta.Crear("Juan", "Pérez", new Email("juan@test.com"), "password123", Rol.Visitante);
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
        var cuenta = Cuenta.Crear("Juan", "Pérez", new Email("juan@test.com"), "password123", Rol.Visitante);
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

    [TestMethod]
    public void ModificarPerfil_CuentaInexistente_DeberiaLanzarExcepcion()
    {
        // Arrange
        Cuenta? cuenta = null;

        var mockRepo = new Mock<IRepositorio<Cuenta>>();
        mockRepo.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Cuenta, bool>>>()))
            .Returns(cuenta);

        var servicio = new ServicioCuenta(mockRepo.Object);
        var dto = new ModificarPerfilDto("Carlos", null, null, null);

        // Act & Assert
        var ex = Assert.ThrowsException<ExcepcionEntidadNoEncontrada>(() => servicio.ModificarPerfil(Guid.NewGuid(), dto));
        Assert.AreEqual("Cuenta no encontrada", ex.Message);
    }

    [TestMethod]
    public void ModificarPerfil_FechaSinVisitante_NoLanzaExcepcion()
    {
        // Arrange
        var cuenta = Cuenta.Crear("Juan", "Pérez", new Email("juan@test.com"), "password123", Rol.Visitante);

        var mockRepo = new Mock<IRepositorio<Cuenta>>();
        mockRepo.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Cuenta, bool>>>()))
            .Returns(cuenta);

        var servicio = new ServicioCuenta(mockRepo.Object);
        var dto = new ModificarPerfilDto(null, null, null, new DateTime(1990, 1, 1));

        // Act - No debe lanzar excepción, simplemente ignora
        servicio.ModificarPerfil(cuenta.Id, dto);

        // Assert
        Assert.IsNull(cuenta.Visitante);
        mockRepo.Verify(r => r.Editar(cuenta), Times.Once);
    }

    [TestMethod]
    public void ModificarPerfil_EmailInvalido_LanzaExcepcion()
    {
        // Arrange
        var cuenta = Cuenta.Crear("Juan", "Pérez", new Email("juan@test.com"), "password123", Rol.Visitante);

        var mockRepo = new Mock<IRepositorio<Cuenta>>();
        mockRepo.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Cuenta, bool>>>()))
            .Returns(cuenta);

        var servicio = new ServicioCuenta(mockRepo.Object);
        var dto = new ModificarPerfilDto(null, null, "email-invalido", null);

        // Act & Assert
        Assert.ThrowsException<ExcepcionDominio>(
            () => servicio.ModificarPerfil(cuenta.Id, dto));
    }

    [TestMethod]
    public void CrearCuentaPorAdmin_Administrador_CreaCuentaCorrectamente()
    {
        // Arrange
        var mockRepo = new Mock<IRepositorio<Cuenta>>();
        mockRepo.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Cuenta, bool>>>()))
            .Returns((Cuenta)null!);

        var servicio = new ServicioCuenta(mockRepo.Object);
        var dto = new RegistrarCuentaDto(
            "María",
            "Rodríguez",
            "maria@admin.com",
            "admin123",
            Rol.Administrador,
            null,
            null);

        // Act
        var resultado = servicio.CrearCuentaPorAdmin(dto);

        // Assert
        Assert.IsNotNull(resultado);
        Assert.AreEqual("María", resultado.Nombre);
        Assert.AreEqual("Rodríguez", resultado.Apellido);
        Assert.AreEqual("maria@admin.com", resultado.Email);
        Assert.IsTrue(resultado.Roles.Contains(Rol.Administrador.ToString()));
        Assert.AreEqual(1, resultado.Roles.Count());
        Assert.IsNull(resultado.Visitante);

        mockRepo.Verify(r => r.Agregar(It.IsAny<Cuenta>()), Times.Once);
    }

    [TestMethod]
    public void CrearCuentaPorAdmin_Operador_CreaCuentaCorrectamente()
    {
        // Arrange
        var mockRepo = new Mock<IRepositorio<Cuenta>>();
        mockRepo.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Cuenta, bool>>>()))
            .Returns((Cuenta)null!);

        var servicio = new ServicioCuenta(mockRepo.Object);
        var dto = new RegistrarCuentaDto(
            "Carlos",
            "López",
            "carlos@operador.com",
            "operador123",
            Rol.Operador,
            null,
            null);

        // Act
        var resultado = servicio.CrearCuentaPorAdmin(dto);

        // Assert
        Assert.IsNotNull(resultado);
        Assert.AreEqual("Carlos", resultado.Nombre);
        Assert.AreEqual("López", resultado.Apellido);
        Assert.IsTrue(resultado.Roles.Contains(Rol.Operador.ToString()));
        Assert.AreEqual(1, resultado.Roles.Count());
        Assert.IsNull(resultado.Visitante);

        mockRepo.Verify(r => r.Agregar(It.IsAny<Cuenta>()), Times.Once);
    }

    [TestMethod]
    public void CrearCuentaPorAdmin_VisitanteEstandar_CreaCuentaConPerfilCorrectamente()
    {
        // Arrange
        var mockRepo = new Mock<IRepositorio<Cuenta>>();
        mockRepo.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Cuenta, bool>>>()))
            .Returns((Cuenta)null!);

        var servicio = new ServicioCuenta(mockRepo.Object);
        var fechaNacimiento = new DateTime(1990, 5, 15);
        var dto = new RegistrarCuentaDto(
            "Ana",
            "García",
            "ana@visitante.com",
            "visitante123",
            Rol.Visitante,
            fechaNacimiento,
            null);

        // Act
        var resultado = servicio.CrearCuentaPorAdmin(dto);

        // Assert
        Assert.IsNotNull(resultado);
        Assert.AreEqual("Ana", resultado.Nombre);
        Assert.IsTrue(resultado.Roles.Contains(Rol.Visitante.ToString()));
        Assert.AreEqual(1, resultado.Roles.Count());
        Assert.IsNotNull(resultado.Visitante);
        Assert.AreEqual(fechaNacimiento, resultado.Visitante.FechaNacimiento);
        Assert.AreEqual(NivelMembresia.Estandar.ToString(), resultado.Visitante.NivelMembresia);

        mockRepo.Verify(r => r.Agregar(It.IsAny<Cuenta>()), Times.Once);
    }

    [TestMethod]
    public void CrearCuentaPorAdmin_VisitantePremium_CreaCuentaConNivelCorrecto()
    {
        // Arrange
        var mockRepo = new Mock<IRepositorio<Cuenta>>();
        mockRepo.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Cuenta, bool>>>()))
            .Returns((Cuenta)null!);

        var servicio = new ServicioCuenta(mockRepo.Object);
        var fechaNacimiento = new DateTime(1985, 3, 20);
        var dto = new RegistrarCuentaDto(
            "Luis",
            "Fernández",
            "luis@premium.com",
            "premium123",
            Rol.Visitante,
            fechaNacimiento,
            NivelMembresia.Premium);

        // Act
        var resultado = servicio.CrearCuentaPorAdmin(dto);

        // Assert
        Assert.IsNotNull(resultado.Visitante);
        Assert.AreEqual(NivelMembresia.Premium.ToString(), resultado.Visitante.NivelMembresia);
    }

    [TestMethod]
    public void CrearCuentaPorAdmin_VisitanteVIP_CreaCuentaConNivelCorrecto()
    {
        // Arrange
        var mockRepo = new Mock<IRepositorio<Cuenta>>();
        mockRepo.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Cuenta, bool>>>()))
            .Returns((Cuenta)null!);

        var servicio = new ServicioCuenta(mockRepo.Object);
        var fechaNacimiento = new DateTime(1988, 7, 10);
        var dto = new RegistrarCuentaDto(
            "Sofía",
            "Martín",
            "sofia@vip.com",
            "vip123",
            Rol.Visitante,
            fechaNacimiento,
            NivelMembresia.VIP);

        // Act
        var resultado = servicio.CrearCuentaPorAdmin(dto);

        // Assert
        Assert.IsNotNull(resultado.Visitante);
        Assert.AreEqual(NivelMembresia.VIP.ToString(), resultado.Visitante.NivelMembresia);
    }

    [TestMethod]
    public void CrearCuentaPorAdmin_EmailDuplicado_LanzaExcepcion()
    {
        // Arrange
        var cuentaExistente = Cuenta.Crear("Usuario", "Existente",
            new Email("duplicado@test.com"), "password123", Rol.Visitante);

        var mockRepo = new Mock<IRepositorio<Cuenta>>();
        mockRepo.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Cuenta, bool>>>()))
            .Returns(cuentaExistente);

        var servicio = new ServicioCuenta(mockRepo.Object);
        var dto = new RegistrarCuentaDto(
            "Nuevo",
            "Usuario",
            "duplicado@test.com",
            "password123",
            Rol.Administrador,
            null,
            null);

        // Act & Assert
        var ex = Assert.ThrowsException<ExcepcionDominio>(
            () => servicio.CrearCuentaPorAdmin(dto));

        Assert.AreEqual("Ya existe una cuenta con este email.", ex.Message);
    }

    [TestMethod]
    public void CrearCuentaPorAdmin_EmailInvalido_LanzaExcepcion()
    {
        // Arrange
        var mockRepo = new Mock<IRepositorio<Cuenta>>();
        var servicio = new ServicioCuenta(mockRepo.Object);
        var dto = new RegistrarCuentaDto(
            "Nombre",
            "Apellido",
            "email-invalido",
            "password123",
            Rol.Operador,
            null,
            null);

        // Act & Assert
        Assert.ThrowsException<ExcepcionDominio>(
            () => servicio.CrearCuentaPorAdmin(dto));
    }

    [TestMethod]
    public void CrearCuentaPorAdmin_PasswordVacio_LanzaExcepcion()
    {
        // Arrange
        var mockRepo = new Mock<IRepositorio<Cuenta>>();
        var servicio = new ServicioCuenta(mockRepo.Object);
        var dto = new RegistrarCuentaDto(
            "Nombre",
            "Apellido",
            "test@test.com",
            string.Empty,
            Rol.Administrador,
            null,
            null);

        // Act & Assert
        Assert.ThrowsException<ExcepcionDominio>(
            () => servicio.CrearCuentaPorAdmin(dto));
    }

    [TestMethod]
    public void CrearCuentaPorAdmin_VisitanteSinFechaNacimiento_LanzaExcepcion()
    {
        // Arrange
        var mockRepo = new Mock<IRepositorio<Cuenta>>();
        mockRepo.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Cuenta, bool>>>()))
            .Returns((Cuenta)null!);

        var servicio = new ServicioCuenta(mockRepo.Object);
        var dto = new RegistrarCuentaDto(
            "Pedro",
            "Martínez",
            "pedro@test.com",
            "password123",
            Rol.Visitante,
            null,
            null);

        // Act & Assert
        var ex = Assert.ThrowsException<ExcepcionDominio>(
            () => servicio.CrearCuentaPorAdmin(dto));

        Assert.AreEqual("La fecha de nacimiento es requerida para visitantes.", ex.Message);
    }

    [TestMethod]
    public void CambiarNivelMembresia_VisitanteAPremium_CambiaCorrectamente()
    {
        // Arrange
        var cuenta = Cuenta.Crear("Ana", "García", new Email("ana@test.com"), "password123", Rol.Visitante);
        cuenta.AsignarVisitante(new DateTime(1990, 1, 1));

        var mockRepo = new Mock<IRepositorio<Cuenta>>();
        mockRepo.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Cuenta, bool>>>()))
            .Returns(cuenta);

        var servicio = new ServicioCuenta(mockRepo.Object);

        // Act
        servicio.CambiarNivelMembresia(cuenta.Id, NivelMembresia.Premium);

        // Assert
        Assert.AreEqual(NivelMembresia.Premium, cuenta.Visitante!.NivelMembresia);
        mockRepo.Verify(r => r.Editar(cuenta), Times.Once);
    }

    [TestMethod]
    public void CambiarNivelMembresia_CuentaSinVisitante_LanzaExcepcion()
    {
        // Arrange
        var cuenta = Cuenta.Crear("Pedro", "Admin", new Email("pedro@admin.com"), "admin123", Rol.Administrador);

        var mockRepo = new Mock<IRepositorio<Cuenta>>();
        mockRepo.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Cuenta, bool>>>()))
            .Returns(cuenta);

        var servicio = new ServicioCuenta(mockRepo.Object);

        // Act & Assert
        var ex = Assert.ThrowsException<ExcepcionDominio>(
            () => servicio.CambiarNivelMembresia(cuenta.Id, NivelMembresia.Premium));

        // Por error de tilde en el mensaje original, se usa Contains
        StringAssert.Contains(ex.Message, "perfil de visitante");
    }

    [TestMethod]
    public void CambiarNivelMembresia_CuentaNoExiste_LanzaExcepcion()
    {
        // Arrange
        var mockRepo = new Mock<IRepositorio<Cuenta>>();
        mockRepo.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Cuenta, bool>>>()))
            .Returns((Cuenta)null!);

        var servicio = new ServicioCuenta(mockRepo.Object);
        var cuentaIdInexistente = Guid.NewGuid();

        // Act & Assert
        var ex = Assert.ThrowsException<ExcepcionEntidadNoEncontrada>(
            () => servicio.CambiarNivelMembresia(cuentaIdInexistente, NivelMembresia.Premium));

        Assert.AreEqual("Cuenta no encontrada", ex.Message);
    }

    [TestMethod]
    public void CambiarNivelMembresia_VisitanteAVIP_CambiaCorrectamente()
    {
        // Arrange
        var cuenta = Cuenta.Crear("Luis", "VIP", new Email("luis@vip.com"), "password123", Rol.Visitante);
        cuenta.AsignarVisitante(new DateTime(1985, 5, 20));

        var mockRepo = new Mock<IRepositorio<Cuenta>>();
        mockRepo.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Cuenta, bool>>>()))
            .Returns(cuenta);

        var servicio = new ServicioCuenta(mockRepo.Object);

        // Act
        servicio.CambiarNivelMembresia(cuenta.Id, NivelMembresia.VIP);

        // Assert
        Assert.AreEqual(NivelMembresia.VIP, cuenta.Visitante!.NivelMembresia);
    }

    [TestMethod]
    public void CambiarNivelMembresia_PremiumAEstandar_CambiaCorrectamente()
    {
        // Arrange
        var cuenta = Cuenta.Crear("María", "Premium", new Email("maria@premium.com"), "password123", Rol.Visitante);
        cuenta.AsignarVisitante(new DateTime(1992, 3, 10));
        cuenta.Visitante!.AsignarMembresia(NivelMembresia.Premium);

        var mockRepo = new Mock<IRepositorio<Cuenta>>();
        mockRepo.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Cuenta, bool>>>()))
            .Returns(cuenta);

        var servicio = new ServicioCuenta(mockRepo.Object);

        // Act
        servicio.CambiarNivelMembresia(cuenta.Id, NivelMembresia.Estandar);

        // Assert
        Assert.AreEqual(NivelMembresia.Estandar, cuenta.Visitante.NivelMembresia);
    }

    [TestMethod]
    public void ObtenerPorId_CuentaExistente_RetornaCuentaDto()
    {
        // Arrange
        var cuenta = Cuenta.Crear("Juan", "Pérez", new Email("juan@test.com"), "password123", Rol.Visitante);
        cuenta.AsignarVisitante(new DateTime(1990, 1, 1));

        var mockRepo = new Mock<IRepositorio<Cuenta>>();
        mockRepo.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Cuenta, bool>>>()))
            .Returns(cuenta);

        var servicio = new ServicioCuenta(mockRepo.Object);

        // Act
        var resultado = servicio.ObtenerPorId(cuenta.Id);

        // Assert
        Assert.IsNotNull(resultado);
        Assert.AreEqual(cuenta.Id, resultado.Id);
        Assert.AreEqual(cuenta.Nombre, resultado.Nombre);
        Assert.AreEqual(cuenta.Apellido, resultado.Apellido);
        Assert.AreEqual(cuenta.Email.Valor, resultado.Email);
        Assert.IsNotNull(resultado.Visitante);
    }

    [TestMethod]
    public void ObtenerPorId_CuentaAdministrador_RetornaAdminSinVisitante()
    {
        // Arrange
        var cuenta = Cuenta.Crear("María", "Admin", new Email("maria@admin.com"), "admin123", Rol.Administrador);

        var mockRepo = new Mock<IRepositorio<Cuenta>>();
        mockRepo.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Cuenta, bool>>>()))
            .Returns(cuenta);

        var servicio = new ServicioCuenta(mockRepo.Object);

        // Act
        var resultado = servicio.ObtenerPorId(cuenta.Id);

        // Assert
        Assert.IsNotNull(resultado);
        Assert.AreEqual(cuenta.Id, resultado.Id);
        Assert.AreEqual("María", resultado.Nombre);
        Assert.AreEqual("Admin", resultado.Apellido);
        Assert.IsTrue(resultado.Roles.Contains(Rol.Administrador.ToString()));
        Assert.IsNull(resultado.Visitante);
    }

    [TestMethod]
    public void ObtenerPorId_CuentaNoExiste_LanzaExcepcion()
    {
        // Arrange
        var mockRepo = new Mock<IRepositorio<Cuenta>>();
        mockRepo.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Cuenta, bool>>>()))
            .Returns((Cuenta)null!);

        var servicio = new ServicioCuenta(mockRepo.Object);
        var idInexistente = Guid.NewGuid();

        // Act & Assert
        var ex = Assert.ThrowsException<ExcepcionEntidadNoEncontrada>(
            () => servicio.ObtenerPorId(idInexistente));

        Assert.AreEqual("Cuenta no encontrada", ex.Message);
    }

    [TestMethod]
    public void ObtenerPorId_VisitantePremium_RetornaConNivelCorrecto()
    {
        // Arrange
        var cuenta = Cuenta.Crear("Luis", "Premium", new Email("luis@premium.com"), "password123", Rol.Visitante);
        cuenta.AsignarVisitante(new DateTime(1985, 5, 20));
        cuenta.Visitante!.AsignarMembresia(NivelMembresia.Premium);

        var mockRepo = new Mock<IRepositorio<Cuenta>>();
        mockRepo.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Cuenta, bool>>>()))
            .Returns(cuenta);

        var servicio = new ServicioCuenta(mockRepo.Object);

        // Act
        var resultado = servicio.ObtenerPorId(cuenta.Id);

        // Assert
        Assert.IsNotNull(resultado.Visitante);
        Assert.AreEqual(NivelMembresia.Premium.ToString(), resultado.Visitante.NivelMembresia);
    }

    [TestMethod]
    public void ObtenerPorEmail_CuentaExistente_RetornaCuentaDto()
    {
        // Arrange
        var email = "ana@test.com";
        var cuenta = Cuenta.Crear("Ana", "García", new Email(email), "password123", Rol.Visitante);
        cuenta.AsignarVisitante(new DateTime(1992, 3, 15));

        var mockRepo = new Mock<IRepositorio<Cuenta>>();
        mockRepo.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Cuenta, bool>>>()))
            .Returns(cuenta);

        var servicio = new ServicioCuenta(mockRepo.Object);

        // Act
        var resultado = servicio.ObtenerPorEmail(email);

        // Assert
        Assert.IsNotNull(resultado);
        Assert.AreEqual(cuenta.Id, resultado.Id);
        Assert.AreEqual("Ana", resultado.Nombre);
        Assert.AreEqual("García", resultado.Apellido);
        Assert.AreEqual(email, resultado.Email);
    }

    [TestMethod]
    public void ObtenerPorEmail_EmailNoExiste_LanzaExcepcion()
    {
        // Arrange
        var mockRepo = new Mock<IRepositorio<Cuenta>>();
        mockRepo.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Cuenta, bool>>>()))
            .Returns((Cuenta)null!);

        var servicio = new ServicioCuenta(mockRepo.Object);
        var emailInexistente = "noexiste@test.com";

        // Act & Assert
        var ex = Assert.ThrowsException<ExcepcionEntidadNoEncontrada>(
            () => servicio.ObtenerPorEmail(emailInexistente));

        Assert.AreEqual($"Cuenta con email {emailInexistente} no encontrada.", ex.Message);
    }

    [TestMethod]
    public void ObtenerPorEmail_OperadorExistente_RetornaOperadorSinVisitante()
    {
        // Arrange
        var email = "carlos@operador.com";
        var cuenta = Cuenta.Crear("Carlos", "Operador", new Email(email), "password123", Rol.Operador);

        var mockRepo = new Mock<IRepositorio<Cuenta>>();
        mockRepo.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Cuenta, bool>>>()))
            .Returns(cuenta);

        var servicio = new ServicioCuenta(mockRepo.Object);

        // Act
        var resultado = servicio.ObtenerPorEmail(email);

        // Assert
        Assert.IsNotNull(resultado);
        Assert.AreEqual("Carlos", resultado.Nombre);
        Assert.IsTrue(resultado.Roles.Contains(Rol.Operador.ToString()));
        Assert.IsNull(resultado.Visitante);
    }

    [TestMethod]
    public void ObtenerPorEmail_EmailConMayusculas_BuscaCorrectamente()
    {
        // Arrange
        var emailOriginal = "sofia@test.com";
        var emailConMayusculas = "SOFIA@TEST.COM";
        var cuenta = Cuenta.Crear("Sofía", "Martín", new Email(emailOriginal), "password123", Rol.Visitante);
        cuenta.AsignarVisitante(new DateTime(1988, 7, 10));

        var mockRepo = new Mock<IRepositorio<Cuenta>>();
        mockRepo.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Cuenta, bool>>>()))
            .Returns(cuenta);

        var servicio = new ServicioCuenta(mockRepo.Object);

        // Act
        var resultado = servicio.ObtenerPorEmail(emailConMayusculas);

        // Assert
        Assert.IsNotNull(resultado);
        Assert.AreEqual("Sofía", resultado.Nombre);
        Assert.AreEqual(emailOriginal, resultado.Email);
    }

    [TestMethod]
    public void ObtenerPorEmail_VisitanteVIP_RetornaConNivelCorrecto()
    {
        // Arrange
        var email = "vip@test.com";
        var cuenta = Cuenta.Crear("Roberto", "VIP", new Email(email), "password123", Rol.Visitante);
        cuenta.AsignarVisitante(new DateTime(1980, 12, 25));
        cuenta.Visitante!.AsignarMembresia(NivelMembresia.VIP);

        var mockRepo = new Mock<IRepositorio<Cuenta>>();
        mockRepo.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Cuenta, bool>>>()))
            .Returns(cuenta);

        var servicio = new ServicioCuenta(mockRepo.Object);

        // Act
        var resultado = servicio.ObtenerPorEmail(email);

        // Assert
        Assert.IsNotNull(resultado.Visitante);
        Assert.AreEqual(NivelMembresia.VIP.ToString(), resultado.Visitante.NivelMembresia);
    }
}
