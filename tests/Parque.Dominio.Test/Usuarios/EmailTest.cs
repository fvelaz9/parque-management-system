using Parque.Dominio.Excepciones;
using Parque.Dominio.Usuarios;

namespace Parque.Dominio.Test.Usuarios;

[TestClass]
public class EmailTest
{
    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void Constructor_ConEmailVacio_DeberiaLanzarExcepcion()
    {
        new Email(string.Empty);
    }

    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void Constructor_SinArroba_DeberiaLanzarExcepcion()
    {
        new Email("invalido");
    }

    [TestMethod]
    public void Constructor_DeberiaNormalizarEmail()
    {
        var email = new Email("  TEST@EXAMPLE.COM  ");
        Assert.AreEqual("test@example.com", email.Valor);
    }
}
