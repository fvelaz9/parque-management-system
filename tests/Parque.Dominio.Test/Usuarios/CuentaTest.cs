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
        var password = new PasswordHash("hash123");

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
        var password = new PasswordHash("hash456");

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
}
