using Parque.Dominio.Excepciones;

namespace Parque.Dominio.Usuarios;
public class PasswordHash(string valor)
{
    public string Valor { get; } = ValidarHash(valor);
    private static string ValidarHash(string hash)
    {
        if (string.IsNullOrWhiteSpace(hash))
        {
            throw new ExcepcionDominio("El hash de la contraseña no puede estar vacío.");
        }

        return hash;
    }

    public override string ToString() => Valor;
}
