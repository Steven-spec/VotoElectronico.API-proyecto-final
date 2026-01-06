using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VotoElectronico.API.Models.Entities
{
    public class Voto
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Eleccion")]
        public int EleccionId { get; set; }
        public Eleccion? Eleccion { get; set; }

        [Required]
        [ForeignKey("Candidato")]
        public int CandidatoId { get; set; }
        public Candidato? Candidato { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        public DateTime Fecha { get; set; } = DateTime.UtcNow;
    }
}