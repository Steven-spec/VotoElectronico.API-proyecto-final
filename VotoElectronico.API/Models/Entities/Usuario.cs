using System.ComponentModel.DataAnnotations;

namespace VotoElectronico.API.Models.Entities
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Cedula { get; set; } = string.Empty;

        [Required]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        public string Rol { get; set; } = "VOTANTE";

        [Required]
        public string Password_Hash { get; set; } = string.Empty;

        public DateTime Creado_En { get; set; } = DateTime.UtcNow;
    }
}