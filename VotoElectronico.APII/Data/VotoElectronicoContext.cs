using Microsoft.EntityFrameworkCore;
using Models;

namespace VotoElectronico.APII.Data
{
    public class VotoElectronicoContext : DbContext
    {
        public VotoElectronicoContext(DbContextOptions<VotoElectronicoContext> options)
            : base(options)
        {
        }

        // Entidades principales
        public DbSet<Usuario> Usuarios { get; set; } = default!;
        public DbSet<Eleccion> Elecciones { get; set; } = default!;
        public DbSet<Candidato> Candidatos { get; set; } = default!;
        public DbSet<Partido> Partidos { get; set; } = default!;
        public DbSet<Cargo> Cargos { get; set; } = default!;
        public DbSet<VotoEncriptado> VotosEncriptados { get; set; } = default!;
        public DbSet<Resultado> Resultados { get; set; } = default!;
        public DbSet<AuditoriaVoto> AuditoriasVotos { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurar Usuario
            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Cedula)
                .IsUnique();

            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Configurar Candidato
            modelBuilder.Entity<Candidato>()
                .HasOne(c => c.Usuario)
                .WithMany()
                .HasForeignKey(c => c.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Candidato>()
                .HasOne(c => c.Eleccion)
                .WithMany(e => e.Candidatos)
                .HasForeignKey(c => c.EleccionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Candidato>()
                .HasOne(c => c.Partido)
                .WithMany(p => p.Candidatos)
                .HasForeignKey(c => c.PartidoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Candidato>()
                .HasOne(c => c.Cargo)
                .WithMany(ca => ca.Candidatos)
                .HasForeignKey(c => c.CargoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configurar VotoEncriptado
            modelBuilder.Entity<VotoEncriptado>()
                .HasOne(v => v.Eleccion)
                .WithMany(e => e.Votos)
                .HasForeignKey(v => v.EleccionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<VotoEncriptado>()
                .HasOne(v => v.Candidato)
                .WithMany(c => c.Votos)
                .HasForeignKey(v => v.CandidatoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Índice para evitar doble votación (un hash por elección)
            modelBuilder.Entity<VotoEncriptado>()
                .HasIndex(v => new { v.HashVotante, v.EleccionId })
                .IsUnique();

            // Configurar Resultado
            modelBuilder.Entity<Resultado>()
                .HasOne(r => r.Eleccion)
                .WithMany(e => e.Resultados)
                .HasForeignKey(r => r.EleccionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Resultado>()
                .HasOne(r => r.Candidato)
                .WithMany()
                .HasForeignKey(r => r.CandidatoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configurar AuditoriaVoto
            modelBuilder.Entity<AuditoriaVoto>()
                .HasOne(a => a.Eleccion)
                .WithMany()
                .HasForeignKey(a => a.EleccionId)
                .OnDelete(DeleteBehavior.Restrict);

            // Índice para HashVoto (verificar integridad)
            modelBuilder.Entity<VotoEncriptado>()
                .HasIndex(v => v.HashVoto)
                .IsUnique();
        }
    }
}