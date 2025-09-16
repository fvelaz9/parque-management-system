namespace Parque.Dominio.Usuarios;
public class PasswordHash(string valor)
{
    public string Valor { get; } = valor;
    public override string ToString() => Valor;
}
