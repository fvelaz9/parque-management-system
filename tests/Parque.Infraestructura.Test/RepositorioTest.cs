using Parque.Dominio;
using Microsoft.EntityFrameworkCore;
using Parque.Infraestructura.Repositorios.Repositorio;
using Moq;

namespace Parque.Infraestructura.test;

[TestClass]
public class RepositorioTest
{
    private Guid _id;
        private IQueryable<Cuenta> _data;
        private Mock<DbSet<Cuenta>> _mockSet;
        private Mock<AppContexto> _appContextMock;
        private Repositorio<Cuenta> _repositorio;
        private Cuenta _cuenta;

        [TestInitialize]
        public void Initialize()
        {
            _id = Guid.NewGuid();
            _cuenta = new Cuenta
            {
                Id = _id,
                Username = "TestUserName",
                Password = "TestPassword",
                Email = "test@example.com"
            };
           
            _data = new List<Cuenta> { _cuenta }.AsQueryable();
            
            _mockSet = new Mock<DbSet<Cuenta>>();
            _mockSet.As<IQueryable<Cuenta>>().Setup(m => m.Provider).Returns(_data.Provider);
            _mockSet.As<IQueryable<Cuenta>>().Setup(m => m.Expression).Returns(_data.Expression);
            _mockSet.As<IQueryable<Cuenta>>().Setup(m => m.ElementType).Returns(_data.ElementType);
            _mockSet.As<IQueryable<Cuenta>>().Setup(m => m.GetEnumerator()).Returns(() => _data.GetEnumerator());
            
            _appContextMock = new Mock<AppContexto>();
            _repositorio = new Repositorio<Cuenta>(_appContextMock.Object);
            
            _appContextMock.Setup(x => x.Set<Cuenta>()).Returns(_mockSet.Object);
        }

        [TestMethod]
        public void AgregarTest()
        {
            // Arrange
            var nuevaCuenta = new Cuenta
            {
                Id = Guid.NewGuid(),
                Username = "NuevoUsuario",
                Password = "NuevaPassword",
                Email = "nuevo@example.com"
            };

            _mockSet.Setup(m => m.Add(It.IsAny<Cuenta>())).Verifiable();
            _appContextMock.Setup(x => x.SaveChanges()).Returns(1);

            // Act
            _repositorio.Agregar(nuevaCuenta);

            // Assert
            _mockSet.Verify(m => m.Add(It.Is<Cuenta>(c => c.Username == "NuevoUsuario")), Times.Once());
            _appContextMock.Verify(x => x.SaveChanges(), Times.Once());
        }

        [TestMethod]
        public void EncontrarTest()
        {
            // Act
            var result = _repositorio.Encontrar(c => c.Username == "TestUserName");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("TestUserName", result.Username);
            Assert.AreEqual(_id, result.Id);
        }

        [TestMethod]
        public void Encontrar_NoEncontrado_RetornaNullTest()
        {
            // Act
            var result = _repositorio.Encontrar(c => c.Username == "UsuarioInexistente");

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void EditarTest()
        {
            // Arrange
            var cuentaModificada = new Cuenta
            {
                Id = _id,
                Username = "UsuarioModificado",
                Password = "PasswordModificada",
                Email = "modificado@example.com"
            };

            _mockSet.Setup(m => m.Update(It.IsAny<Cuenta>())).Verifiable();
            _appContextMock.Setup(x => x.SaveChanges()).Returns(1);

            // Act
            _repositorio.Editar(cuentaModificada);

            // Assert
            _mockSet.Verify(m => m.Update(It.Is<Cuenta>(c => c.Username == "UsuarioModificado")), Times.Once());
            _appContextMock.Verify(x => x.SaveChanges(), Times.Once());
        }

        [TestMethod]
        public void EliminarTest()
        {
            // Arrange
            _mockSet.Setup(m => m.Remove(It.IsAny<Cuenta>())).Verifiable();
            _appContextMock.Setup(x => x.SaveChanges()).Returns(1);

            // Act
            _repositorio.Eliminar(c => c.Username == "TestUserName");

            // Assert
            _mockSet.Verify(m => m.Remove(It.Is<Cuenta>(c => c.Username == "TestUserName")), Times.Once());
            _appContextMock.Verify(x => x.SaveChanges(), Times.Once());
        }

        [TestMethod]
        public void Eliminar_EntidadNoEncontrada_NoLlamaRemoveTest()
        {
            // Arrange
            _mockSet.Setup(m => m.Remove(It.IsAny<Cuenta>())).Verifiable();

            // Act
            _repositorio.Eliminar(c => c.Username == "UsuarioInexistente");

            // Assert
            _mockSet.Verify(m => m.Remove(It.IsAny<Cuenta>()), Times.Never());
        }

        [TestMethod]
        public void ObtenerTodosTest()
        {
            // Act
            var result = _repositorio.ObtenerTodos();

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.IsTrue(result.Contains(_cuenta));
        }

        [TestMethod]
        public void ObtenerConPredicadoTest()
        {
            // Act
            var result = _repositorio.Obtener(c => c.Username == "TestUserName");

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("TestUserName", result[0].Username);
        }

        [TestMethod]
        public void ObtenerConPredicado_SinResultados_RetornaListaVaciaTest()
        {
            // Act
            var result = _repositorio.Obtener(c => c.Username == "UsuarioInexistente");

            // Assert
            Assert.AreEqual(0, result.Count);
        }
}
