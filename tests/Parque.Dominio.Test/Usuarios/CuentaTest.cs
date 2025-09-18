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
        var password = new PasswordHash("hash123d12d12d12d12d1d1d31d");

        // Act
        var cuenta = Cuenta.Crear(nombre, apellido, email, password);

        // Assert
        Assert.AreNotEqual(Guid.Empty, cuenta.Id);
        Assert.AreEqual(nombre, cuenta.Nombre);
        Assert.AreEqual(apellido, cuenta.Apellido);
        Assert.AreEqual(email, cuenta.Email);
        Assert.AreEqual(password, cuenta.PasswordHash);
    }

    [TestMethod]
    public void Crear_ConDatosValidos_DebeAsignarRolVisitantePorDefecto()
    {
        // Arrange
        var nombre = "Ana";
        var apellido = "García";
        var email = new Email("ana@test.com");
        var password = new PasswordHash("hash4561d21d121d12d1d11ded111");

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
        Cuenta.Crear(string.Empty, "Pérez", new Email("test@mail.com"), new PasswordHash("hash"));
    }

    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void Crear_ConApellidoVacio_DeberiaLanzarExcepcion()
    {
        Cuenta.Crear("Juan", string.Empty, new Email("test@mail.com"), new PasswordHash("hash"));
    }

    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void Crear_ConEmailNulo_DeberiaLanzarExcepcion()
    {
        Email email = null!;
        Cuenta.Crear("Juan", "Pérez", email, new PasswordHash("hash"));
    }

    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void Crear_ConPasswordNulo_DeberiaLanzarExcepcion()
    {
        PasswordHash pass = null!;
        Cuenta.Crear("Juan", "Pérez", new Email("test@mail.com"), pass);
    }

    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void Crear_ConNombreSoloEspacios_DeberiaLanzarExcepcion()
    {
        Cuenta.Crear("   ", "Pérez", new Email("test@mail.com"), new PasswordHash("hash"));
    }

    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void Crear_ConApellidoSoloEspacios_DeberiaLanzarExcepcion()
    {
        Cuenta.Crear("Juan", "   ", new Email("test@mail.com"), new PasswordHash("hash"));
    }

    [TestMethod]
    public void Crear_ConNombreVacio_DeberiaLanzarExcepcionConMensajeCorrecto()
    {
        var ex = Assert.ThrowsException<ExcepcionDominio>(() =>
            Cuenta.Crear(string.Empty, "Pérez", new Email("test@mail.com"), new PasswordHash("hashd1212d12d12d12d1d131d1d1d")));

        Assert.AreEqual("Nombre es requerido", ex.Message);
    }

    [TestMethod]
    public void Crear_ConApellidoVacio_DeberiaLanzarExcepcionConMensajeCorrecto()
    {
        var ex = Assert.ThrowsException<ExcepcionDominio>(() =>
            Cuenta.Crear("Juan", string.Empty, new Email("test@mail.com"), new PasswordHash("hashd12d12d12d1d21d12d1d12d12d")));

        Assert.AreEqual("Apellido es requerido", ex.Message);
    }

    [TestMethod]
    public void Crear_ConEmailNulo_DeberiaLanzarExcepcionConMensajeCorrecto()
    {
        Email email = null!;
        var ex = Assert.ThrowsException<ExcepcionDominio>(() =>
            Cuenta.Crear("Juan", "Perez", email, new PasswordHash("hasd12d12d12d12d12d12d1d12d12dh")));

        Assert.AreEqual("Email es requerido", ex.Message);
    }

    [TestMethod]
    public void Crear_ConPassNulo_DeberiaLanzarExcepcionConMensajeCorrecto()
    {
        PasswordHash pass = null!;
        var ex = Assert.ThrowsException<ExcepcionDominio>(() =>
            Cuenta.Crear("Juan", "Perez", new Email("test@mail.com"), pass));

        Assert.AreEqual("Password es requerido", ex.Message);
    }
}
