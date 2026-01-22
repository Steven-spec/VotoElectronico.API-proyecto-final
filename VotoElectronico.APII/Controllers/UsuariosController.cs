using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using VotoElectronico.APII.Data;
using VotoElectronico.APII.Services;


namespace VotoElectronico.APII.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly VotoElectronicoContext _context;
        private readonly IEncriptacionService _encriptacion;

        public UsuariosController(VotoElectronicoContext context, IEncriptacionService encriptacion)
        {
            _context = context;
            _encriptacion = encriptacion;
        }

        // GET: api/Usuarios
        [HttpGet]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult<ApiResult<List<Usuario>>>> GetUsuarios()
        {
            try
            {
                var usuarios = await _context.Usuarios
                    .Select(u => new Usuario
                    {
                        Id = u.Id,
                        Cedula = u.Cedula,
                        Nombres = u.Nombres,
                        Apellidos = u.Apellidos,
                        Email = u.Email,
                        Rol = u.Rol,
                        Activo = u.Activo,
                        HaVotado = u.HaVotado,
                        FechaRegistro = u.FechaRegistro,
                        FechaVoto = u.FechaVoto,
                        PasswordHash = "" // No devolver passwords
                    })
                    .ToListAsync();

                return ApiResult<List<Usuario>>.Ok(usuarios);
            }
            catch (Exception ex)
            {
                return ApiResult<List<Usuario>>.Fail(ex.Message);
            }
        }

        // GET: api/Usuarios/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<ApiResult<Usuario>>> GetUsuario(int id)
        {
            try
            {
                var usuario = await _context.Usuarios.FindAsync(id);

                if (usuario == null)
                {
                    return ApiResult<Usuario>.Fail("Usuario no encontrado");
                }

                usuario.PasswordHash = ""; // No devolver password
                return ApiResult<Usuario>.Ok(usuario);
            }
            catch (Exception ex)
            {
                return ApiResult<Usuario>.Fail(ex.Message);
            }
        }

        // GET: api/Usuarios/PorRol/Votante
        [HttpGet("PorRol/{rol}")]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult<ApiResult<List<Usuario>>>> GetUsuariosPorRol(string rol)
        {
            try
            {
                var usuarios = await _context.Usuarios
                    .Where(u => u.Rol == rol)
                    .Select(u => new Usuario
                    {
                        Id = u.Id,
                        Cedula = u.Cedula,
                        Nombres = u.Nombres,
                        Apellidos = u.Apellidos,
                        Email = u.Email,
                        Rol = u.Rol,
                        Activo = u.Activo,
                        HaVotado = u.HaVotado,
                        FechaRegistro = u.FechaRegistro,
                        PasswordHash = ""
                    })
                    .ToListAsync();

                return ApiResult<List<Usuario>>.Ok(usuarios);
            }
            catch (Exception ex)
            {
                return ApiResult<List<Usuario>>.Fail(ex.Message);
            }
        }

        // PUT: api/Usuarios/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<ApiResult<Usuario>>> PutUsuario(int id, Usuario usuario)
        {
            if (id != usuario.Id)
            {
                return ApiResult<Usuario>.Fail("No coinciden los identificadores");
            }

            // Si se está actualizando el password, encriptarlo
            if (!string.IsNullOrEmpty(usuario.PasswordHash))
            {
                usuario.PasswordHash = _encriptacion.HashPassword(usuario.PasswordHash);
            }
            else
            {
                // Mantener el password actual
                var usuarioActual = await _context.Usuarios.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
                if (usuarioActual != null)
                {
                    usuario.PasswordHash = usuarioActual.PasswordHash;
                }
            }

            _context.Entry(usuario).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return ApiResult<Usuario>.Ok(null);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsuarioExists(id))
                {
                    return ApiResult<Usuario>.Fail("Usuario no encontrado");
                }
                throw;
            }
        }

        // DELETE: api/Usuarios/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult<ApiResult<Usuario>>> DeleteUsuario(int id)
        {
            try
            {
                var usuario = await _context.Usuarios.FindAsync(id);
                if (usuario == null)
                {
                    return ApiResult<Usuario>.Fail("Usuario no encontrado");
                }

                _context.Usuarios.Remove(usuario);
                await _context.SaveChangesAsync();

                return ApiResult<Usuario>.Ok(null);
            }
            catch (Exception ex)
            {
                return ApiResult<Usuario>.Fail(ex.Message);
            }
        }

        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.Id == id);
        }
    }
}