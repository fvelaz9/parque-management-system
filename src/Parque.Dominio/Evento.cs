namespace DefaultNamespace;

public class Evento
{
    public int Id {get; set;}
    public string Titulo {get; set;}
    public string Descripcion {get; set;}
    public DateTime Inicio {get; set;}
    public DateTime Fin {get; set;}
    public int Aforo_Maximo {get; set;}
    public float CostoAdicional  {get; set;}
    public List<Atraccion> Atracciones { get; set; } = new List<Atraccion>();
    public EstadoEvento Estado {get; set;}

    public Evento(string titulo, string descripcion,  DateTime inicio,  DateTime fin, int aforo_Maximo, float costoAdicional, EstadoEvento estado)
    {
       Titulo = titulo;
       Descripcion = descripcion;
       Inicio = inicio;
       Fin = fin;
       Aforo_Maximo = aforo_Maximo;
       CostoAdicional = costoAdicional;
       Atracciones = new List<Atraccion>();
       Estado = estado;
    }
}
