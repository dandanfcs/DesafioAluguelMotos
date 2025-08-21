using Application.Dtos;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using WebApi.Controllers;

namespace XUnitTestsIntegration.WebApiTests
{
    public class EntregadorControllerTests
    {
        private readonly Mock<IEntregadorService> _entregadorServiceMock;
        private readonly Mock<ICnhStorageService> _cnhStorageServiceMock;
        private readonly Mock<ILogger<EntregadorController>> _loggerMock;
        private readonly EntregadorController _controller;

        public EntregadorControllerTests()
        {
            _entregadorServiceMock = new Mock<IEntregadorService>();
            _cnhStorageServiceMock = new Mock<ICnhStorageService>();
            _loggerMock = new Mock<ILogger<EntregadorController>>();

            _controller = new EntregadorController(
                _entregadorServiceMock.Object,
                _cnhStorageServiceMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task UploadCnh_DeveRetornarOk_QuandoUploadForSucesso()
        {
            // Arrange
            var entregadorId = Guid.NewGuid();
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.FileName).Returns("cnh.png");

            _cnhStorageServiceMock
                .Setup(s => s.SaveCnhAsync(entregadorId, fileMock.Object))
                .ReturnsAsync("/path/to/cnh.png");

            // Act
            var result = await _controller.UploadCnh(entregadorId, fileMock.Object);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.True(okResult.Value.ToString().Contains("/path/to/cnh.png"));
            Assert.Equal(okResult.StatusCode, 200);
        }

        [Fact]
        public async Task CadastrarEntregador_DeveRetornarOk_QuandoCadastroForSucesso()
        {
            // Arrange
            var entregadorDto = new EntregadorDto
            {
                Nome = "Daniel",
                Cnpj = "12345678000190",
                NumeroCnh = "AB123456",
                TipoCnh = "A",
                ImagemCnh = new FormFile(new MemoryStream(new byte[0]), 0, 0, "Data", "cnh.png")
            };

            _entregadorServiceMock
                .Setup(s => s.ValidarEntregador(entregadorDto))
                .Returns(false);

            _entregadorServiceMock
                .Setup(s => s.AdicionarEntregadorAsync(entregadorDto, entregadorDto.ImagemCnh))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.CadastrarEntregador(entregadorDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Entregador cadastrado com sucesso!", okResult.Value);
        }

        [Fact]
        public async Task CadastrarEntregador_DeveRetornarBadRequest_QuandoValidacaoFalhar()
        {
            // Arrange
            var entregadorDto = new EntregadorDto
            {
                Nome = "",
                Cnpj = "",
                NumeroCnh = "",
                TipoCnh = "B"
            };

            _entregadorServiceMock
                .Setup(s => s.ValidarEntregador(entregadorDto))
                .Returns(true);

            // Act
            var result = await _controller.CadastrarEntregador(entregadorDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.True(badRequestResult.Value.ToString().Contains("Nome, Email e CPF são obrigatórios"));
        }

        [Fact]
        public async Task ObterEntregadores_DeveRetornarOk_ComListaDeEntregadores()
        {
            // Arrange
            List<Entregador> entregadores = new List<Entregador>
            {
                new() { Nome = "Daniel", Cnpj = "12345678000190" },
                new() { Nome = "Maria", Cnpj = "98765432000199" }
            };

            _entregadorServiceMock
                .Setup(s => s.ObterTodosAsync())
                .ReturnsAsync(entregadores);

            // Act
            var result = await _controller.ObterEntregadores();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);

            Assert.Equal(entregadores, okResult.Value);
        }
    }
}
