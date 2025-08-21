using Application.Dtos;
using Application.Services;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

public class MotoServiceTests
{
    private readonly MotoService _service;
    private readonly Mock<IMotoRepository> _motoRepoMock = new();
    private readonly Mock<ILocacaoRepository> _locacaoRepoMock = new();
    private readonly Mock<ILogger<MotoService>> _loggerMock = new();

    public MotoServiceTests()
    {
        // Passa o Mock do logger no construtor
        _service = new MotoService(_motoRepoMock.Object, _locacaoRepoMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task ListarMotosCadastradasAsync_DeveRetornarLista()
    {
        // Arrange
        var motos = new List<Moto> { new() { Identificador = "M1" }, new() { Identificador = "M2" } };
        _motoRepoMock.Setup(x => x.ListarMotosCadastradasAsync()).ReturnsAsync(motos);

        // Act
        var result = await _service.ListarMotosCadastradasAsync();

        // Assert
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task CadastrarMotoAsync_DeveChamarRepositorio()
    {
        // Arrange
        var motoDto = new MotoDto
        {
            Identificador = "M123",
            Modelo = "CB500",
            Placa = "ABC1234",
            Ano = 2023
        };

        // Act
        await _service.CadastrarMotoAsync(motoDto);

        // Assert
        _motoRepoMock.Verify(
            x => x.CadastrarMotoAsync(It.Is<Moto>(m => m.Identificador == "M123")),
            Times.Once
        );
    }

    [Fact]
    public async Task AtualizarPlacaAsync_DeveRetornarTrue_QuandoAtualizado()
    {
        _motoRepoMock.Setup(x => x.AtualizarPlacaDaMotoAsync("M1", "NOVAPLACA")).ReturnsAsync(1);

        var result = await _service.AtualizarPlacaAsync("M1", "NOVAPLACA");

        Assert.True(result);
    }

    [Fact]
    public async Task RemoverMotoAsync_DeveLancarErro_SeLocacaoAtiva()
    {
        _locacaoRepoMock.Setup(x => x.ExisteLocacaoAtivaParaMotoAsync("M1")).ReturnsAsync(true);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.RemoverMotoAsync("M1"));
    }
}
