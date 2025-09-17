using Parque.Dominio.Usuarios;

namespace Parque.Dominio.Test.Usuarios;

[TestClass]
public class EmailTest
{
    [TestMethod]
    public void Constructor_Asigna_Valor_Correcto()
    {
        var email = new Email("mario@example.com");
        Assert.AreEqual("mario@example.com", email.Valor);
    }
}
