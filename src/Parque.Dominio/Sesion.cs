namespace Parque.Dominio;

public class Sesion
{
    public Guid Id { get; set; }
    public Guid UsuarioId { get; set; }
    public required string Token { get; set; }
}
