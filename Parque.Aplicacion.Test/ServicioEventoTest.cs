using Moq;
using Parque.Aplicacion;
using Parque.Dominio;
using Parque.Infraestructura.Repositorios;

namespace Parque.Aplicacion.Test;

[TestClass]
public class ServicioEventoTest
{
    private Mock<IRepositorio<Evento>> _repositorioMock;
}
