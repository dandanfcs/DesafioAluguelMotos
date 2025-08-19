using Application.Dtos;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")] // Somente Admin pode acessar toda a controller
    public class MotoController : ControllerBase
    {
        private readonly IMotoService _motoService;

        public MotoController(IMotoService motoService)
        {
            _motoService = motoService;
        }

        /// <summary>
        /// Retorna todas as motos
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var motos = await _motoService.GetAllAsync();
            return Ok(motos);
        }

        /// <summary>
        /// Retorna uma moto pelo ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var moto = await _motoService.GetByIdAsync(id);
            if (moto == null)
                return NotFound(new { Message = "Moto não encontrada" });

            return Ok(moto);
        }

        /// <summary>
        /// Adiciona uma nova moto
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] MotoDto motoDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _motoService.AddAsync(motoDto);
            return Ok(new { Message = "Moto adicionada com sucesso!" });
        }
    }
}
