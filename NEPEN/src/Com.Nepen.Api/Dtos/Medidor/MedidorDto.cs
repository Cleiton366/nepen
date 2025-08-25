using Desafio_NEPEN.Com.Nepen.Api.Dtos.Leitura;

namespace Desafio_NEPEN.Com.Nepen.Api.Dtos.Medidor;

public class MedidorDto
{
    public string MedidorId { get; set; } = null!;
    public PeriodoDto Periodo { get; set; } = null!;
    public int TotalRegistros { get; set; }
    public List<LeituraDto> Leituras { get; set; } = new List<LeituraDto>();
}