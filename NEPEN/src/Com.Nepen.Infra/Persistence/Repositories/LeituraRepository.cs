using Desafio_NEPEN.Com.Nepen.Core.Entities;
using Desafio_NEPEN.Com.Nepen.Core.Interfaces;
using Desafio_NEPEN.Com.Nepen.Infra.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Desafio_NEPEN.Com.Nepen.Infra.Persistence.Repositories;

public class LeituraRepository : ILeituraRepository
{
    private readonly AppDbContext _context;

    public LeituraRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Leitura> CriarAsync(Leitura leitura)
    {
        _context.Leituras.Add(leitura);
        await _context.SaveChangesAsync();
        return leitura;
    }

    public async Task<List<Leitura>> ObterPorMedidorAsync(string medidorId, DateTime dataInicio, DateTime dataFim, int limite = 100)
    {
        return await _context.Leituras
            .AsNoTracking()
            .Where(l => l.MedidorId == medidorId && l.Timestamp >= dataInicio && l.Timestamp <= dataFim)
            .OrderByDescending(l => l.Timestamp)
            .Take(limite)
            .ToListAsync();
    }

    public async Task<Leitura?> ObterPorMedidorETimestampAsync(string medidorId, DateTime timestamp)
    {
        return await _context.Leituras
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.MedidorId == medidorId && l.Timestamp == timestamp);
    }

    public async Task<Leitura?> ObterUltimaLeituraAsync(string medidorId, DateTime dataInicio, DateTime dataFim)
    {
        return await _context.Leituras
            .AsNoTracking()
            .Where(l => l.MedidorId == medidorId && l.Timestamp >= dataInicio && l.Timestamp <= dataFim)
            .OrderByDescending(l => l.Timestamp)
            .FirstOrDefaultAsync();
    }
}
