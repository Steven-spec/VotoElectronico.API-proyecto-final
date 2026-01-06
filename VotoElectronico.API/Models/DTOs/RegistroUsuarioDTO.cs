using System.ComponentModel.DataAnnotations;

namespace VotoElectronico.API.Models.DTOs
{
    public class RegistroUsuarioDTO
    {
        [Required(ErrorMessage = "La cédula es obligatoria")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "La cédula debe tener 10 dígitos")]
        public string Cedula { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
        public string Password { get; set; } = string.Empty;

        public string Rol { get; set; } = "VOTANTE";
    }
}