namespace Desafio_NEPEN.Com.Nepen.Api.Dtos.Leitura;

public class LeituraDto
{
    public long Id { get; set; }
    public DateTime Timestamp { get; set; }
    public decimal Tensao { get; set; }
    public decimal Corrente { get; set; }
    public decimal PotenciaAtiva { get; set; }
    public decimal PotenciaReativa { get; set; }
    public decimal EnergiaAtivaDireta { get; set; }
    public decimal EnergiaAtivaReversa { get; set; }
    public decimal FatorPotencia { get; set; }
    public decimal Frequencia { get; set; }
    public DateTime DataCriacao { get; set; }
}