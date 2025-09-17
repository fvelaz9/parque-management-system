using Parque.Dominio.Usuarios;

namespace Parque.Dominio.Test.Usuarios;

[TestClass]
public class EmailTest
{
    [TestMethod]
    public void TestMethod1()
    {
        var email = new Email("mario");
        Assert.AreEqual("mario", email.Valor);
    }
}
