using System.Security.Cryptography;
using System.Text;

namespace VotoElectronico.APII.Services
{
    public interface IEncriptacionService
    {
        string GenerarHash(string texto);
        string GenerarHashVotante(string cedula, int eleccionId);
        string EncriptarVoto(int candidatoId);
        string GenerarHashVoto(string hashVotante, string votoEncriptado, DateTime timestamp);
        string HashPassword(string password);
    }

    public class EncriptacionService : IEncriptacionService
    {
        private readonly IConfiguration _configuration;

        public EncriptacionService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerarHash(string texto)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(texto);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        public string GenerarHashVotante(string cedula, int eleccionId)
        {
            // Hash anónimo: SHA256(cedula + eleccionId + salt secreto)
            var salt = _configuration["Security:VotanteSalt"] ?? "DefaultSalt2024";
            var texto = $"{cedula}|{eleccionId}|{salt}";
            return GenerarHash(texto);
        }

        public string EncriptarVoto(int candidatoId)
        {
            // Encriptar el ID del candidato con AES
            var key = _configuration["Security:EncryptionKey"] ?? "DefaultKey2024SecureVoting!!!";

            using (var aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key.PadRight(32).Substring(0, 32));
                aes.GenerateIV();

                using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                using (var ms = new MemoryStream())
                {
                    ms.Write(aes.IV, 0, aes.IV.Length);

                    using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    using (var sw = new StreamWriter(cs))
                    {
                        sw.Write(candidatoId.ToString());
                    }

                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        public string GenerarHashVoto(string hashVotante, string votoEncriptado, DateTime timestamp)
        {
            // Hash inmutable del voto completo para auditoría
            var texto = $"{hashVotante}|{votoEncriptado}|{timestamp:yyyyMMddHHmmss}";
            return GenerarHash(texto);
        }

        public string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var salt = _configuration["Security:PasswordSalt"] ?? "PasswordSalt2024";
                var bytes = Encoding.UTF8.GetBytes(password + salt);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }
    }
}