using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using VotoElectronico.APII.Data;

namespace VotoElectronico.APII.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EleccionesController : ControllerBase
    {
        private readonly VotoElectronicoContext _context;

        public EleccionesController(VotoElectronicoContext context)
        {
            _context = context;
        }

        // GET: api/Elecciones
        [HttpGet]
        public async Task<ActionResult<ApiResult<List<Eleccion>>>> GetElecciones()
        {
            try
            {
                var elecciones = await _context.Elecciones
                    .Include(e => e.Candidatos)
                    .ToListAsync();
                return ApiResult<List<Eleccion>>.Ok(elecciones);
            }
            catch (Exception ex)
            {
                return ApiResult<List<Eleccion>>.Fail(ex.Message);
            }
        }

        // GET: api/Elecciones/Activas
        [HttpGet("Activas")]
        public async Task<ActionResult<ApiResult<List<Eleccion>>>> GetEleccionesActivas()
        {
            try
            {
                var ahora = DateTime.Now;
                var elecciones = await _context.Elecciones
                    .Include(e => e.Candidatos)
                        .ThenInclude(c => c.Usuario)
                    .Include(e => e.Candidatos)
                        .ThenInclude(c => c.Partido)
                    .Include(e => e.Candidatos)
                        .ThenInclude(c => c.Cargo)
                    .Where(e => e.FechaInicio <= ahora && e.FechaFin >= ahora && e.Estado == "EnCurso")
                    .ToListAsync();

                return ApiResult<List<Eleccion>>.Ok(elecciones);
            }
            catch (Exception ex)
            {
                return ApiResult<List<Eleccion>>.Fail(ex.Message);
            }
        }

        // GET: api/Elecciones/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResult<Eleccion>>> GetEleccion(int id)
        {
            try
            {
                var eleccion = await _context.Elecciones
                    .Include(e => e.Candidatos)
                        .ThenInclude(c => c.Usuario)
                    .Include(e => e.Candidatos)
                        .ThenInclude(c => c.Partido)
                    .Include(e => e.Candidatos)
                        .ThenInclude(c => c.Cargo)
                    .FirstOrDefaultAsync(e => e.Id == id);

                if (eleccion == null)
                {
                    return ApiResult<Eleccion>.Fail("Elección no encontrada");
                }

                return ApiResult<Eleccion>.Ok(eleccion);
            }
            catch (Exception ex)
            {
                return ApiResult<Eleccion>.Fail(ex.Message);
            }
        }

        // POST: api/Elecciones
        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult<ApiResult<Eleccion>>> PostEleccion(Eleccion eleccion)
        {
            try
            {
                eleccion.FechaCreacion = DateTime.Now;
                eleccion.Estado = "Programada";

                _context.Elecciones.Add(eleccion);
                await _context.SaveChangesAsync();

                return ApiResult<Eleccion>.Ok(eleccion);
            }
            catch (Exception ex)
            {
                return ApiResult<Eleccion>.Fail(ex.Message);
            }
        }

        // PUT: api/Elecciones/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult<ApiResult<Eleccion>>> PutEleccion(int id, Eleccion eleccion)
        {
            if (id != eleccion.Id)
            {
                return ApiResult<Eleccion>.Fail("No coinciden los identificadores");
            }

            _context.Entry(eleccion).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return ApiResult<Eleccion>.Ok(null);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EleccionExists(id))
                {
                    return ApiResult<Eleccion>.Fail("Elección no encontrada");
                }
                throw;
            }
        }

        // POST: api/Elecciones/5/IniciarVotacion
        [HttpPost("{id}/IniciarVotacion")]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult<ApiResult<bool>>> IniciarVotacion(int id)
        {
            try
            {
                var eleccion = await _context.Elecciones.FindAsync(id);
                if (eleccion == null)
                {
                    return ApiResult<bool>.Fail("Elección no encontrada");
                }

                eleccion.Estado = "EnCurso";
                await _context.SaveChangesAsync();

                return ApiResult<bool>.Ok(true);
            }
            catch (Exception ex)
            {
                return ApiResult<bool>.Fail(ex.Message);
            }
        }

        // POST: api/Elecciones/5/FinalizarVotacion
        [HttpPost("{id}/FinalizarVotacion")]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult<ApiResult<bool>>> FinalizarVotacion(int id)
        {
            try
            {
                var eleccion = await _context.Elecciones.FindAsync(id);
                if (eleccion == null)
                {
                    return ApiResult<bool>.Fail("Elección no encontrada");
                }

                eleccion.Estado = "Finalizada";
                eleccion.FechaFin = DateTime.Now;
                await _context.SaveChangesAsync();

                return ApiResult<bool>.Ok(true);
            }
            catch (Exception ex)
            {
                return ApiResult<bool>.Fail(ex.Message);
            }
        }

        // DELETE: api/Elecciones/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult<ApiResult<Eleccion>>> DeleteEleccion(int id)
        {
            try
            {
                var eleccion = await _context.Elecciones.FindAsync(id);
                if (eleccion == null)
                {
                    return ApiResult<Eleccion>.Fail("Elección no encontrada");
                }

                _context.Elecciones.Remove(eleccion);
                await _context.SaveChangesAsync();

                return ApiResult<Eleccion>.Ok(null);
            }
            catch (Exception ex)
            {
                return ApiResult<Eleccion>.Fail(ex.Message);
            }
        }

        private bool EleccionExists(int id)
        {
            return _context.Elecciones.Any(e => e.Id == id);
        }
    }
}