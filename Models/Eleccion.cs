using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models
{
    public class Eleccion
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Nombre { get; set; }

        [StringLength(1000)]
        public string? Descripcion { get; set; }

        [Required]
        public DateTime FechaInicio { get; set; }

        [Required]
        public DateTime FechaFin { get; set; }

        [Required]
        [StringLength(20)]
        public string TipoEleccion { get; set; } // "Nominal", "Plancha"

        [Required]
        [StringLength(50)]
        public string MetodoAdjudicacion { get; set; } // "Webster", "DHundt", "Simple"

        [StringLength(20)]
        public string Estado { get; set; } = "Programada"; // "Programada", "EnCurso", "Finalizada"

        public bool ResultadosPublicos { get; set; } = false;

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        // Navegación
        public List<Candidato>? Candidatos { get; set; }
        public List<VotoEncriptado>? Votos { get; set; }
        public List<Resultado>? Resultados { get; set; }
    }
}