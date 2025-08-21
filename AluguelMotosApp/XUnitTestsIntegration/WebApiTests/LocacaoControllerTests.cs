using Application.Dtos;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using WebApi.Controllers;

namespace WebApi.Tests.Controllers
{
    public class LocacaoControllerTests
    {
        private readonly Mock<ILocacaoService> _locacaoServiceMock;
        private readonly Mock<ILogger<LocacaoController>> _loggerMock;
        private readonly LocacaoController _controller;

        public LocacaoControllerTests()
        {
            _locacaoServiceMock = new Mock<ILocacaoService>();
            _loggerMock = new Mock<ILogger<LocacaoController>>();
            _controller = new LocacaoController(_locacaoServiceMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task AlugarMoto_DeveRetornarOk_QuandoLocacaoCriada()
        {
            // Arrange
            var dto = new LocacaoDto { EntregadorId = Guid.NewGuid(), MotoId = "12345", Plano = 7 };
            var locacaoEsperada = new Locacao { EntregadorId = dto.EntregadorId, MotoId = dto.MotoId };
           
            _locacaoServiceMock.Setup(s => s.CriarLocacaoAsync(dto))
               .ReturnsAsync(locacaoEsperada);

            // Act
            var resultado = await _controller.AlugarMoto(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado);
       
            Assert.Equal(okResult.StatusCode, 200);
        }

        [Fact]
        public async Task AlugarMoto_DeveRetornar500_QuandoException()
        {
            // Arrange
            var dto = new LocacaoDto { EntregadorId = Guid.NewGuid(), MotoId = "12345", Plano = 7 };

            _locacaoServiceMock.Setup(s => s.CriarLocacaoAsync(dto))
                .ThrowsAsync(new Exception("Erro interno"));

            // Act
            var resultado = await _controller.AlugarMoto(dto);

            // Assert
            var status = Assert.IsType<ObjectResult>(resultado);
            Assert.Equal(500, status.StatusCode);
        }

        [Fact]
        public async Task ObterLocacao_DeveRetornarOk_QuandoEncontrada()
        {
            // Arrange
            var id = Guid.NewGuid();
            var locacao = new Locacao { EntregadorId = id, MotoId = "12345" };

            _locacaoServiceMock.Setup(s => s.ObterPorIdAsync(id))
               .ReturnsAsync(locacao);

            // Act
            var resultado = await _controller.ObterLocacao(id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado);

            Assert.Equal(okResult.StatusCode, 200);
            Assert.Equal(id, ((Domain.Entities.Locacao)okResult.Value).EntregadorId);
        }

        [Fact]
        public async Task ObterLocacao_DeveRetornarNotFound_QuandoNaoEncontrada()
        {
            // Arrange
            var id = Guid.NewGuid();

            _locacaoServiceMock.Setup(s => s.ObterPorIdAsync(id))
                .ThrowsAsync(new KeyNotFoundException());

            // Act
            var resultado = await _controller.ObterLocacao(id);

            // Assert
            var notFound = Assert.IsType<NotFoundObjectResult>(resultado);
            Assert.Equal("Locação não encontrada.", notFound.Value);
        }

        [Fact]
        public async Task ConsultarLocacao_DeveRetornarOk_QuandoCalculoSucesso()
        {
            // Arrange
            var id = Guid.NewGuid();
            var dataDevolucao = DateTime.UtcNow.AddDays(5);
            var dto = new DataDevolucaoLocacaoDto { DataDevolucao = dataDevolucao };
            decimal valorEsperado = 350.75m;

            // Act
            _locacaoServiceMock.Setup(s => s.CalcularValorFinalAsync(id, dataDevolucao))
                .ReturnsAsync(valorEsperado);

            var resultado = await _controller.ConsultarLocacao(id, dto);
           

            //Assert
            Assert.Equal(valorEsperado, ((Microsoft.AspNetCore.Mvc.ObjectResult)resultado).Value);

        }

        [Fact]
        public async Task ConsultarLocacao_DeveRetornarBadRequest_QuandoRegraNegocioInvalida()
        {
            // Arrange
            var id = Guid.NewGuid();
            var dto = new DataDevolucaoLocacaoDto { DataDevolucao = DateTime.UtcNow };
            _locacaoServiceMock.Setup(s => s.CalcularValorFinalAsync(id, dto.DataDevolucao))
                .ThrowsAsync(new InvalidOperationException("Data inválida"));

            // Act
            var resultado = await _controller.ConsultarLocacao(id, dto);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(resultado);
            Assert.Equal("Data inválida", badRequest.Value);
        }

        [Fact]
        public async Task ConsultarLocacao_DeveRetornar500_QuandoException()
        {
            // Arrange
            var id = Guid.NewGuid();
            var dto = new DataDevolucaoLocacaoDto { DataDevolucao = DateTime.UtcNow };
            _locacaoServiceMock.Setup(s => s.CalcularValorFinalAsync(id, dto.DataDevolucao))
                .ThrowsAsync(new Exception("Erro inesperado"));

            // Act
            var resultado = await _controller.ConsultarLocacao(id, dto);

            // Assert
            var status = Assert.IsType<ObjectResult>(resultado);
            Assert.Equal(500, status.StatusCode);
        }
    }
}
