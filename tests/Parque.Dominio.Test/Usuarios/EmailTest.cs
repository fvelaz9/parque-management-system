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
    public void Constructor_ConValorValido_DebeCrearEmailCorrectamente()
    {
        // Arrange
        var valorEmail = "test@example.com";

        // Act
        var email = new Email(valorEmail);

        // Assert
        Assert.AreEqual(valorEmail, email.Valor);
    }

    [TestMethod]
    public void Valor_DebeRetornarElValorDelConstructor()
    {
        // Arrange
        var valorEsperado = "usuario@dominio.com";
        var email = new Email(valorEsperado);

        // Act
        var valorActual = email.Valor;

        // Assert
        Assert.AreEqual(valorEsperado, valorActual);
    }

    [TestMethod]
    public void Constructor_ConDiferentesFormatos_DebeAceptarTodosLosValores()
    {
        // Arrange & Act & Assert
        var email1 = new Email("simple@test.com");
        var email2 = new Email("with.dots@example.org");
        var email3 = new Email("with+plus@domain.net");

        Assert.AreEqual("simple@test.com", email1.Valor);
        Assert.AreEqual("with.dots@example.org", email2.Valor);
        Assert.AreEqual("with+plus@domain.net", email3.Valor);
    }
}
