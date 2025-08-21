using Application.Dtos;
using Application.Services;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

public class LocacaoServiceTests
{
    private readonly Mock<ILocacaoRepository> _locacaoRepoMock;
    private readonly Mock<IEntregadorRepository> _entregadorRepoMock;
    private readonly Mock<IMotoRepository> _motoRepoMock;
    private readonly Mock<ILogger<LocacaoService>> _loggerMock;
    private readonly LocacaoService _service;

    public LocacaoServiceTests()
    {
        _locacaoRepoMock = new Mock<ILocacaoRepository>();
        _entregadorRepoMock = new Mock<IEntregadorRepository>();
        _motoRepoMock = new Mock<IMotoRepository>();
        _loggerMock = new Mock<ILogger<LocacaoService>>();

        _service = new LocacaoService(
            _locacaoRepoMock.Object,
            _entregadorRepoMock.Object,
            _motoRepoMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task CriarLocacaoAsync_DeveCriarLocacaoComSucesso()
    {
        // Arrange
        var entregador = new Entregador { Id = Guid.NewGuid(), TipoCnh = "A" };
        var moto = new Moto { Identificador = "123456", Placa = "ABC1234" };

        var dto = new LocacaoDto
        {
            EntregadorId = entregador.Id,
            MotoId = moto.Identificador,
            Plano = 7
        };

        _entregadorRepoMock.Setup(r => r.ObterPorIdAsync(dto.EntregadorId)).ReturnsAsync(entregador);
        _motoRepoMock.Setup(r => r.ObterMotoPorIdAsync(dto.MotoId)).ReturnsAsync(moto);
        _locacaoRepoMock.Setup(r => r.ExisteAlgumaLocacaoComEsseEntregadorIdEMotoId(dto.EntregadorId, dto.MotoId))
                        .ReturnsAsync(false);

        // Act
        var locacao = await _service.CriarLocacaoAsync(dto);

        // Assert
        Assert.Equal(dto.MotoId, locacao.MotoId);
        Assert.Equal(dto.EntregadorId, locacao.EntregadorId);
        _locacaoRepoMock.Verify(r => r.AdicionarAsync(It.IsAny<Locacao>()), Times.Once);
    }

    [Fact]
    public async Task CriarLocacaoAsync_DeveFalhar_SeEntregadorNaoPossuiCnhA()
    {
        var entregador = new Entregador { Id = Guid.NewGuid(), TipoCnh = "B" };
        var dto = new LocacaoDto { EntregadorId = entregador.Id, MotoId = "123456", Plano = 7 };

        _entregadorRepoMock.Setup(r => r.ObterPorIdAsync(dto.EntregadorId)).ReturnsAsync(entregador);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CriarLocacaoAsync(dto));
    }

    [Fact]
    public async Task CriarLocacaoAsync_DeveFalhar_SeMotoNaoExiste()
    {
        var entregador = new Entregador { Id = Guid.NewGuid(), TipoCnh = "A" };
        var dto = new LocacaoDto { EntregadorId = entregador.Id, MotoId = "123456", Plano = 7 };

        _entregadorRepoMock.Setup(r => r.ObterPorIdAsync(dto.EntregadorId)).ReturnsAsync(entregador);
        _motoRepoMock.Setup(r => r.ObterMotoPorIdAsync(dto.MotoId)).ReturnsAsync((Moto?)null);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CriarLocacaoAsync(dto));
    }

    [Fact]
    public async Task CriarLocacaoAsync_DeveFalhar_SeJaExisteLocacaoAtiva()
    {
        var entregador = new Entregador { Id = Guid.NewGuid(), TipoCnh = "A" };
        var moto = new Moto {Identificador = "123456", Placa = "ABC1234" };
        var dto = new LocacaoDto { EntregadorId = entregador.Id, MotoId = moto.Identificador, Plano = 7 };

        _entregadorRepoMock.Setup(r => r.ObterPorIdAsync(dto.EntregadorId)).ReturnsAsync(entregador);
        _motoRepoMock.Setup(r => r.ObterMotoPorIdAsync(dto.MotoId)).ReturnsAsync(moto);
        _locacaoRepoMock.Setup(r => r.ExisteAlgumaLocacaoComEsseEntregadorIdEMotoId(dto.EntregadorId, dto.MotoId))
                        .ReturnsAsync(true);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CriarLocacaoAsync(dto));
    }

    [Fact]
    public async Task CalcularValorFinalAsync_DeveRetornarValorCorreto_QuandoDevolucaoNoPrazo()
    {
        var id = Guid.NewGuid();
        var locacao = new Locacao
        {
            Id = id,
            DataInicio = DateTime.UtcNow.Date,
            DataPrevisaoTermino = DateTime.UtcNow.Date.AddDays(7),
            valorTotalLocacao = 700m // supondo 100/dia
        };

        _locacaoRepoMock.Setup(r => r.ObterPorIdAsync(id)).ReturnsAsync(locacao);

        var valor = await _service.CalcularValorFinalAsync(id, locacao.DataPrevisaoTermino);

        Assert.Equal(700m, valor);
    }

    [Fact]
    public async Task CalcularValorFinalAsync_DeveAplicarMultaPorDevolucaoAntecipada()
    {
        var id = Guid.NewGuid();
        var inicio = DateTime.UtcNow.Date;
        var previsao = inicio.AddDays(7);
        var locacao = new Locacao
        {
            Id = id,
            DataInicio = inicio,
            DataPrevisaoTermino = previsao,
            valorTotalLocacao = 700m
        };

        _locacaoRepoMock.Setup(r => r.ObterPorIdAsync(id)).ReturnsAsync(locacao);

        var devolucaoAntecipada = inicio.AddDays(3); // devolveu no 3º dia
        var valor = await _service.CalcularValorFinalAsync(id, devolucaoAntecipada);

        Assert.True(valor < 700m); // tem multa mas paga menos que o total
    }

    [Fact]
    public async Task CalcularValorFinalAsync_DeveAplicarMultaPorDevolucaoAtrasada()
    {
        var id = Guid.NewGuid();
        var inicio = DateTime.UtcNow.Date;
        var previsao = inicio.AddDays(7);
        var locacao = new Locacao
        {
            Id = id,
            DataInicio = inicio,
            DataPrevisaoTermino = previsao,
            valorTotalLocacao = 700m
        };

        _locacaoRepoMock.Setup(r => r.ObterPorIdAsync(id)).ReturnsAsync(locacao);

        var devolucaoAtrasada = previsao.AddDays(2); // 2 dias de atraso
        var valor = await _service.CalcularValorFinalAsync(id, devolucaoAtrasada);

        Assert.Equal(700m + 100m, valor); // multa = 50 * 2
    }

    [Fact]
    public async Task ObterPorIdAsync_DeveLancarExcecao_SeNaoEncontrar()
    {
        var id = Guid.NewGuid();
        _locacaoRepoMock.Setup(r => r.ObterPorIdAsync(id)).ReturnsAsync((Locacao?)null);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.ObterPorIdAsync(id));
    }
}
