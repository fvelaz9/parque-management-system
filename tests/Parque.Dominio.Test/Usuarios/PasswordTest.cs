using Parque.Dominio.Usuarios;

namespace Parque.Dominio.Test.Usuarios;

[TestClass]
public class PasswordTest
{
    [TestMethod]
    public void TestMethod1()
    {
        var pass = new PasswordHash("prueba");
        Assert.AreEqual("prueba", pass.Valor);
    }

    [TestMethod]
    public void Constructor_Asigna_Valor_Correcto()
    {
        var password = new PasswordHash("mario@example.com");
        Assert.AreEqual("mario@example.com", password.Valor);
    }

    [TestMethod]
    public void Igualdad_Deberia_Ser_False_Para_Valor_Distinto()
    {
        var e1 = new PasswordHash("a@example.com");
        var e2 = new PasswordHash("b@example.com");
        Assert.AreNotEqual(e1, e2);
    }
}
