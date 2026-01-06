using System.ComponentModel.DataAnnotations;

namespace VotoElectronico.API.Models.Entities
{
    public class Eleccion
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        public DateTime FechaInicio { get; set; }

        [Required]
        public DateTime FechaFin { get; set; }

        [Required]
        public string Estado { get; set; } = "ABIERTA";

        public ICollection<Voto> Votos { get; set; } = new List<Voto>();
        public ICollection<Candidato> Candidatos { get; set; } = new List<Candidato>();
    }
}