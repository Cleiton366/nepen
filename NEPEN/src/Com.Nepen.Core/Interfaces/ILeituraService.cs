using Desafio_NEPEN.Com.Nepen.Api.Dtos.Leitura;
using Desafio_NEPEN.Com.Nepen.Api.Dtos.Medidor;
using Desafio_NEPEN.Com.Nepen.Core.Entities;

namespace Desafio_NEPEN.Com.Nepen.Core.Interfaces;

public interface ILeituraService
{
    Task<MedidorDto> ObterLeiturasAsync(string medidorId, DateTime dataInicio, DateTime dataFim, int limite);
    Task<LeituraDto> CriarLeituraAsync(string medidorId, LeituraCreateDto leituraDto, string correlationId);
    Task<Medidor?> ObterPorIdAsync(string medidorId);
    Task<Leitura?> ObterPorMedidorETimestampAsync(string medidorId, DateTime timestamp);
}