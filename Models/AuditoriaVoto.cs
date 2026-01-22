using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models
{
    public class AuditoriaVoto
    {
        [Key]
        public int Id { get; set; }

        public int EleccionId { get; set; }

        // Hash del votante para verificar duplicados SIN revelar identidad
        [Required]
        [StringLength(255)]
        public string HashVotante { get; set; }

        public DateTime FechaHora { get; set; } = DateTime.Now;

        [StringLength(50)]
        public string? IpAddress { get; set; }

        [StringLength(20)]
        public string Accion { get; set; } // "VotoRegistrado", "IntentoDobleVoto"

        public bool Exitoso { get; set; }

        [StringLength(500)]
        public string? Observaciones { get; set; }

        // Navegación
        public Eleccion? Eleccion { get; set; }
    }
}