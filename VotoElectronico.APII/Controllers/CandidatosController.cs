using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VotoElectronico.APII.Data;
using Models;

namespace VotoElectronico.APII.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CandidatosController : ControllerBase
    {
        private readonly VotoElectronicoContext _context;

        public CandidatosController(VotoElectronicoContext context)
        {
            _context = context;
        }

        // GET: api/Candidatos
        [HttpGet]
        public async Task<ActionResult<ApiResult<List<Candidato>>>> GetCandidatos()
        {
            try
            {
                var candidatos = await _context.Candidatos
                    .Include(c => c.Usuario)
                    .Include(c => c.Partido)
                    .Include(c => c.Cargo)
                    .Include(c => c.Eleccion)
                    .ToListAsync();
                return ApiResult<List<Candidato>>.Ok(candidatos);
            }
            catch (Exception ex)
            {
                return ApiResult<List<Candidato>>.Fail(ex.Message);
            }
        }

        // GET: api/Candidatos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResult<Candidato>>> GetCandidato(int id)
        {
            try
            {
                var candidato = await _context.Candidatos
                    .Include(c => c.Usuario)
                    .Include(c => c.Partido)
                    .Include(c => c.Cargo)
                    .Include(c => c.Eleccion)
                    .Include(c => c.Votos)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (candidato == null)
                {
                    return ApiResult<Candidato>.Fail("Candidato no encontrado");
                }

                return ApiResult<Candidato>.Ok(candidato);
            }
            catch (Exception ex)
            {
                return ApiResult<Candidato>.Fail(ex.Message);
            }
        }

        // GET: api/Candidatos/PorCargo/5
        [HttpGet("PorCargo/{cargoId}")]
        public async Task<ActionResult<ApiResult<List<Candidato>>>> GetCandidatosPorCargo(int cargoId)
        {
            try
            {
                var candidatos = await _context.Candidatos
                    .Include(c => c.Usuario)
                    .Include(c => c.Partido)
                    .Include(c => c.Cargo)
                    .Include(c => c.Eleccion)
                    .Where(c => c.CargoId == cargoId)
                    .ToListAsync();

                return ApiResult<List<Candidato>>.Ok(candidatos);
            }
            catch (Exception ex)
            {
                return ApiResult<List<Candidato>>.Fail(ex.Message);
            }
        }

        // GET: api/Candidatos/PorEleccion/5
        [HttpGet("PorEleccion/{eleccionId}")]
        public async Task<ActionResult<ApiResult<List<Candidato>>>> GetCandidatosPorEleccion(int eleccionId)
        {
            try
            {
                var candidatos = await _context.Candidatos
                    .Include(c => c.Usuario)
                    .Include(c => c.Partido)
                    .Include(c => c.Cargo)
                    .Where(c => c.EleccionId == eleccionId)
                    .ToListAsync();

                return ApiResult<List<Candidato>>.Ok(candidatos);
            }
            catch (Exception ex)
            {
                return ApiResult<List<Candidato>>.Fail(ex.Message);
            }
        }

        // POST: api/Candidatos
        [HttpPost]
        public async Task<ActionResult<ApiResult<Candidato>>> PostCandidato(Candidato candidato)
        {
            try
            {
                // Validar que el usuario existe y es candidato
                var usuario = await _context.Usuarios.FindAsync(candidato.UsuarioId);
                if (usuario == null)
                {
                    return ApiResult<Candidato>.Fail("Usuario no encontrado");
                }

                if (usuario.Rol != "Candidato")
                {
                    return ApiResult<Candidato>.Fail("El usuario debe tener rol de Candidato");
                }

                // Validar que la elección existe
                var eleccion = await _context.Elecciones.FindAsync(candidato.EleccionId);
                if (eleccion == null)
                {
                    return ApiResult<Candidato>.Fail("Elección no encontrada");
                }

                // Validar que el partido existe
                var partido = await _context.Partidos.FindAsync(candidato.PartidoId);
                if (partido == null)
                {
                    return ApiResult<Candidato>.Fail("Partido no encontrado");
                }

                // Validar que el cargo existe
                var cargo = await _context.Cargos.FindAsync(candidato.CargoId);
                if (cargo == null)
                {
                    return ApiResult<Candidato>.Fail("Cargo no encontrado");
                }

                // Verificar que no esté ya registrado en esa elección
                var yaRegistrado = await _context.Candidatos
                    .AnyAsync(c => c.UsuarioId == candidato.UsuarioId && c.EleccionId == candidato.EleccionId);

                if (yaRegistrado)
                {
                    return ApiResult<Candidato>.Fail("El candidato ya está registrado en esta elección");
                }

                _context.Candidatos.Add(candidato);
                await _context.SaveChangesAsync();

                // Cargar las relaciones para devolver
                candidato.Usuario = usuario;
                candidato.Partido = partido;
                candidato.Cargo = cargo;
                candidato.Eleccion = eleccion;

                return ApiResult<Candidato>.Ok(candidato);
            }
            catch (Exception ex)
            {
                return ApiResult<Candidato>.Fail(ex.Message);
            }
        }

        // PUT: api/Candidatos/5
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResult<Candidato>>> PutCandidato(int id, Candidato candidato)
        {
            if (id != candidato.Id)
            {
                return ApiResult<Candidato>.Fail("No coinciden los identificadores");
            }

            _context.Entry(candidato).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return ApiResult<Candidato>.Ok(null);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Candidatos.Any(e => e.Id == id))
                {
                    return ApiResult<Candidato>.Fail("Candidato no encontrado");
                }
                throw;
            }
        }

        // DELETE: api/Candidatos/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResult<Candidato>>> DeleteCandidato(int id)
        {
            try
            {
                var candidato = await _context.Candidatos.FindAsync(id);
                if (candidato == null)
                {
                    return ApiResult<Candidato>.Fail("Candidato no encontrado");
                }

                _context.Candidatos.Remove(candidato);
                await _context.SaveChangesAsync();

                return ApiResult<Candidato>.Ok(null);
            }
            catch (Exception ex)
            {
                return ApiResult<Candidato>.Fail(ex.Message);
            }
        }
    }
}