using Application.Dtos;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using WebApi.Controllers;

public class MotoControllerTests 
{
    private readonly MotoController _controller;
    private readonly Mock<IMotoService> _motoServiceMock;
    private readonly Mock<ILogger<MotoController>> _loggerMock;

    public MotoControllerTests()
    {
        _motoServiceMock = new Mock<IMotoService>();
        _loggerMock = new Mock<ILogger<MotoController>>();

        _controller = new MotoController(_motoServiceMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task ListarMotos_DeveRetornarOkComLista()
    {
        var motos = new List<Moto> { new() { Identificador = "M1" }, new() { Identificador = "M2" } };
        _motoServiceMock.Setup(s => s.ListarMotosCadastradasAsync()).ReturnsAsync(motos);

        var result = await _controller.ListarMotos();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var list = Assert.IsAssignableFrom<IEnumerable<Moto>>(okResult.Value);
        Assert.Equal(2, ((List<Moto>)list).Count);
    }

    [Fact]
    public async Task CadastrarMoto_DeveRetornarOk()
    {
        var motoDto = new MotoDto { Identificador = "M1", Modelo = "CB500", Placa = "ABC123", Ano = 2023 };

        var result = await _controller.CadastrarMoto(motoDto);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
    }

    [Fact]
    public async Task ObterMotoPorId_DeveRetornarNotFound_SeNaoExistir()
    {
        _motoServiceMock.Setup(s => s.ObterMotoPorIdAsync("ID_INEXISTENTE")).ReturnsAsync((Moto)null);

        var result = await _controller.ObterMotoPorId("ID_INEXISTENTE");

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task AtualizarPlaca_DeveRetornarNoContent_SeSucesso()
    {
        _motoServiceMock.Setup(s => s.AtualizarPlacaAsync("M1", "NOVAPLACA")).ReturnsAsync(true);
        var placaDto = new PlacaDto { Placa = "NOVAPLACA" };

        var result = await _controller.AtualizarPlaca("M1", placaDto);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task ObterMotoPelaPlaca_DeveRetornarBadRequest_SePlacaVazia()
    {
        var placaDto = "";

        var result = await _controller.ObterMotoPelaPlaca(placaDto);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequest.StatusCode);
    }
}
