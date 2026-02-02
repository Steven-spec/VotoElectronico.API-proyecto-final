using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class CodigoVerificacion
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        public string Cedula { get; set; }

        [Required]
        [StringLength(10)]
        public string Codigo { get; set; } // Código alfanumérico de 6 caracteres

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public DateTime FechaExpiracion { get; set; } // 10 minutos después

        public bool Usado { get; set; } = false;

        public int? EleccionId { get; set; }
    }
}