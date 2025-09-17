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
    public List<int> Atracciones { get; set; } = new List<int>();
    public EstadoEvento Estado {get; set;}
    
    

}
