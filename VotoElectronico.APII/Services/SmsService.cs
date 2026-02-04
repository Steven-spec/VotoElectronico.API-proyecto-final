namespace VotoElectronico.APII.Services
{
    public interface ISmsService
    {
        Task<bool> EnviarConfirmacionVoto(string telefono, string nombreCompleto);
    }

    public class SmsService : ISmsService
    {
        private readonly ILogger<SmsService> _logger;

        public SmsService(ILogger<SmsService> logger)
        {
            _logger = logger;
        }

        public async Task<bool> EnviarConfirmacionVoto(string telefono, string nombreCompleto)
        {
            try
            {
                // SIMULACIÓN - En producción usar Twilio, AWS SNS, etc.
                _logger.LogInformation($"========================================");
                _logger.LogInformation($"SMS DE CONFIRMACIÓN");
                _logger.LogInformation($"Para: {telefono}");
                _logger.LogInformation($"Mensaje:");
                _logger.LogInformation($"{nombreCompleto}, su voto ha sido registrado exitosamente.");
                _logger.LogInformation($"Gracias por participar. Sistema Voto Electrónico.");
                _logger.LogInformation($"========================================");

                await Task.Delay(100);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al enviar SMS: {ex.Message}");
                return false;
            }
        }
    }
}