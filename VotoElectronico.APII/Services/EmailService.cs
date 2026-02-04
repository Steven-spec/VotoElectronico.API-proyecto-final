namespace VotoElectronico.APII.Services
{
    public interface IEmailService
    {
        Task<bool> EnviarCodigoVerificacion(string email, string codigo, string nombreCompleto);
        Task<bool> EnviarConfirmacionVoto(string email, string nombreCompleto, string comprobante);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<bool> EnviarCodigoVerificacion(string email, string codigo, string nombreCompleto)
        {
            try
            {
                // SIMULACIÓN - En producción usar un servicio real como SendGrid, AWS SES, etc.
                _logger.LogInformation($"========================================");
                _logger.LogInformation($"EMAIL DE VERIFICACIÓN");
                _logger.LogInformation($"Para: {email}");
                _logger.LogInformation($"Destinatario: {nombreCompleto}");
                _logger.LogInformation($"Asunto: Código de Verificación - Voto Electrónico");
                _logger.LogInformation($"");
                _logger.LogInformation($"Hola {nombreCompleto},");
                _logger.LogInformation($"");
                _logger.LogInformation($"Su código de verificación es: {codigo}");
                _logger.LogInformation($"");
                _logger.LogInformation($"Este código es válido por 10 minutos.");
                _logger.LogInformation($"No comparta este código con nadie.");
                _logger.LogInformation($"========================================");

                // Simular envío
                await Task.Delay(100);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al enviar email: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> EnviarConfirmacionVoto(string email, string nombreCompleto, string comprobante)
        {
            try
            {
                _logger.LogInformation($"========================================");
                _logger.LogInformation($"EMAIL DE CONFIRMACIÓN DE VOTO");
                _logger.LogInformation($"Para: {email}");
                _logger.LogInformation($"Destinatario: {nombreCompleto}");
                _logger.LogInformation($"Asunto: Confirmación de Voto - Voto Electrónico");
                _logger.LogInformation($"");
                _logger.LogInformation($"Hola {nombreCompleto},");
                _logger.LogInformation($"");
                _logger.LogInformation($"Su voto ha sido registrado exitosamente.");
                _logger.LogInformation($"Comprobante: {comprobante}");
                _logger.LogInformation($"");
                _logger.LogInformation($"Gracias por participar en este proceso electoral.");
                _logger.LogInformation($"========================================");

                await Task.Delay(100);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al enviar confirmación: {ex.Message}");
                return false;
            }
        }
    }
}