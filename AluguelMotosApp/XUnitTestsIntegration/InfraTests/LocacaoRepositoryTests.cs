using System;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Infrastructure.Tests.Repositories
{
    public class LocacaoRepositoryTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly LocacaoRepository _repository;

        public LocacaoRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _repository = new LocacaoRepository(_context);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Fact]
        public async Task AdicionarAsync_DeveAdicionarLocacao()
        {
            // Arrange
            var locacao = new Locacao
            {
                Id = Guid.NewGuid(),
                EntregadorId = Guid.NewGuid(),
                MotoId = "M123"
            };

            // Act
            await _repository.AdicionarAsync(locacao);

            // Assert
            var inserido = await _context.Locacao.FindAsync(locacao.Id);
            Assert.NotNull(inserido);
            Assert.Equal("M123", inserido.MotoId);
        }

        [Fact]
        public async Task AtualizarAsync_DeveAtualizarLocacao()
        {
            // Arrange
            var locacao = new Locacao
            {
                Id = Guid.NewGuid(),
                EntregadorId = Guid.NewGuid(),
                MotoId = "M123"
            };
            await _context.Locacao.AddAsync(locacao);
            await _context.SaveChangesAsync();

            // Act
            locacao.MotoId = "M456";
            await _repository.AtualizarAsync(locacao);

            // Assert
            var atualizado = await _context.Locacao.FindAsync(locacao.Id);
            Assert.Equal("M456", atualizado.MotoId);
        }

        [Fact]
        public async Task ExisteLocacaoAtivaParaMotoAsync_DeveRetornarTrue_QuandoExistirLocacaoAtiva()
        {
            // Arrange
            var locacao = new Locacao { Id = Guid.NewGuid(), MotoId = "M1", DataFim = null };
            await _context.Locacao.AddAsync(locacao);
            await _context.SaveChangesAsync();

            // Act
            var existe = await _repository.ExisteLocacaoAtivaParaMotoAsync("M1");

            // Assert
            Assert.True(existe);
        }

        [Fact]
        public async Task ExisteAlgumaLocacaoComEsseEntregadorIdEMotoId_DeveRetornarTrue_QuandoExistir()
        {
            // Arrange
            var entregadorId = Guid.NewGuid();
            var locacao = new Locacao { Id = Guid.NewGuid(), EntregadorId = entregadorId, MotoId = "M1" };
            await _context.Locacao.AddAsync(locacao);
            await _context.SaveChangesAsync();

            // Act
            var existe = await _repository.ExisteAlgumaLocacaoComEsseEntregadorIdEMotoId(entregadorId, "M1");

            // Assert
            Assert.True(existe);
        }
    }
}
