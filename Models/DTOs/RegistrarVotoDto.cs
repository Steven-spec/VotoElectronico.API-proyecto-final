using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models.DTOs
{
    public class RegistrarVotoDto
    {
        [Required]
        public int EleccionId { get; set; }

        [Required]
        public int CandidatoId { get; set; }

        [Required]
        public string TokenVotante { get; set; } // JWT del votante autenticado
    }

    public class VotoResponseDto
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public string? ComprobanteHash { get; set; } // Hash para que el votante verifique su voto
    }
}