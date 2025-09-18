using Parque.Dominio.Usuarios;

namespace Parque.Dominio.Test.Usuarios;

[TestClass]
public class PasswordHashTest
{
    [TestMethod]
    public void Constructor_ConValorValido_DebeCrearPasswordHashCorrectamente()
    {
        // Arrange
        var valorHash = "abc123def456";

        // Act
        var passwordHash = new PasswordHash(valorHash);

        // Assert
        Assert.AreEqual(valorHash, passwordHash.Valor);
    }

    [TestMethod]
    public void Valor_DebeRetornarElValorDelConstructor()
    {
        // Arrange
        var hashEsperado = "hashedPassword123";
        var passwordHash = new PasswordHash(hashEsperado);

        // Act
        var valorActual = passwordHash.Valor;

        // Assert
        Assert.AreEqual(hashEsperado, valorActual);
    }

    [TestMethod]
    public void ToString_DebeRetornarElValorDelHash()
    {
        // Arrange
        var valorHash = "mySecureHash789";
        var passwordHash = new PasswordHash(valorHash);

        // Act
        var resultado = passwordHash.ToString();

        // Assert
        Assert.AreEqual(valorHash, resultado);
    }

    [TestMethod]
    public void Constructor_ConHashVacio_DebeCrearPasswordHashConValorVacio()
    {
        // Arrange
        var valorVacio = string.Empty;

        // Act
        var passwordHash = new PasswordHash(valorVacio);

        // Assert
        Assert.AreEqual(valorVacio, passwordHash.Valor);
    }

    [TestMethod]
    public void Constructor_ConDiferentesTiposDeHash_DebeAceptarTodosLosValores()
    {
        // Arrange & Act & Assert
        var hash1 = new PasswordHash("plaintext"); // Simulando diferentes tipos
        var hash2 = new PasswordHash("$2b$12$abcdefghijklmnopqrstuvwxyz"); // BCrypt format
        var hash3 = new PasswordHash("5e884898da28047151d0e56f8dc6292773603d0d6aabbdd62a11ef721d1542d8"); // SHA256 format

        Assert.AreEqual("plaintext", hash1.Valor);
        Assert.AreEqual("$2b$12$abcdefghijklmnopqrstuvwxyz", hash2.Valor);
        Assert.AreEqual("5e884898da28047151d0e56f8dc6292773603d0d6aabbdd62a11ef721d1542d8", hash3.Valor);
    }
}
