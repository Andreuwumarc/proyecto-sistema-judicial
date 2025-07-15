using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;

namespace SistemaGestionJudicial.Models
{

    public class SistemaContext : DbContext
    {
        public SistemaContext(DbContextOptions<SistemaContext> options)
    : base(options)
        {
        }

        public DbSet<PartePolicial> PartesPoliciales { get; set; }
        public DbSet<Persona> Personas { get; set; }
        public DbSet<Denuncia> Denuncias { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Denuncia>(entity =>
            {
                entity.ToTable("denuncias");   // Nombre real de la tabla
                entity.HasKey(e => e.Id_Denuncia);
            });

            modelBuilder.Entity<PartePolicial>(entity =>
            {
                entity.ToTable("partes_policiales");  // Nombre real de la tabla
                entity.HasKey(e => e.Id_Parte);

                entity.HasOne(p => p.PersonaPolicia)
                    .WithMany()
                    .HasForeignKey(p => p.Id_Persona_Policia);

                entity.HasOne(p => p.Denuncia)
                    .WithMany()
                    .HasForeignKey(p => p.Id_Denuncia);
            });

            modelBuilder.Entity<Persona>(entity =>
            {
                entity.ToTable("personas");   // Nombre real de la tabla
                entity.HasKey(e => e.Id_Persona);
            });
        }

    }
}
