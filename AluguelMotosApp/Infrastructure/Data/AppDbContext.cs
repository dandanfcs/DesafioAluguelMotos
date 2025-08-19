using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Moto> Moto => Set<Moto>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Moto>(entity =>
            {
                // Define a chave primária
                entity.HasKey(m => m.Identificador);

                // Define índice único para Placa
                entity.HasIndex(m => m.Placa)
                      .IsUnique();
            }); 

            base.OnModelCreating(modelBuilder);
        }
    }
}
