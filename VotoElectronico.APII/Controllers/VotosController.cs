using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.DTOs;
using System.Security.Claims;
using VotoElectronico.APII.Data;
using VotoElectronico.APII.Services;

namespace VotoElectronico.APII.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VotosController : ControllerBase
    {
        private readonly VotoElectronicoContext _context;
        private readonly IEncriptacionService _encriptacion;

        public VotosController(VotoElectronicoContext context, IEncriptacionService encriptacion)
        {
            _context = context;
            _encriptacion = encriptacion;
        }

        // POST: api/Votos/RegistrarVoto
        [HttpPost("RegistrarVoto")]
        [Authorize(Roles = "Votante")]
        public async Task<ActionResult<VotoResponseDto>> RegistrarVoto(RegistrarVotoDto votoDto)
        {
            try
            {
                // Obtener usuario del token JWT
                var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var cedula = User.FindFirst("Cedula")?.Value ?? "";

                var usuario = await _context.Usuarios.FindAsync(usuarioId);
                if (usuario == null)
                {
                    return new VotoResponseDto { Success = false, Message = "Usuario no encontrado" };
                }

                // Verificar que no haya votado ya
                if (usuario.HaVotado)
                {
                    return new VotoResponseDto { Success = false, Message = "Ya ha emitido su voto" };
                }

                // Verificar que la elección esté activa
                var eleccion = await _context.Elecciones.FindAsync(votoDto.EleccionId);
                if (eleccion == null || eleccion.Estado != "EnCurso")
                {
                    return new VotoResponseDto { Success = false, Message = "La elección no está activa" };
                }

                // Verificar que el candidato exista y pertenezca a la elección
                var candidato = await _context.Candidatos
                    .FirstOrDefaultAsync(c => c.Id == votoDto.CandidatoId && c.EleccionId == votoDto.EleccionId);

                if (candidato == null)
                {
                    return new VotoResponseDto { Success = false, Message = "Candidato no válido" };
                }

                // Generar hash anónimo del votante
                var hashVotante = _encriptacion.GenerarHashVotante(cedula, votoDto.EleccionId);

                // Verificar duplicados por hash (aunque ya verificamos usuario.HaVotado)
                var yaVoto = await _context.VotosEncriptados
                    .AnyAsync(v => v.HashVotante == hashVotante && v.EleccionId == votoDto.EleccionId);

                if (yaVoto)
                {
                    return new VotoResponseDto { Success = false, Message = "Intento de voto duplicado detectado" };
                }

                // Encriptar el voto
                var votoEncriptado = _encriptacion.EncriptarVoto(votoDto.CandidatoId);
                var timestamp = DateTime.Now;

                // Generar hash del voto completo para inmutabilidad
                var hashVoto = _encriptacion.GenerarHashVoto(hashVotante, votoEncriptado, timestamp);

                // Registrar el voto encriptado
                var voto = new VotoEncriptado
                {
                    EleccionId = votoDto.EleccionId,
                    CandidatoId = votoDto.CandidatoId,
                    HashVotante = hashVotante,
                    VotoEncriptadoData = votoEncriptado,
                    HashVoto = hashVoto,
                    FechaHora = timestamp,
                    IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                    UserAgent = Request.Headers["User-Agent"].ToString()
                };

                _context.VotosEncriptados.Add(voto);

                // Actualizar estado del votante
                usuario.HaVotado = true;
                usuario.FechaVoto = timestamp;

                // Registrar auditoría
                var auditoria = new AuditoriaVoto
                {
                    EleccionId = votoDto.EleccionId,
                    HashVotante = hashVotante,
                    FechaHora = timestamp,
                    IpAddress = voto.IpAddress,
                    Accion = "VotoRegistrado",
                    Exitoso = true,
                    Observaciones = "Voto registrado correctamente"
                };

                _context.AuditoriasVotos.Add(auditoria);

                await _context.SaveChangesAsync();

                return new VotoResponseDto
                {
                    Success = true,
                    Message = "Voto registrado exitosamente",
                    ComprobanteHash = hashVoto // El votante puede usar esto para verificar su voto
                };
            }
            catch (Exception ex)
            {
                return new VotoResponseDto
                {
                    Success = false,
                    Message = $"Error al registrar voto: {ex.Message}"
                };
            }
        }

        // GET: api/Votos/VerificarVoto/{hash}
        [HttpGet("VerificarVoto/{hash}")]
        public async Task<ActionResult<ApiResult<bool>>> VerificarVoto(string hash)
        {
            try
            {
                var existe = await _context.VotosEncriptados
                    .AnyAsync(v => v.HashVoto == hash);

                return ApiResult<bool>.Ok(existe);
            }
            catch (Exception ex)
            {
                return ApiResult<bool>.Fail(ex.Message);
            }
        }

        // GET: api/Votos/TotalVotos/{eleccionId}
        [HttpGet("TotalVotos/{eleccionId}")]
        public async Task<ActionResult<ApiResult<int>>> GetTotalVotos(int eleccionId)
        {
            try
            {
                var total = await _context.VotosEncriptados
                    .CountAsync(v => v.EleccionId == eleccionId);

                return ApiResult<int>.Ok(total);
            }
            catch (Exception ex)
            {
                return ApiResult<int>.Fail(ex.Message);
            }
        }
    }
}