using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VotoElectronico.API.Models.Entities
{
    public class Candidato
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Nombre { get; set; } = string.Empty;

        public string Partido { get; set; } = string.Empty;

        public string Foto { get; set; } = string.Empty;

        [Required]
        [ForeignKey("Eleccion")]
        public int EleccionId { get; set; }

        public Eleccion? Eleccion { get; set; }
    }
}