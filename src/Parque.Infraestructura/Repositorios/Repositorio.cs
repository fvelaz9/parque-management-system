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
    public void Editar(T entidad)
    {
        _contexto.Set<T>().Update(entidad);
        _contexto.SaveChanges();
    }
    public void Eliminar(Expression<Func<T, bool>> predicado)
    {
        T? entidad = Encontrar(predicado);
        if(entidad != null)
        {
            _contexto.Set<T>.().Remove(entidad);
            _contexto.SaveChanges();
        }
    }

    public List<T> ObtenerTodos()
    {
        return _contexto.Set<T>().ToList();
    }
    public List<T> Obtener(Expression<Func<T, bool>> predicado)
    {
        return _contexto.Set<T>().Where(predicado).ToList();
    }
    
}
