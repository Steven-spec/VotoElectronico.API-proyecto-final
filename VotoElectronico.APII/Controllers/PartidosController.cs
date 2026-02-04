using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VotoElectronico.APII.Data;
using Models;

namespace VotoElectronico.APII.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PartidosController : ControllerBase
    {
        private readonly VotoElectronicoContext _context;

        public PartidosController(VotoElectronicoContext context)
        {
            _context = context;
        }

        // GET: api/Partidos
        [HttpGet]
        public async Task<ActionResult<ApiResult<List<Partido>>>> GetPartidos()
        {
            try
            {
                var partidos = await _context.Partidos.ToListAsync();
                return ApiResult<List<Partido>>.Ok(partidos);
            }
            catch (Exception ex)
            {
                return ApiResult<List<Partido>>.Fail(ex.Message);
            }
        }

        // GET: api/Partidos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResult<Partido>>> GetPartido(int id)
        {
            try
            {
                var partido = await _context.Partidos
                    .Include(p => p.Candidatos)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (partido == null)
                {
                    return ApiResult<Partido>.Fail("Partido no encontrado");
                }

                return ApiResult<Partido>.Ok(partido);
            }
            catch (Exception ex)
            {
                return ApiResult<Partido>.Fail(ex.Message);
            }
        }

        // POST: api/Partidos
        [HttpPost]
        public async Task<ActionResult<ApiResult<Partido>>> PostPartido(Partido partido)
        {
            try
            {
                _context.Partidos.Add(partido);
                await _context.SaveChangesAsync();

                return ApiResult<Partido>.Ok(partido);
            }
            catch (Exception ex)
            {
                return ApiResult<Partido>.Fail(ex.Message);
            }
        }

        // PUT: api/Partidos/5
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResult<Partido>>> PutPartido(int id, Partido partido)
        {
            if (id != partido.Id)
            {
                return ApiResult<Partido>.Fail("No coinciden los identificadores");
            }

            _context.Entry(partido).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return ApiResult<Partido>.Ok(null);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Partidos.Any(e => e.Id == id))
                {
                    return ApiResult<Partido>.Fail("Partido no encontrado");
                }
                throw;
            }
        }

        // DELETE: api/Partidos/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResult<Partido>>> DeletePartido(int id)
        {
            try
            {
                var partido = await _context.Partidos.FindAsync(id);
                if (partido == null)
                {
                    return ApiResult<Partido>.Fail("Partido no encontrado");
                }

                _context.Partidos.Remove(partido);
                await _context.SaveChangesAsync();

                return ApiResult<Partido>.Ok(null);
            }
            catch (Exception ex)
            {
                return ApiResult<Partido>.Fail(ex.Message);
            }
        }
    }
}