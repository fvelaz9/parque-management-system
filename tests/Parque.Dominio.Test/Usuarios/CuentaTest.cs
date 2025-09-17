using Parque.Dominio.Usuarios;

namespace Parque.Dominio.Test.Usuarios;

[TestClass]
public class CuentaTest
{
    [TestMethod]
    public void TestMethod1()
    {
        var email = new Email("mailprueba");
        var password = new PasswordHash("passprueba");
        var cuenta = Cuenta.Crear("jorge", "ramirez", email, password);
        Assert.AreEqual("jorge", cuenta.Nombre);
        Assert.AreEqual("ramirez", cuenta.Apellido);
        Assert.AreEqual(cuenta.Id, cuenta.Id);
        Assert.AreEqual("mailprueba", cuenta.Email.Valor);
        Assert.AreEqual("passprueba", cuenta.PasswordHash.Valor);
    }
}
