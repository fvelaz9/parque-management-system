namespace Parque.Dominio.Usuarios;
public class Email(string valor)
{
    public string Valor { get; } = valor;
    public override string ToString() => Valor;
}
