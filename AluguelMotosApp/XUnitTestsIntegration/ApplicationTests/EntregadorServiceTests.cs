using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Application.Dtos;
using Application.Interfaces;
using Application.Services;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Application.Tests.Services
{
    public class EntregadorServiceTests
    {
        private readonly Mock<IEntregadorRepository> _repositoryMock;
        private readonly Mock<ICnhStorageService> _cnhStorageMock;
        private readonly Mock<ILogger<EntregadorService>> _loggerMock;
        private readonly EntregadorService _service;

        public EntregadorServiceTests()
        {
            _repositoryMock = new Mock<IEntregadorRepository>();
            _cnhStorageMock = new Mock<ICnhStorageService>();
            _loggerMock = new Mock<ILogger<EntregadorService>>();

            _service = new EntregadorService(
                _repositoryMock.Object,
                _cnhStorageMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task AdicionarEntregadorAsync_DeveAdicionarEntregador_QuandoValido()
        {
            // Arrange
            var dto = new EntregadorDto
            {
                Nome = "João",
                Cnpj = "123456789",
                NumeroCnh = "CNH123",
                TipoCnh = "A",
                DataNascimento = new DateTime(1990, 1, 1)
            };

            var arquivoMock = new Mock<IFormFile>();

            _repositoryMock
                .Setup(r => r.ObterPorCnhOuCnpjAsync(dto.Cnpj, dto.NumeroCnh))
                .ReturnsAsync((Entregador)null);

            _cnhStorageMock
                .Setup(c => c.SaveCnhAsync(It.IsAny<Guid>(), arquivoMock.Object))
                .ReturnsAsync("caminho/arquivo");

            // Act
            await _service.AdicionarEntregadorAsync(dto, arquivoMock.Object);

            // Assert
            _repositoryMock.Verify(r => r.AdicionarAsync(It.Is<Entregador>(
                e => e.Nome == dto.Nome &&
                     e.Cnpj == dto.Cnpj &&
                     e.NumeroCnh == dto.NumeroCnh
            )), Times.Once);

            _cnhStorageMock.Verify(c => c.SaveCnhAsync(It.IsAny<Guid>(), arquivoMock.Object), Times.Once);
        }

        [Fact]
        public async Task AdicionarEntregadorAsync_DeveLancarException_QuandoCnpjOuCnhJaExistir()
        {
            // Arrange
            var dto = new EntregadorDto
            {
                Nome = "João",
                Cnpj = "123456789",
                NumeroCnh = "CNH123",
                TipoCnh = "A"
            };

            _repositoryMock
                .Setup(r => r.ObterPorCnhOuCnpjAsync(dto.Cnpj, dto.NumeroCnh))
                .ReturnsAsync(new Entregador());

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.AdicionarEntregadorAsync(dto, null));
        }

        [Fact]
        public void ValidarTipoDaCnh_DeveAceitarValoresValidos()
        {
            EntregadorService.ValidarTipoDaCnh("A");
            EntregadorService.ValidarTipoDaCnh("B");
            EntregadorService.ValidarTipoDaCnh("A+B");
        }

        [Fact]
        public void ValidarTipoDaCnh_DeveLancarException_QuandoInvalido()
        {
            Assert.Throws<InvalidOperationException>(() =>
                EntregadorService.ValidarTipoDaCnh("C"));
            Assert.Throws<ArgumentException>(() =>
                EntregadorService.ValidarTipoDaCnh(""));
        }

        [Fact]
        public async Task ObterPorIdAsync_DeveRetornarEntregador_QuandoExistir()
        {
            var id = Guid.NewGuid();
            var entregador = new Entregador { Id = id, Nome = "João" };

            _repositoryMock.Setup(r => r.ObterPorIdAsync(id)).ReturnsAsync(entregador);

            var resultado = await _service.ObterPorIdAsync(id);

            Assert.Equal(id, resultado.Id);
            Assert.Equal("João", resultado.Nome);
        }

        [Fact]
        public async Task ObterPorIdAsync_DeveLancarKeyNotFoundException_QuandoNaoExistir()
        {
            var id = Guid.NewGuid();

            _repositoryMock.Setup(r => r.ObterPorIdAsync(id)).ReturnsAsync((Entregador)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.ObterPorIdAsync(id));
        }

        [Fact]
        public async Task RemoverEntregadorAsync_DeveChamarRepositorio_QuandoExistir()
        {
            var id = Guid.NewGuid();
            var entregador = new Entregador { Id = id, Nome = "João" };

            _repositoryMock.Setup(r => r.ObterPorIdAsync(id)).ReturnsAsync(entregador);

            await _service.RemoverEntregadorAsync(id);

            _repositoryMock.Verify(r => r.RemoverAsync(id), Times.Once);
        }

        [Fact]
        public async Task RemoverEntregadorAsync_DeveLancarKeyNotFoundException_QuandoNaoExistir()
        {
            var id = Guid.NewGuid();
            _repositoryMock.Setup(r => r.ObterPorIdAsync(id)).ReturnsAsync((Entregador)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.RemoverEntregadorAsync(id));
        }

        [Fact]
        public void ValidarEntregador_DeveRetornarTrue_QuandoDtoInvalido()
        {
            var dto = new EntregadorDto { Nome = "", Cnpj = null, NumeroCnh = "" };
            var resultado = _service.ValidarEntregador(dto);
            Assert.True(resultado);
        }

        [Fact]
        public void ValidarEntregador_DeveRetornarFalse_QuandoDtoValido()
        {
            var dto = new EntregadorDto { Nome = "João", Cnpj = "123", NumeroCnh = "CNH123" };
            var resultado = _service.ValidarEntregador(dto);
            Assert.False(resultado);
        }
    }
}
