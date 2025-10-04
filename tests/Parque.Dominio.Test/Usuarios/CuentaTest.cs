using Parque.Dominio.Excepciones;
using Parque.Dominio.Usuarios;

namespace Parque.Dominio.Test.Usuarios;

[TestClass]
public class CuentaTest
{
    [TestMethod]
    public void Crear_ConDatosValidos_DebeCrearCuentaConPropiedadesCorrectas()
    {
        // Arrange
        var nombre = "Juan";
        var apellido = "Pérez";
        var email = new Email("juan@test.com");
        var password = "password123";

        // Act
        var cuenta = Cuenta.Crear(nombre, apellido, email, password);

        // Assert
        Assert.AreNotEqual(Guid.Empty, cuenta.Id);
        Assert.AreEqual(nombre, cuenta.Nombre);
        Assert.AreEqual(apellido, cuenta.Apellido);
        Assert.AreEqual(email, cuenta.Email);
        Assert.AreEqual(password, cuenta.Password);
    }

    [TestMethod]
    public void Crear_ConDatosValidos_DebeAsignarRolVisitantePorDefecto()
    {
        // Arrange
        var nombre = "Ana";
        var apellido = "García";
        var email = new Email("ana@test.com");
        var password = "password123";

        // Act
        var cuenta = Cuenta.Crear(nombre, apellido, email, password);

        // Assert
        Assert.IsTrue(cuenta.Roles.Contains(Rol.Visitante));
        Assert.AreEqual(1, cuenta.Roles.Count);
    }

    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void Crear_ConNombreVacio_DeberiaLanzarExcepcion()
    {
        // Arrange & Act & Assert
        Cuenta.Crear(string.Empty, "Pérez", new Email("test@mail.com"), "password123");
    }

    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void Crear_ConApellidoVacio_DeberiaLanzarExcepcion()
    {
        Cuenta.Crear("Juan", string.Empty, new Email("test@mail.com"), "password123");
    }

    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void Crear_ConEmailNulo_DeberiaLanzarExcepcion()
    {
        Email email = null!;
        Cuenta.Crear("Juan", "Pérez", email, "password123");
    }

    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void Crear_ConPasswordNulo_DeberiaLanzarExcepcion()
    {
        Cuenta.Crear("Juan", "Pérez", new Email("test@mail.com"), null!);
    }

    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void Crear_ConNombreSoloEspacios_DeberiaLanzarExcepcion()
    {
        Cuenta.Crear("   ", "Pérez", new Email("test@mail.com"), "password123");
    }

    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void Crear_ConApellidoSoloEspacios_DeberiaLanzarExcepcion()
    {
        Cuenta.Crear("Juan", "   ", new Email("test@mail.com"), "password123");
    }

    [TestMethod]
    public void Crear_ConNombreVacio_DeberiaLanzarExcepcionConMensajeCorrecto()
    {
        var ex = Assert.ThrowsException<ExcepcionDominio>(() =>
            Cuenta.Crear(string.Empty, "Pérez", new Email("test@mail.com"), "password123"));

        Assert.AreEqual("Nombre es requerido", ex.Message);
    }

    [TestMethod]
    public void Crear_ConApellidoVacio_DeberiaLanzarExcepcionConMensajeCorrecto()
    {
        var ex = Assert.ThrowsException<ExcepcionDominio>(() =>
            Cuenta.Crear("Juan", string.Empty, new Email("test@mail.com"), "password123"));

        Assert.AreEqual("Apellido es requerido", ex.Message);
    }

    [TestMethod]
    public void Crear_ConEmailNulo_DeberiaLanzarExcepcionConMensajeCorrecto()
    {
        Email email = null!;
        var ex = Assert.ThrowsException<ExcepcionDominio>(() =>
            Cuenta.Crear("Juan", "Perez", email, "password123"));

        Assert.AreEqual("Email es requerido", ex.Message);
    }

    [TestMethod]
    public void Crear_ConPassNulo_DeberiaLanzarExcepcionConMensajeCorrecto()
    {
        var ex = Assert.ThrowsException<ExcepcionDominio>(() =>
            Cuenta.Crear("Juan", "Perez", new Email("test@mail.com"), null!));

        Assert.AreEqual("Password es requerido", ex.Message);
    }

    [TestMethod]
    public void ActualizarNombre_NombreVacio_LanzaExcepcionDominio()
    {
        // Arrange
        var cuenta = Cuenta.Crear(
            "Juan",
            "Pérez",
            new Email("juan@test.com"),
            "password123");

        // Act & Assert
        var ex = Assert.ThrowsException<ExcepcionDominio>(() => cuenta.ActualizarNombre(string.Empty));
        Assert.AreEqual("Nombre es requerido", ex.Message);
    }

    [TestMethod]
    public void ActualizarApellido_ApellidoVacio_LanzaExcepcionDominio()
    {
        // Arrange
        var cuenta = Cuenta.Crear(
            "Juan",
            "Pérez",
            new Email("juan@test.com"),
            "password123");

        // Act & Assert
        var ex = Assert.ThrowsException<ExcepcionDominio>(() => cuenta.ActualizarApellido(string.Empty));
        Assert.AreEqual("Apellido es requerido", ex.Message);
    }

    [TestMethod]
    public void ActualizarEmail_EmailNulo_LanzaExcepcionDominio()
    {
        // Arrange
        var cuenta = Cuenta.Crear(
            "Juan",
            "Pérez",
            new Email("juan@test.com"),
            "password123");

        // Act & Assert
        var ex = Assert.ThrowsException<ExcepcionDominio>(() => cuenta.ActualizarEmail(null!));
        Assert.AreEqual("Email es requerido", ex.Message);
    }

    [TestMethod]
    public void ActualizarPass_PassVacia_LanzaExcepcionDominio()
    {
        // Arrange
        var cuenta = Cuenta.Crear(
            "Juan",
            "Pérez",
            new Email("juan@test.com"),
            "password123");

        // Act & Assert
        var ex = Assert.ThrowsException<ExcepcionDominio>(() => cuenta.ActualizarPassword(string.Empty));
        Assert.AreEqual("Password es requerido", ex.Message);
    }

    [TestMethod]
    public void AsignarRol_RolAdministrador_AsignaCorrectamente()
    {
        // Arrange
        var cuenta = Cuenta.Crear("Juan", "Pérez", new Email("juan@test.com"), "password123");

        // Act
        cuenta.AgregarRol(Rol.Administrador);

        // Assert
        Assert.IsTrue(cuenta.Roles.Contains(Rol.Visitante));
        Assert.AreEqual(2, cuenta.Roles.Count);
    }

    [TestMethod]
    public void AgregarRol_VariosRoles_AgregaTodos()
    {
        // Arrange
        var cuenta = Cuenta.Crear("Juan", "Pérez", new Email("juan@test.com"), "password123");

        // Act
        cuenta.AgregarRol(Rol.Administrador);
        cuenta.AgregarRol(Rol.Operador);

        // Assert
        Assert.IsTrue(cuenta.Roles.Contains(Rol.Visitante));
        Assert.IsTrue(cuenta.Roles.Contains(Rol.Administrador));
        Assert.IsTrue(cuenta.Roles.Contains(Rol.Operador));
        Assert.AreEqual(3, cuenta.Roles.Count);
    }

    [TestMethod]
    public void AgregarRol_RolDuplicado_LanzaExcepcion()
    {
        // Arrange
        var cuenta = Cuenta.Crear("Juan", "Pérez", new Email("juan@test.com"), "password123");
        cuenta.AgregarRol(Rol.Administrador);

        // Act & Assert
        var ex = Assert.ThrowsException<ExcepcionDominio>(
            () => cuenta.AgregarRol(Rol.Administrador));

        Assert.AreEqual("La cuenta ya tiene el rol Administrador", ex.Message);
    }

    [TestMethod]
    public void AgregarRol_VisitanteDuplicado_LanzaExcepcion()
    {
        // Arrange
        var cuenta = Cuenta.Crear("Juan", "Pérez", new Email("juan@test.com"), "password123");

        // Act & Assert
        var ex = Assert.ThrowsException<ExcepcionDominio>(
            () => cuenta.AgregarRol(Rol.Visitante));

        Assert.AreEqual("La cuenta ya tiene el rol Visitante", ex.Message);
    }

    [TestMethod]
    public void AgregarRol_RolOperador_MantieneLosRolesExistentes()
    {
        // Arrange
        var cuenta = Cuenta.Crear("Juan", "Pérez", new Email("juan@test.com"), "password123");
        cuenta.AgregarRol(Rol.Administrador);

        // Act
        cuenta.AgregarRol(Rol.Operador);

        // Assert
        Assert.IsTrue(cuenta.Roles.Contains(Rol.Visitante));
        Assert.IsTrue(cuenta.Roles.Contains(Rol.Administrador));
        Assert.IsTrue(cuenta.Roles.Contains(Rol.Operador));
        Assert.AreEqual(3, cuenta.Roles.Count);
    }

    [TestMethod]
    public void QuitarRol_RolExistente_QuitaCorrectamente()
    {
        // Arrange
        var cuenta = Cuenta.Crear("Juan", "Pérez", new Email("juan@test.com"), "password123");
        cuenta.AgregarRol(Rol.Administrador);
        cuenta.AgregarRol(Rol.Operador);

        // Act
        cuenta.QuitarRol(Rol.Administrador);

        // Assert
        Assert.IsFalse(cuenta.Roles.Contains(Rol.Administrador));
        Assert.IsTrue(cuenta.Roles.Contains(Rol.Visitante));
        Assert.IsTrue(cuenta.Roles.Contains(Rol.Operador));
        Assert.AreEqual(2, cuenta.Roles.Count);
    }

    [TestMethod]
    public void QuitarRol_RolNoExistente_LanzaExcepcion()
    {
        // Arrange
        var cuenta = Cuenta.Crear("Juan", "Pérez", new Email("juan@test.com"), "password123");

        // Act & Assert
        var ex = Assert.ThrowsException<ExcepcionDominio>(
            () => cuenta.QuitarRol(Rol.Administrador));

        Assert.AreEqual("La cuenta no tiene el rol Administrador", ex.Message);
    }

    [TestMethod]
    public void QuitarRol_UltimoRol_LanzaExcepcion()
    {
        // Arrange
        var cuenta = Cuenta.Crear("Juan", "Pérez", new Email("juan@test.com"), "password123");

        // Act & Assert
        var ex = Assert.ThrowsException<ExcepcionDominio>(
            () => cuenta.QuitarRol(Rol.Visitante));

        Assert.AreEqual("La cuenta debe tener al menos un rol", ex.Message);
    }

    [TestMethod]
    public void QuitarRol_DejaUnRol_NoLanzaExcepcion()
    {
        // Arrange
        var cuenta = Cuenta.Crear("Juan", "Pérez", new Email("juan@test.com"), "password123");
        cuenta.AgregarRol(Rol.Administrador);

        // Act
        cuenta.QuitarRol(Rol.Administrador);

        // Assert
        Assert.IsTrue(cuenta.Roles.Contains(Rol.Visitante));
        Assert.IsFalse(cuenta.Roles.Contains(Rol.Administrador));
        Assert.AreEqual(1, cuenta.Roles.Count);
    }

    [TestMethod]
    public void QuitarRol_QuitaVariosRoles_MantieneLosRestantes()
    {
        // Arrange
        var cuenta = Cuenta.Crear("Juan", "Pérez", new Email("juan@test.com"), "password123");
        cuenta.AgregarRol(Rol.Administrador);
        cuenta.AgregarRol(Rol.Operador);

        // Act
        cuenta.QuitarRol(Rol.Administrador);
        cuenta.QuitarRol(Rol.Operador);

        // Assert
        Assert.IsTrue(cuenta.Roles.Contains(Rol.Visitante));
        Assert.IsFalse(cuenta.Roles.Contains(Rol.Administrador));
        Assert.IsFalse(cuenta.Roles.Contains(Rol.Operador));
        Assert.AreEqual(1, cuenta.Roles.Count);
    }

    [TestMethod]
    public void QuitarRol_VisitanteConVariosRoles_QuitaCorrectamente()
    {
        // Arrange
        var cuenta = Cuenta.Crear("Juan", "Pérez", new Email("juan@test.com"), "password123");
        cuenta.AgregarRol(Rol.Administrador);

        // Act
        cuenta.QuitarRol(Rol.Visitante);

        // Assert
        Assert.IsFalse(cuenta.Roles.Contains(Rol.Visitante));
        Assert.IsTrue(cuenta.Roles.Contains(Rol.Administrador));
        Assert.AreEqual(1, cuenta.Roles.Count);
    }

    [TestMethod]
    public void QuitarRol_OperadorNoExistente_LanzaExcepcion()
    {
        // Arrange
        var cuenta = Cuenta.Crear("Juan", "Pérez", new Email("juan@test.com"), "password123");
        cuenta.AgregarRol(Rol.Administrador);

        // Act & Assert
        var ex = Assert.ThrowsException<ExcepcionDominio>(
            () => cuenta.QuitarRol(Rol.Operador));

        Assert.AreEqual("La cuenta no tiene el rol Operador", ex.Message);
    }
}
