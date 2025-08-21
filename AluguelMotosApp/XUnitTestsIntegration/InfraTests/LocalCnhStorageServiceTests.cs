using Application.Interfaces;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace XUnitTestsIntegration.InfraTests
{
    public class LocalCnhStorageServiceTests
    {
        private readonly Mock<ILogger<LocalCnhStorageService>> _loggerMock;
        private readonly LocalCnhStorageService _service;

        public LocalCnhStorageServiceTests()
        {
            _loggerMock = new Mock<ILogger<LocalCnhStorageService>>();
            _service = new LocalCnhStorageService(_loggerMock.Object);
        }

        [Fact]
        public async Task SaveCnhAsync_DeveSalvarArquivo_ComSucesso()
        {
            // Arrange
            var entregadorId = Guid.NewGuid();
            var fileMock = new Mock<IFormFile>();
            var content = "conteudo fake";
            var fileName = "cnh.png";
            var ms = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(content));
            fileMock.Setup(f => f.FileName).Returns(fileName);
            fileMock.Setup(f => f.Length).Returns(ms.Length);
            fileMock.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), default))
                .Returns<Stream, CancellationToken>((stream, _) =>
                {
                    ms.Position = 0;
                    return ms.CopyToAsync(stream);
                });

            // Act
            var path = await _service.SaveCnhAsync(entregadorId, fileMock.Object);

            // Assert
            Assert.True(File.Exists(path));
            Assert.EndsWith(".png", path);
        }

        [Fact]
        public async Task SaveCnhAsync_ArquivoNulo_DeveLancarExcecao()
        {
            // Arrange
            var entregadorId = Guid.NewGuid();

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _service.SaveCnhAsync(entregadorId, null));
        }

        [Fact]
        public async Task SaveCnhAsync_ArquivoVazio_DeveLancarExcecao()
        {
            // Arrange
            var entregadorId = Guid.NewGuid();
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.Length).Returns(0);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _service.SaveCnhAsync(entregadorId, fileMock.Object));
        }

        [Fact]
        public async Task SaveCnhAsync_ArquivoJaExiste_DeveSobrescrever()
        {
            // Arrange
            var entregadorId = Guid.NewGuid();
            var fileMock = new Mock<IFormFile>();
            var content = "conteudo fake";
            var fileName = "cnh.jpg";
            var ms = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(content));

            fileMock.Setup(f => f.FileName).Returns(fileName);
            fileMock.Setup(f => f.Length).Returns(ms.Length);
            fileMock.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), default))
                .Returns<Stream, CancellationToken>((stream, _) =>
                {
                    ms.Position = 0;
                    return ms.CopyToAsync(stream);
                });

            // Primeiro salvamento
            var path = await _service.SaveCnhAsync(entregadorId, fileMock.Object);

            // Segundo salvamento deve sobrescrever
            var path2 = await _service.SaveCnhAsync(entregadorId, fileMock.Object);

            // Assert
            Assert.Equal(path, path2);
            Assert.True(File.Exists(path2));
        }
    }
}
