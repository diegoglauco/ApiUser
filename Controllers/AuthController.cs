using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace UsuariosApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {

            var username = _configuration["Auth:Username"]!;
            var password = _configuration["Auth:Password"]!;
            // Teste: qualquer usuário "admin" com senha "123"
            if (request.Username == username && request.Password == password)
            {
                var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!);
                var securityKey = new SymmetricSecurityKey(key);
                var creds = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.Name, request.Username)
                    }),
                    Expires = DateTime.UtcNow.AddMinutes(30),
                    SigningCredentials = creds
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                var jwt = tokenHandler.WriteToken(token);

                return Ok(new { token = jwt });
            }

            return Unauthorized("Usuário ou senha inválidos");
        }
    }

    // Classe simples para receber login
    public record LoginRequest(string Username, string Password);
}
