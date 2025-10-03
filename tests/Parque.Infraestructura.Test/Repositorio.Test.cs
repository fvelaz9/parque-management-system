using Microsoft.EntityFrameworkCore;
using Moq;
using Parque.Dominio.Usuarios;
using Parque.Infraestructura.Repositorios;

namespace Parque.Infraestructura.test;

[TestClass]
public class RepositorioTest
{
    private IQueryable<Cuenta>? _data;
    private Mock<DbSet<Cuenta>>? _mockSet;
    private Mock<AppContexto>? _appContextMock;
    private Repositorio<Cuenta>? _repositorio;
    private Cuenta? _cuenta;

    [TestInitialize]
    public void Initialize()
    {
        var nombre = "TestUserName";
        var apellido = "Pérez";
        var email = new Email("juan@test.com");
        var password = "password123";
        _cuenta = Cuenta.Crear(nombre, apellido, email, password);

        _data = new List<Cuenta> { _cuenta }.AsQueryable();

        _mockSet = new Mock<DbSet<Cuenta>>();
        _mockSet.As<IQueryable<Cuenta>>().Setup(m => m.Provider).Returns(_data.Provider);
        _mockSet.As<IQueryable<Cuenta>>().Setup(m => m.Expression).Returns(_data.Expression);
        _mockSet.As<IQueryable<Cuenta>>().Setup(m => m.ElementType).Returns(_data.ElementType);
        _mockSet.As<IQueryable<Cuenta>>().Setup(m => m.GetEnumerator()).Returns(() => _data.GetEnumerator());

        var options = new DbContextOptionsBuilder<AppContexto>().Options;
        _appContextMock = new Mock<AppContexto>(options);
        _repositorio = new Repositorio<Cuenta>(_appContextMock.Object);

        _appContextMock.Setup(x => x.Set<Cuenta>()).Returns(_mockSet.Object);
    }

    [TestMethod]
    public void AgregarTest()
    {
        // Arrange
        var nombre = "Juan";
        var apellido = "Pérez";
        var email = new Email("juan@test.com");
        var password = "password123";
        var nuevaCuenta = Cuenta.Crear(nombre, apellido, email, password);

        // Mock the DbContext.Add method instead of DbSet.Add
        _appContextMock!.Setup(x => x.Add(It.IsAny<Cuenta>())).Verifiable();
        _appContextMock!.Setup(x => x.SaveChanges()).Returns(1);

        // Act
        _repositorio!.Agregar(nuevaCuenta);

        // Assert
        _appContextMock.Verify(x => x.Add(It.Is<Cuenta>(c => c.Nombre == "Juan")), Times.Once());
        _appContextMock.Verify(x => x.SaveChanges(), Times.Once());
    }

    [TestMethod]
    public void EncontrarTest()
    {
        // Act
        var result = _repositorio!.Encontrar(c => c.Nombre == "TestUserName");

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("TestUserName", result.Nombre);
        Assert.AreEqual(_cuenta!.Id, result.Id);
    }

    [TestMethod]
    public void Encontrar_NoEncontrado_RetornaNullTest()
    {
        // Act
        var result = _repositorio!.Encontrar(c => c.Nombre == "UsuarioInexistente");

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public void EditarTest()
    {
        // Arrange
        var nombre = "UsuarioModificado";
        var apellido = "Pérez";
        var email = new Email("juan@test.com");
        var password = "password123";
        var cuentaModificada = Cuenta.Crear(nombre, apellido, email, password);

        _mockSet!.Setup(m => m.Update(It.IsAny<Cuenta>())).Verifiable();
        _appContextMock!.Setup(x => x.SaveChanges()).Returns(1);

        // Act
        _repositorio!.Editar(cuentaModificada);

        // Assert
        _mockSet.Verify(m => m.Update(It.Is<Cuenta>(c => c.Nombre == "UsuarioModificado")), Times.Once());
        _appContextMock.Verify(x => x.SaveChanges(), Times.Once());
    }

    [TestMethod]
    public void EliminarTest()
    {
        // Arrange
        _mockSet!.Setup(m => m.Remove(It.IsAny<Cuenta>())).Verifiable();
        _appContextMock!.Setup(x => x.SaveChanges()).Returns(1);

        // Act
        _repositorio!.Eliminar(c => c.Nombre == "TestUserName");

        // Assert
        _mockSet.Verify(m => m.Remove(It.Is<Cuenta>(c => c.Nombre == "TestUserName")), Times.Once());
        _appContextMock.Verify(x => x.SaveChanges(), Times.Once());
    }

    [TestMethod]
    public void Eliminar_EntidadNoEncontrada_NoLlamaRemoveTest()
    {
        // Arrange
        _mockSet!.Setup(m => m.Remove(It.IsAny<Cuenta>())).Verifiable();

        // Act
        _repositorio!.Eliminar(c => c.Nombre == "UsuarioInexistente");

        // Assert
        _mockSet.Verify(m => m.Remove(It.IsAny<Cuenta>()), Times.Never());
    }

    [TestMethod]
    public void ObtenerTodosTest()
    {
        // Act
        var result = _repositorio!.ObtenerTodos();

        // Assert
        Assert.AreEqual(1, result.Count);
        Assert.IsNotNull(_cuenta);
        Assert.IsTrue(result.Contains(_cuenta!));
    }

    [TestMethod]
    public void ObtenerConPredicadoTest()
    {
        // Act
        var result = _repositorio!.Obtener(c => c.Nombre == "TestUserName");

        // Assert
        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("TestUserName", result[0].Nombre);
    }

    [TestMethod]
    public void ObtenerConPredicado_SinResultados_RetornaListaVaciaTest()
    {
        // Act
        var result = _repositorio!.Obtener(c => c.Nombre == "UsuarioInexistente");

        // Assert
        Assert.AreEqual(0, result.Count);
    }
}
