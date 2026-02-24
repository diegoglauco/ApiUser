using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UsuariosApi.Data;
using UsuariosApi.Models;

using Microsoft.AspNetCore.Authorization;


namespace UsuariosApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsuariosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsuariosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/usuarios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UsuarioResponseDto>>> GetUsuarios()
        {
            var usuarios = await _context.Usuarios
                .AsNoTracking() // bom para consultas sem rastrear entidade
                .Select(u => new UsuarioResponseDto
                {
                    Id = u.Id,
                    Login = u.Login,
                    Admin = u.Admin
                })
                .ToListAsync(); // retorna lista completa

            return Ok(usuarios); // HTTP 200 + lista de DTOs
        }

        // GET: api/usuarios/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UsuarioResponseDto>> GetUsuario(int id)
        {
            var usuario = await _context.Usuarios
                .AsNoTracking()
                .Where(u => u.Id == id) // filtra primeiro
                .Select(u => new UsuarioResponseDto
                {
                    Id = u.Id,
                    Login = u.Login,
                    Admin = u.Admin
                })
                .FirstOrDefaultAsync(); // pega o primeiro ou null

            if (usuario is null)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Usuário não encontrado",
                    Detail = $"Não existe usuário com id {id}.",
                    Status = StatusCodes.Status404NotFound,
                    Instance = HttpContext.Request.Path
                });
            }

            return Ok(usuario);
        }

        // POST: api/usuarios
        [HttpPost]
        public async Task<ActionResult<UsuarioResponseDto>> PostUsuario(UsuarioCreateDto dto)
        {
            try
            {
                // Validação mínima
                if (string.IsNullOrWhiteSpace(dto.Login) || string.IsNullOrWhiteSpace(dto.Senha))
                {
                    return BadRequest(new ProblemDetails
                    {
                        Title = "Dados inválidos",
                        Detail = "Login e Senha são obrigatórios.",
                        Status = StatusCodes.Status400BadRequest,
                        Instance = HttpContext.Request.Path
                    });
                }

                // Define a data de criação
                var dataCriacao = dto.DataCriacao ?? DateTime.UtcNow;


                // Cria entidade
                var usuario = new Usuario
                {
                    Login = dto.Login,
                    Senha = PasswordHelper.HashSenha(dto.Senha),
                    Admin = dto.Admin,
                    DataCriacao = dataCriacao
                };
                Console.WriteLine($"Admin  --> : {dto.Admin}");
                _context.Usuarios.Add(usuario);
                await _context.SaveChangesAsync();

                // Retorna DTO de resposta
                var response = new UsuarioResponseDto
                {
                    Id = usuario.Id,
                    Login = usuario.Login,
                    Admin = usuario.Admin
                };

                return CreatedAtAction(nameof(GetUsuario), new { id = usuario.Id }, response);
            }
            catch (DbUpdateException ex)
            {
                // Logando o erro
                return StatusCode(500, $"Erro ao salvar usuário: {ex.InnerException?.Message ?? ex.Message}");
            }
        }


        [HttpPut("{id:int}")]
        public async Task<ActionResult<UsuarioResponseDto>> PutUsuario(int id, UsuarioUpdateDto dto)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Id == id);

            if (usuario is null)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Usuário não encontrado",
                    Detail = $"Não existe usuário com id {id}.",
                    Status = StatusCodes.Status404NotFound,
                    Instance = HttpContext.Request.Path
                });
            }
            if (string.IsNullOrWhiteSpace(dto.Login))
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Login inválido",
                    Detail = "O campo Login não pode ser vazio.",
                    Status = StatusCodes.Status400BadRequest,
                    Instance = HttpContext.Request.Path
                });
            }

            // Atualiza apenas os campos permitidos
            usuario.Login = dto.Login;
            usuario.Admin = dto.Admin;

            await _context.SaveChangesAsync();

            var response = new UsuarioResponseDto
            {
                Id = usuario.Id,
                Login = usuario.Login,
                Admin = usuario.Admin
            };

            return Ok(response);
        }


        // DELETE: api/usuarios/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario is null)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Usuário não encontrado",
                    Detail = $"Não existe usuário com id {id}.",
                    Status = StatusCodes.Status404NotFound,
                    Instance = HttpContext.Request.Path
                });
            }

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
