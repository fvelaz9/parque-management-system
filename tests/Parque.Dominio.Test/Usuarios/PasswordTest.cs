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
}
