namespace DefaultNamespace;

public class Repositorio<T>(AppContexto contexto) : IRepositorio<T> where T : class
{
    private readonly AppContexto _contexto = contexto;

    public void Agregar(T entidad)
    {
        _contexto.Add(entidad);
        _contexto.SaveChanges();
    }
    public T? Encontrar(Expression<Func<T, bool>> predicado)
    {
        return _dbSet.FirstOrDefault(predicado);
    }
    
}
