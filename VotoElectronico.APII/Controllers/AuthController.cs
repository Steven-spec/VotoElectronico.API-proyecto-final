using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.DTOs;
using VotoElectronico.APII.Data;
using VotoElectronico.APII.Services;


namespace VotoElectronico.APII.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly VotoElectronicoContext _context;
        private readonly IJwtService _jwtService;
        private readonly IEncriptacionService _encriptacion;

        public AuthController(
            VotoElectronicoContext context,
            IJwtService jwtService,
            IEncriptacionService encriptacion)
        {
            _context = context;
            _jwtService = jwtService;
            _encriptacion = encriptacion;
        }

        // POST: api/Auth/Login
        [HttpPost("Login")]
        public async Task<ActionResult<LoginResponseDto>> Login(LoginDto loginDto)
        {
            try
            {
                var passwordHash = _encriptacion.HashPassword(loginDto.Password);

                var usuario = await _context.Usuarios
                    .FirstOrDefaultAsync(u => u.Email == loginDto.Email && u.PasswordHash == passwordHash);

                if (usuario == null)
                {
                    return Ok(new LoginResponseDto
                    {
                        Success = false,
                        Message = "Credenciales inválidas"
                    });
                }

                if (!usuario.Activo)
                {
                    return Ok(new LoginResponseDto
                    {
                        Success = false,
                        Message = "Usuario inactivo"
                    });
                }

                var token = _jwtService.GenerarToken(usuario);

                return Ok(new LoginResponseDto
                {
                    Success = true,
                    Token = token,
                    Usuario = new UsuarioDto
                    {
                        Id = usuario.Id,
                        Nombres = usuario.Nombres,
                        Apellidos = usuario.Apellidos,
                        Email = usuario.Email,
                        Rol = usuario.Rol,
                        HaVotado = usuario.HaVotado
                    }
                });
            }
            catch (Exception ex)
            {
                return Ok(new LoginResponseDto
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        // POST: api/Auth/Register
        [HttpPost("Register")]
        public async Task<ActionResult<ApiResult<Usuario>>> Register(Usuario usuario)
        {
            try
            {
                // Verificar si ya existe
                if (await _context.Usuarios.AnyAsync(u => u.Cedula == usuario.Cedula))
                {
                    return ApiResult<Usuario>.Fail("La cédula ya está registrada");
                }

                if (await _context.Usuarios.AnyAsync(u => u.Email == usuario.Email))
                {
                    return ApiResult<Usuario>.Fail("El email ya está registrado");
                }

                // Encriptar password
                usuario.PasswordHash = _encriptacion.HashPassword(usuario.PasswordHash);
                usuario.FechaRegistro = DateTime.Now;
                usuario.Activo = true;

                _context.Usuarios.Add(usuario);
                await _context.SaveChangesAsync();

                // No devolver el password hash
                usuario.PasswordHash = string.Empty;

                return ApiResult<Usuario>.Ok(usuario);
            }
            catch (Exception ex)
            {
                return ApiResult<Usuario>.Fail(ex.Message);
            }
        }
    }
}