using System.ComponentModel.DataAnnotations;

namespace VotoElectronico.API.Models.DTOs
{
    public class LoginDTO
    {
        [Required(ErrorMessage = "La cédula es obligatoria")]
        public string Cedula { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        public string Password { get; set; } = string.Empty;
    }
}