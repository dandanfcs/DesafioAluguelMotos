using Application.Dtos;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MotoController : ControllerBase
    {
        private readonly ILogger<MotoController> _logger;
        private readonly IMotoService _motoService;

        public MotoController(ILogger<MotoController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "motos")]
        public async void Motos()
        {
           await _motoService.GetAllAsync();
        }

        [HttpPost(Name = "motos")]
        public async void Motos(MotoDto motoDto)
        {
            await _motoService.GetAllAsync();
        }
    }
}
