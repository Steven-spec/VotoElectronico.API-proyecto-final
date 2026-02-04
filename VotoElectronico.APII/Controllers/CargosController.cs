using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VotoElectronico.APII.Data;
using Models;

namespace VotoElectronico.APII.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CargosController : ControllerBase
    {
        private readonly VotoElectronicoContext _context;

        public CargosController(VotoElectronicoContext context)
        {
            _context = context;
        }

        // GET: api/Cargos
        [HttpGet]
        public async Task<ActionResult<ApiResult<List<Cargo>>>> GetCargos()
        {
            try
            {
                var cargos = await _context.Cargos.ToListAsync();
                return ApiResult<List<Cargo>>.Ok(cargos);
            }
            catch (Exception ex)
            {
                return ApiResult<List<Cargo>>.Fail(ex.Message);
            }
        }

        // GET: api/Cargos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResult<Cargo>>> GetCargo(int id)
        {
            try
            {
                var cargo = await _context.Cargos
                    .Include(c => c.Candidatos)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (cargo == null)
                {
                    return ApiResult<Cargo>.Fail("Cargo no encontrado");
                }

                return ApiResult<Cargo>.Ok(cargo);
            }
            catch (Exception ex)
            {
                return ApiResult<Cargo>.Fail(ex.Message);
            }
        }

        // POST: api/Cargos
        [HttpPost]
        public async Task<ActionResult<ApiResult<Cargo>>> PostCargo(Cargo cargo)
        {
            try
            {
                _context.Cargos.Add(cargo);
                await _context.SaveChangesAsync();

                return ApiResult<Cargo>.Ok(cargo);
            }
            catch (Exception ex)
            {
                return ApiResult<Cargo>.Fail(ex.Message);
            }
        }

        // PUT: api/Cargos/5
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResult<Cargo>>> PutCargo(int id, Cargo cargo)
        {
            if (id != cargo.Id)
            {
                return ApiResult<Cargo>.Fail("No coinciden los identificadores");
            }

            _context.Entry(cargo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return ApiResult<Cargo>.Ok(null);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Cargos.Any(e => e.Id == id))
                {
                    return ApiResult<Cargo>.Fail("Cargo no encontrado");
                }
                throw;
            }
        }

        // DELETE: api/Cargos/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResult<Cargo>>> DeleteCargo(int id)
        {
            try
            {
                var cargo = await _context.Cargos.FindAsync(id);
                if (cargo == null)
                {
                    return ApiResult<Cargo>.Fail("Cargo no encontrado");
                }

                _context.Cargos.Remove(cargo);
                await _context.SaveChangesAsync();

                return ApiResult<Cargo>.Ok(null);
            }
            catch (Exception ex)
            {
                return ApiResult<Cargo>.Fail(ex.Message);
            }
        }
    }
}