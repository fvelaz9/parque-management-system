using System.Linq.Expressions;

namespace Parque.Infraestructura.Repositorios;

public interface IRepositorio<T>
{
    void Agregar(T entity);
    T? Encontrar(Expression<Func<T, bool>> predicate);
    void Editar(T entity);
    void Eliminar(Expression<Func<T, bool>> predicate);
    List<T> ObtenerTodos();
    List<T> Obtener(Expression<Func<T, bool>> predicate);
}
