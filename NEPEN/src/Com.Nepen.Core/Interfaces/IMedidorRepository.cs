using Desafio_NEPEN.Com.Nepen.Core.Entities;

namespace Desafio_NEPEN.Com.Nepen.Core.Interfaces;

public interface IMedidorRepository
{
    Task<Medidor?> ObterPorIdAsync(string medidorId);
    Task<Medidor> CriarAsync(Medidor medidor);
}