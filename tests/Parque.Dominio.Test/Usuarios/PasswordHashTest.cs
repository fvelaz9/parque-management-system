using Parque.Dominio.Excepciones;
using Parque.Dominio.Usuarios;

namespace Parque.Dominio.Test.Usuarios;

[TestClass]
public class PasswordHashTest
{
    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void Constructor_ConHashVacio_DeberiaLanzarExcepcion()
    {
        new PasswordHash(string.Empty);
    }

    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void Constructor_ConHashMuyCorto_DeberiaLanzarExcepcion()
    {
        new PasswordHash("abc123");
    }

    [TestMethod]
    public void Constructor_ConHashValido_DeberiaCrearCorrectamente()
    {
        var hashValido = "$2a$10$N9qo8uLOickgx2ZMRZoMye1234567890ABCDEFGHIJ";
        var password = new PasswordHash(hashValido);
        Assert.AreEqual(hashValido, password.Valor);
    }
}
