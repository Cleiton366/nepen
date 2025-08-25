namespace Desafio_NEPEN.Com.Nepen.Api.Dtos.Leitura;
using System.ComponentModel.DataAnnotations;

public class LeituraCreateDto
{
    [Required]
    [DataType(DataType.DateTime)]
    public DateTime Timestamp { get; set; }

    [Range(0, 1000)]
    public decimal Tensao { get; set; }

    [Range(0, 1000)]
    public decimal Corrente { get; set; }

    [Range(0, 100_000)]
    public decimal PotenciaAtiva { get; set; }

    [Range(0, 50_000)]
    public decimal PotenciaReativa { get; set; }

    [Range(0, 999_999_999.99)]
    public decimal EnergiaAtivaDireta { get; set; }

    [Range(0, 999_999_999.99)]
    public decimal EnergiaAtivaReversa { get; set; }

    [Range(0, 1)]
    public decimal FatorPotencia { get; set; }

    [Range(58, 62)]
    public decimal Frequencia { get; set; }
}