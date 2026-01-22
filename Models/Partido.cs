using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models
{
    public class Partido
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; }

        [StringLength(50)]
        public string? Siglas { get; set; }

        [StringLength(200)]
        public string? Logo { get; set; }

        [StringLength(20)]
        public string? Color { get; set; }

        // Navegación
        public List<Candidato>? Candidatos { get; set; }
    }
}