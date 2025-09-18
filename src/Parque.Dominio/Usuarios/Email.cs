using Parque.Dominio.Excepciones;

namespace Parque.Dominio.Usuarios;
public class Email(string valor)
{
    public string Valor { get; } = ValidarYNormalizar(valor);
    private static string ValidarYNormalizar(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
        {
            throw new ExcepcionDominio("El email no puede estar vacío.");
        }

        return valor;
    }
}
