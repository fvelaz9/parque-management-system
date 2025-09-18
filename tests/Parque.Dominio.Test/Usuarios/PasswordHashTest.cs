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
}
