using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Moto> Moto => Set<Moto>();
        public DbSet<Entregador> Entregador { get; set; }
        public DbSet<Locacao> Locacao { get; set; }
        public DbSet<Notificacao> Notificacoes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Moto>(entity =>
            {
                entity.HasKey(m => m.Identificador);

                // Define índice único para Placa
                entity.HasIndex(m => m.Placa)
                      .IsUnique();

                entity.HasMany(m => m.Locacoes)
                    .WithOne(l => l.Moto)
                 .HasForeignKey(l => l.MotoId);
            });

            modelBuilder.Entity<Locacao>(entity =>
            {
                entity.HasKey(m => m.Id);

            });

            modelBuilder.Entity<Entregador>(entity =>
            {
                entity.HasKey(m => m.Id);

                entity.HasMany(e => e.Locacoes)
                .WithOne(l => l.Entregador)
                .HasForeignKey(l => l.EntregadorId);
             });

            modelBuilder.Entity<Notificacao>(entity =>
            {
                entity.HasKey(m => m.Id);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
