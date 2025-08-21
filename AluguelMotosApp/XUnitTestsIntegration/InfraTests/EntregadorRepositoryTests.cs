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
    public class EntregadorRepositoryTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly EntregadorRepository _repository;

        public EntregadorRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _repository = new EntregadorRepository(_context);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Fact]
        public async Task AdicionarAsync_DeveAdicionarEntregador()
        {
            // Arrange
            var entregador = new Entregador
            {
                Id = Guid.NewGuid(),
                Nome = "João",
                Cnpj = "123456",
                NumeroCnh = "CNH123"
            };

            // Act
            await _repository.AdicionarAsync(entregador);

            // Assert
            var inserido = await _context.Entregador.FindAsync(entregador.Id);
            Assert.NotNull(inserido);
            Assert.Equal("João", inserido.Nome);
        }

        [Fact]
        public async Task AtualizarAsync_DeveAtualizarEntregador()
        {
            // Arrange
            var entregador = new Entregador
            {
                Id = Guid.NewGuid(),
                Nome = "João",
                Cnpj = "123456",
                NumeroCnh = "CNH123"
            };
            await _context.Entregador.AddAsync(entregador);
            await _context.SaveChangesAsync();

            // Act
            entregador.Nome = "Maria";
            await _repository.AtualizarAsync(entregador);

            // Assert
            var atualizado = await _context.Entregador.FindAsync(entregador.Id);
            Assert.Equal("Maria", atualizado.Nome);
        }

        [Fact]
        public async Task ObterPorCnhOuCnpjAsync_DeveRetornarEntregador_QuandoExistir()
        {
            // Arrange
            var entregador = new Entregador
            {
                Id = Guid.NewGuid(),
                Nome = "João",
                Cnpj = "123456",
                NumeroCnh = "CNH123"
            };
            await _context.Entregador.AddAsync(entregador);
            await _context.SaveChangesAsync();

            // Act
            var resultado = await _repository.ObterPorCnhOuCnpjAsync("123456", "CNH123");

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(entregador.Id, resultado.Id);
        }

        [Fact]
        public async Task ObterPorIdAsync_DeveRetornarEntregador_QuandoExistir()
        {
            // Arrange
            var entregador = new Entregador
            {
                Id = Guid.NewGuid(),
                Nome = "João"
            };
            await _context.Entregador.AddAsync(entregador);
            await _context.SaveChangesAsync();

            // Act
            var resultado = await _repository.ObterPorIdAsync(entregador.Id);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(entregador.Id, resultado.Id);
        }

        [Fact]
        public async Task ObterTodosAsync_DeveRetornarTodosEntregadores()
        {
            // Arrange
            var entregador1 = new Entregador { Id = Guid.NewGuid(), Nome = "João" };
            var entregador2 = new Entregador { Id = Guid.NewGuid(), Nome = "Maria" };
            await _context.Entregador.AddRangeAsync(entregador1, entregador2);
            await _context.SaveChangesAsync();

            // Act
            var todos = await _repository.ObterTodosAsync();

            // Assert
            Assert.Equal(2, todos.Count());
        }

        [Fact]
        public async Task RemoverAsync_DeveRemoverEntregador_QuandoExistir()
        {
            // Arrange
            var entregador = new Entregador
            {
                Id = Guid.NewGuid(),
                Nome = "João"
            };
            await _context.Entregador.AddAsync(entregador);
            await _context.SaveChangesAsync();

            // Act
            await _repository.RemoverAsync(entregador.Id);

            // Assert
            var removido = await _context.Entregador.FindAsync(entregador.Id);
            Assert.Null(removido);
        }

        [Fact]
        public async Task RemoverAsync_NaoFazNada_QuandoEntregadorNaoExistir()
        {
            // Act
            await _repository.RemoverAsync(Guid.NewGuid());

            // Assert
            var count = await _context.Entregador.CountAsync();
            Assert.Equal(0, count);
        }
    }
}
