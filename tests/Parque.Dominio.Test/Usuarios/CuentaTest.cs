using Parque.Dominio.Usuarios;

namespace Parque.Dominio.Test.Usuarios;

[TestClass]
public class CuentaTest
{
    [TestMethod]
    public void Crear_CuentaValida_AsignaDatosCorrectos()
    {
        var email = new Email("prueba@mail.com");
        var hash = new PasswordHash("abc123");
        var roles = new List<Rol> { Rol.Visitante };
        var cuenta = Cuenta.Crear("Mario", "Rossi", email, hash, roles);
        Assert.AreEqual("Mario", cuenta.Nombre);
        Assert.AreEqual(email, cuenta.Email);
        Assert.IsTrue(cuenta.Roles.Contains(Rol.Visitante));
    }
}
