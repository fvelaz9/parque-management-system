using Parque.Dominio.Usuarios;

namespace Parque.Dominio.Test;

[TestClass]
public sealed class Test1
{
    [TestMethod]
    public void TestMethod1()
    {
        Email email = new("mario");
        Assert.AreEqual("mario", email.Valor);
    }
}
