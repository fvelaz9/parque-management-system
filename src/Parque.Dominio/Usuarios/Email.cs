using Parque.Dominio.Excepciones;

namespace Parque.Dominio.Usuarios;
public class Email(string valor)
{
    public string Valor { get; } = ValidarYNormalizar(valor);
    private static string ValidarYNormalizar(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ExcepcionDominio("El email no puede estar vacío.");
        }

        var normalizado = email.Trim().ToLowerInvariant();

        if(!normalizado.Contains("@") || normalizado.IndexOf("@") != normalizado.LastIndexOf("@"))
        {
            throw new ExcepcionDominio("Email debe tener formato válido");
        }

        var partes = normalizado.Split('@');
        if(partes[0].Length == 0 || partes[1].Length == 0 || !partes[1].Contains("."))
        {
            throw new ExcepcionDominio("Email debe tener formato válido");
        }

        return normalizado;
    }

    public override string ToString() => Valor;
}
