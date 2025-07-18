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
        public virtual DbSet<Rol> Roles { get; set; }
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

            modelBuilder.Entity<Rol>(entity =>
            {
                entity.HasKey(e => e.IdRol).HasName("PK__roles__6ABCB5E0BE2F1518");

                entity.ToTable("roles");

                entity.Property(e => e.IdRol)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id_rol");
                entity.Property(e => e.Nombre)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("nombre");
            });
        }

    }
}
