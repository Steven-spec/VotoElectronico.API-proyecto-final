using System.ComponentModel.DataAnnotations;

namespace VotoElectronico.API.Models.DTOs
{
    public class VotoDTO
    {
        [Required(ErrorMessage = "El ID de la elección es obligatorio")]
        public int EleccionId { get; set; }

        [Required(ErrorMessage = "El ID del candidato es obligatorio")]
        public int CandidatoId { get; set; }
    }
}