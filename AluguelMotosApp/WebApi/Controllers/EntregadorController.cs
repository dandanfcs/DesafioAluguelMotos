using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Application.Interfaces;
using Infrastructure.Interfaces;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EntregadorController : ControllerBase
    {
        private readonly ICnhStorageService _cnhStorageService;

        public EntregadorController(ICnhStorageService cnhStorageService)
        {
            _cnhStorageService = cnhStorageService;
        }

        [HttpPost("{entregadorId}/upload-cnh")]
        public async Task<IActionResult> UploadCnh([FromRoute] Guid entregadorId, IFormFile file)
        {
            if (file == null)
                return BadRequest("Nenhum arquivo enviado.");

            var allowedExtensions = new[] { ".png", ".bmp" };
            var extension = Path.GetExtension(file.FileName).ToLower();

            if (!allowedExtensions.Contains(extension))
                return BadRequest("Formato de arquivo inválido. Apenas PNG ou BMP são permitidos.");

            var filePath = await _cnhStorageService.SaveCnhAsync(entregadorId, file);

            return Ok(new { FilePath = filePath });
        }
    }
}
