using Desafio_NEPEN.Com.Nepen.Core.Entities;

namespace Desafio_NEPEN.Com.Nepen.Core.Interfaces;

public interface ILeituraRepository
{
    Task<List<Leitura>> ObterPorMedidorAsync
        (string medidorId, DateTime dataInicio, DateTime dataFim, int limite = 100);
    Task<Leitura> CriarAsync(Leitura leitura);
    
    Task<Leitura?> ObterPorMedidorETimestampAsync(string medidorId, DateTime timestamp);

    Task<Leitura?> ObterUltimaLeituraAsync(string medidorId, DateTime dataInicio, DateTime dataFim);
}