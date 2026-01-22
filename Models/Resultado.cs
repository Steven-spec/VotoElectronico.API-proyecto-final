using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models
{
    public class Resultado
    {
        [Key]
        public int Id { get; set; }

        // FK
        public int EleccionId { get; set; }
        public int CandidatoId { get; set; }

        public int TotalVotos { get; set; }

        public decimal Porcentaje { get; set; }

        // Para método de adjudicación de escaños
        public int? EscanosAdjudicados { get; set; }

        public DateTime FechaCalculo { get; set; } = DateTime.Now;

        // Navegación
        public Eleccion? Eleccion { get; set; }
        public Candidato? Candidato { get; set; }
    }
}