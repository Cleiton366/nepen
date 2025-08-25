namespace Desafio_NEPEN.Com.Nepen.Core.Entities;

public class Medidor
{
    public string MedidorId { get; set; } = null!;
    
    public ICollection<Leitura> Leituras { get; set; } = new List<Leitura>();
}