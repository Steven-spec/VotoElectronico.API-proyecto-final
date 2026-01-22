using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models
{
    public class VotoEncriptado
    {
        [Key]
        public int Id { get; set; }

        // FK
        public int EleccionId { get; set; }
        public int CandidatoId { get; set; }

        // Hash anónimo del votante (SHA256 de cedula + salt + timestamp)
        [Required]
        [StringLength(255)]
        public string HashVotante { get; set; }

        // Voto encriptado (el candidato seleccionado encriptado)
        [Required]
        [StringLength(500)]
        public string VotoEncriptadoData { get; set; }

        // Timestamp del voto
        public DateTime FechaHora { get; set; } = DateTime.Now;

        // Hash del voto completo para inmutabilidad
        [Required]
        [StringLength(255)]
        public string HashVoto { get; set; }

        // IP y datos de auditoría (sin identificar al votante)
        [StringLength(50)]
        public string? IpAddress { get; set; }

        [StringLength(200)]
        public string? UserAgent { get; set; }

        // Navegación
        public Eleccion? Eleccion { get; set; }
        public Candidato? Candidato { get; set; }
    }
}