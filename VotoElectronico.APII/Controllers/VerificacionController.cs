using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VotoElectronico.APII.Data;
using VotoElectronico.APII.Services;
using Models;
using System.Text;

namespace VotoElectronico.APII.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VerificacionController : ControllerBase
    {
        private readonly VotoElectronicoContext _context;
        private readonly IEmailService _emailService;

        public VerificacionController(VotoElectronicoContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        // POST: api/Verificacion/GenerarCodigo
        [HttpPost("GenerarCodigo")]
        public async Task<ActionResult<ApiResult<string>>> GenerarCodigo([FromBody] string cedula)
        {
            try
            {
                // Verificar que el votante existe
                var votante = await _context.Usuarios
                    .FirstOrDefaultAsync(u => u.Cedula == cedula && u.Rol == "Votante");

                if (votante == null)
                {
                    return ApiResult<string>.Fail("Cédula no registrada o no es votante");
                }

                if (votante.HaVotado)
                {
                    return ApiResult<string>.Fail("Esta cédula ya ha emitido su voto");
                }

                // Generar código alfanumérico de 6 caracteres
                string codigo = GenerarCodigoAlfanumerico(6);

                // Guardar código en base de datos
                var codigoVerif = new CodigoVerificacion
                {
                    Cedula = cedula,
                    Codigo = codigo,
                    FechaCreacion = DateTime.Now,
                    FechaExpiracion = DateTime.Now.AddMinutes(10),
                    Usado = false
                };

                _context.CodigosVerificacion.Add(codigoVerif);
                await _context.SaveChangesAsync();

                // Enviar código por email
                await _emailService.EnviarCodigoVerificacion(
                    votante.Email,
                    codigo,
                    $"{votante.Nombres} {votante.Apellidos}"
                );

                return ApiResult<string>.Ok("Código enviado a su correo electrónico");
            }
            catch (Exception ex)
            {
                return ApiResult<string>.Fail(ex.Message);
            }
        }

        // POST: api/Verificacion/ValidarCodigo
        [HttpPost("ValidarCodigo")]
        public async Task<ActionResult<ApiResult<string>>> ValidarCodigo([FromBody] ValidarCodigoDto dto)
        {
            try
            {
                var codigo = await _context.CodigosVerificacion
                    .Where(c => c.Cedula == dto.Cedula && c.Codigo == dto.Codigo)
                    .OrderByDescending(c => c.FechaCreacion)
                    .FirstOrDefaultAsync();

                if (codigo == null)
                {
                    return ApiResult<string>.Fail("Código incorrecto");
                }

                if (codigo.Usado)
                {
                    return ApiResult<string>.Fail("Código ya ha sido utilizado");
                }

                if (codigo.FechaExpiracion < DateTime.Now)
                {
                    return ApiResult<string>.Fail("Código expirado. Solicite uno nuevo");
                }

                // Marcar como usado
                codigo.Usado = true;
                await _context.SaveChangesAsync();

                // Generar token temporal (podrías usar JWT aquí también)
                return ApiResult<string>.Ok($"VERIFIED_{dto.Cedula}_{DateTime.Now.Ticks}");
            }
            catch (Exception ex)
            {
                return ApiResult<string>.Fail(ex.Message);
            }
        }

        private string GenerarCodigoAlfanumerico(int longitud)
        {
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789"; // Sin letras/números confusos
            var random = new Random();
            var codigo = new StringBuilder();

            for (int i = 0; i < longitud; i++)
            {
                codigo.Append(chars[random.Next(chars.Length)]);
            }

            return codigo.ToString();
        }
    }

    public class ValidarCodigoDto
    {
        public string Cedula { get; set; }
        public string Codigo { get; set; }
    }
}