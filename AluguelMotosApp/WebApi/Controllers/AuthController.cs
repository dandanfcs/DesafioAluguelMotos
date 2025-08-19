using Application.Dtos;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Cria um novo usuário com a role informada (ex: Admin, Entregador).
        /// </summary>
        [HttpPost("register")]
        [AllowAnonymous] // pode ser público
        public async Task<IActionResult> CadastrarUsuario([FromBody] UserDto request)
        {
            try
            {
                var userId = await _userService.RegistrarUsuarioAsync(request.Email, request.Password, request.Role);
                return Ok(new { UserId = userId, Message = "Usuário criado com sucesso!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }


        /// <summary>
        /// Faz login e retorna um token JWT.
        /// </summary>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            var token = await _userService.LoginAsync(request.Email, request.Password);

            if (token == null)
                return Unauthorized(new { Message = "Credenciais inválidas" });

            return Ok(new { Token = token });
        }
    }

}
