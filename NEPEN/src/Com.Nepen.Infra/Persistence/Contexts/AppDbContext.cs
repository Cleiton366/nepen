using Desafio_NEPEN.Com.Nepen.Core.Entities;
using Microsoft.EntityFrameworkCore;
namespace Desafio_NEPEN.Com.Nepen.Infra.Persistence.Contexts;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    
    public DbSet<Medidor> Medidores { get; set; }
    public DbSet<Leitura> Leituras { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Medidor>(entity =>
        {
            entity.HasKey(e => e.MedidorId);
        });

        modelBuilder.Entity<Leitura>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Medidor)
                .WithMany(m => m.Leituras)
                .HasForeignKey(e => e.MedidorId);
        });
    }
}