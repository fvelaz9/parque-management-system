namespace Parque.Aplicacion.Test.Servicios;

[TestClass]
public class ServicioAutenticacionTest
{
    [TestMethod]
    public void TestMethod1()
    {
    }

    [TestMethod]
    public void ValidarToken_TokenValido_DeberiaRetornarTrue()
    {
    }

    [TestMethod]
    public void ValidarToken_TokenExpirado_DeberiaRetornarFalse()
    {
    }

    [TestMethod]
    public void ValidarToken_TokenInvalido_DeberiaLanzarExcepcion()
    {
    }

    [TestMethod]
    public void GenerarToken_CuentaValida_DeberiaIncluirClaims()
    {
    }

    [TestMethod]
    public void ObtenerClaimsDeToken_TokenValido_DeberiaExtraerRol()
    {
    }
}
