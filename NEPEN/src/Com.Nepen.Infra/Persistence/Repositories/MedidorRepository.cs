using Desafio_NEPEN.Com.Nepen.Core.Entities;
using Desafio_NEPEN.Com.Nepen.Core.Interfaces;
using Desafio_NEPEN.Com.Nepen.Infra.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Desafio_NEPEN.Com.Nepen.Infra.Persistence.Repositories;

public class MedidorRepository : IMedidorRepository
{
    private readonly AppDbContext _context;

    public MedidorRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Medidor?> ObterPorIdAsync(string medidorId)
    {
        return await _context.Medidores
            .Include(m => m.Leituras)
            .FirstOrDefaultAsync(m => m.MedidorId == medidorId);
    }
    
    public async Task<Medidor> CriarAsync(Medidor medidor)
    {
        _context.Medidores.Add(medidor);
        await _context.SaveChangesAsync();
        return medidor;
    }
}