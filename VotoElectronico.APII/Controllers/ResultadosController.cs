using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using VotoElectronico.APII.Data;


namespace VotoElectronico.APII.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResultadosController : ControllerBase
    {
        private readonly VotoElectronicoContext _context;

        public ResultadosController(VotoElectronicoContext context)
        {
            _context = context;
        }

        // GET: api/Resultados/Eleccion/5
        [HttpGet("Eleccion/{eleccionId}")]
        public async Task<ActionResult<ApiResult<List<object>>>> GetResultadosEleccion(int eleccionId)
        {
            try
            {
                var eleccion = await _context.Elecciones.FindAsync(eleccionId);

                if (eleccion == null)
                {
                    return ApiResult<List<object>>.Fail("Elección no encontrada");
                }

                // Solo mostrar resultados si la elección ha finalizado o si están públicos
                if (eleccion.Estado != "Finalizada" && !eleccion.ResultadosPublicos)
                {
                    return ApiResult<List<object>>.Fail("Los resultados aún no están disponibles");
                }

                var resultados = await _context.VotosEncriptados
                    .Where(v => v.EleccionId == eleccionId)
                    .Include(v => v.Candidato)
                        .ThenInclude(c => c.Usuario)
                    .Include(v => v.Candidato)
                        .ThenInclude(c => c.Partido)
                    .Include(v => v.Candidato)
                        .ThenInclude(c => c.Cargo)
                    .GroupBy(v => new
                    {
                        v.CandidatoId,
                        v.Candidato.Usuario.Nombres,
                        v.Candidato.Usuario.Apellidos,
                        PartidoNombre = v.Candidato.Partido.Nombre,
                        CargoNombre = v.Candidato.Cargo.Nombre,
                        v.Candidato.NumeroCandidato
                    })
                    .Select(g => new
                    {
                        CandidatoId = g.Key.CandidatoId,
                        NombreCandidato = g.Key.Nombres + " " + g.Key.Apellidos,
                        Partido = g.Key.PartidoNombre,
                        Cargo = g.Key.CargoNombre,
                        NumeroCandidato = g.Key.NumeroCandidato,
                        TotalVotos = g.Count()
                    })
                    .OrderByDescending(r => r.TotalVotos)
                    .ToListAsync();

                // Calcular porcentajes
                var totalVotos = resultados.Sum(r => r.TotalVotos);
                var resultadosConPorcentaje = resultados.Select(r => new
                {
                    r.CandidatoId,
                    r.NombreCandidato,
                    r.Partido,
                    r.Cargo,
                    r.NumeroCandidato,
                    r.TotalVotos,
                    Porcentaje = totalVotos > 0 ? Math.Round((decimal)r.TotalVotos / totalVotos * 100, 2) : 0
                }).ToList<object>();

                return ApiResult<List<object>>.Ok(resultadosConPorcentaje);
            }
            catch (Exception ex)
            {
                return ApiResult<List<object>>.Fail(ex.Message);
            }
        }

        // POST: api/Resultados/CalcularResultados/5
        [HttpPost("CalcularResultados/{eleccionId}")]
        public async Task<ActionResult<ApiResult<bool>>> CalcularResultados(int eleccionId)
        {
            try
            {
                var eleccion = await _context.Elecciones.FindAsync(eleccionId);

                if (eleccion == null)
                {
                    return ApiResult<bool>.Fail("Elección no encontrada");
                }

                // Eliminar resultados anteriores
                var resultadosAnteriores = await _context.Resultados
                    .Where(r => r.EleccionId == eleccionId)
                    .ToListAsync();
                _context.Resultados.RemoveRange(resultadosAnteriores);

                // Calcular nuevos resultados
                var votos = await _context.VotosEncriptados
                    .Where(v => v.EleccionId == eleccionId)
                    .GroupBy(v => v.CandidatoId)
                    .Select(g => new { CandidatoId = g.Key, Total = g.Count() })
                    .ToListAsync();

                var totalVotos = votos.Sum(v => v.Total);

                foreach (var voto in votos)
                {
                    var resultado = new Resultado
                    {
                        EleccionId = eleccionId,
                        CandidatoId = voto.CandidatoId,
                        TotalVotos = voto.Total,
                        Porcentaje = totalVotos > 0 ? Math.Round((decimal)voto.Total / totalVotos * 100, 2) : 0,
                        FechaCalculo = DateTime.Now
                    };

                    _context.Resultados.Add(resultado);
                }

                // Aplicar método de adjudicación de escaños si aplica
                if (eleccion.MetodoAdjudicacion == "Webster" || eleccion.MetodoAdjudicacion == "DHundt")
                {
                    // TODO: Implementar métodos Webster y D'Hundt
                    // Por ahora solo calculamos votos
                }

                await _context.SaveChangesAsync();

                return ApiResult<bool>.Ok(true);
            }
            catch (Exception ex)
            {
                return ApiResult<bool>.Fail(ex.Message);
            }
        }

        // GET: api/Resultados/Tiempo-Real/5
        [HttpGet("Tiempo-Real/{eleccionId}")]
        public async Task<ActionResult<ApiResult<object>>> GetResultadosTiempoReal(int eleccionId)
        {
            try
            {
                var totalVotos = await _context.VotosEncriptados
                    .CountAsync(v => v.EleccionId == eleccionId);

                var totalVotantes = await _context.Usuarios
                    .CountAsync(u => u.Rol == "Votante");

                var porcentajeParticipacion = totalVotantes > 0
                    ? Math.Round((decimal)totalVotos / totalVotantes * 100, 2)
                    : 0;

                var resultado = new
                {
                    TotalVotos = totalVotos,
                    TotalVotantes = totalVotantes,
                    PorcentajeParticipacion = porcentajeParticipacion,
                    UltimaActualizacion = DateTime.Now
                };

                return ApiResult<object>.Ok(resultado);
            }
            catch (Exception ex)
            {
                return ApiResult<object>.Fail(ex.Message);
            }
        }
    }
}