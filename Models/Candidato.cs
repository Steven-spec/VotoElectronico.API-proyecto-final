using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models
{
    public class Candidato
    {
        [Key]
        public int Id { get; set; }

        // FK a Usuario
        public int UsuarioId { get; set; }

        // FK a Elección
        public int EleccionId { get; set; }

        // FK a Partido
        public int PartidoId { get; set; }

        // FK a Cargo
        public int CargoId { get; set; }

        [StringLength(500)]
        public string? Propuestas { get; set; }

        [StringLength(200)]
        public string? Foto { get; set; }

        public int NumeroCandidato { get; set; } // Número de lista

        public bool Activo { get; set; } = true;

        // Navegación
        public Usuario? Usuario { get; set; }
        public Eleccion? Eleccion { get; set; }
        public Partido? Partido { get; set; }
        public Cargo? Cargo { get; set; }
        public List<VotoEncriptado>? Votos { get; set; }
    }
}