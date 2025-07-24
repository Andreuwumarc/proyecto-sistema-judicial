using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace SistemaGestionJudicial.Models;

public partial class ProyectoContext : DbContext
{
    public ProyectoContext()
    {
    }

    public ProyectoContext(DbContextOptions<ProyectoContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Delito> Delitos { get; set; }

    public virtual DbSet<Denuncia> Denuncias { get; set; }

    public virtual DbSet<Fiscale> Fiscales { get; set; }

    public virtual DbSet<Juicio> Juicios { get; set; }

    public virtual DbSet<JuiciosAcusado> JuiciosAcusados { get; set; }

    public virtual DbSet<PartesPoliciale> PartesPoliciales { get; set; }

    public virtual DbSet<Persona> Personas { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Sentencia> Sentencias { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
/*#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer();*/
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Delito>(entity =>
        {
            entity.HasKey(e => e.IdDelito).HasName("PK__delitos__2C33B839D3CF507E");

            entity.ToTable("delitos");

            entity.Property(e => e.IdDelito)
                .ValueGeneratedNever()
                .HasColumnName("id_delito");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(300)
                .IsUnicode(false)
                .HasColumnName("descripcion");
            entity.Property(e => e.GravedadDelito)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("gravedad_delito");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("nombre");
            entity.Property(e => e.TipoDelito)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("tipo_delito");
        });

        modelBuilder.Entity<Denuncia>(entity =>
        {
            entity.HasKey(e => e.IdDenuncia).HasName("PK__denuncia__2BD955A495AF9B36");

            entity.ToTable("denuncias");

            entity.Property(e => e.IdDenuncia)
                .ValueGeneratedNever()
                .HasColumnName("id_denuncia");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("descripcion");
            entity.Property(e => e.FechaDenuncia).HasColumnName("fecha_denuncia");
            entity.Property(e => e.IdDelito).HasColumnName("id_delito");
            entity.Property(e => e.IdPersonaDenuncia).HasColumnName("id_persona_denuncia");
            entity.Property(e => e.LugarHecho)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("lugar_hecho");

            entity.HasOne(d => d.IdDelitoNavigation).WithMany(p => p.Denuncia)
                .HasForeignKey(d => d.IdDelito)
                .HasConstraintName("fk_denuncias_delitos");

            entity.HasOne(d => d.IdPersonaDenunciaNavigation).WithMany(p => p.Denuncia)
                .HasForeignKey(d => d.IdPersonaDenuncia)
                .HasConstraintName("fk_denuncias_personas");
        });

        modelBuilder.Entity<Fiscale>(entity =>
        {
            entity.HasKey(e => e.IdFiscal).HasName("PK__fiscales__F787D6251A47514F");

            entity.ToTable("fiscales");

            entity.Property(e => e.IdFiscal)
                .ValueGeneratedNever()
                .HasColumnName("id_fiscal");
            entity.Property(e => e.FechaAsignacion).HasColumnName("fecha_asignacion");
            entity.Property(e => e.IdDenuncia).HasColumnName("id_denuncia");
            entity.Property(e => e.IdPersonaFiscal).HasColumnName("id_persona_fiscal");

            entity.HasOne(d => d.IdDenunciaNavigation).WithMany(p => p.Fiscales)
                .HasForeignKey(d => d.IdDenuncia)
                .HasConstraintName("fk_fiscales_denuncias");

            entity.HasOne(d => d.IdPersonaFiscalNavigation).WithMany(p => p.Fiscales)
                .HasForeignKey(d => d.IdPersonaFiscal)
                .HasConstraintName("fk_fiscales_personas");
        });

        modelBuilder.Entity<Juicio>(entity =>
        {
            entity.HasKey(e => e.IdJuicio).HasName("PK__juicios__C34C7089D25FC735");

            entity.ToTable("juicios");

            entity.Property(e => e.IdJuicio)
                .ValueGeneratedNever()
                .HasColumnName("id_juicio");
            entity.Property(e => e.Estado)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("estado");
            entity.Property(e => e.FechaFin).HasColumnName("fecha_fin");
            entity.Property(e => e.FechaInicio).HasColumnName("fecha_inicio");
            entity.Property(e => e.IdDenuncia).HasColumnName("id_denuncia");
            entity.Property(e => e.IdPersonaJuez).HasColumnName("id_persona_juez");

            entity.HasOne(d => d.IdDenunciaNavigation).WithMany(p => p.Juicios)
                .HasForeignKey(d => d.IdDenuncia)
                .HasConstraintName("fk_juicios_denuncias");

            entity.HasOne(d => d.IdPersonaJuezNavigation).WithMany(p => p.Juicios)
                .HasForeignKey(d => d.IdPersonaJuez)
                .HasConstraintName("fk_juicios_personas");
        });

        modelBuilder.Entity<JuiciosAcusado>(entity =>
        {
            entity.HasKey(e => e.IdJuicioAcusado).HasName("PK__juicios___099870E2FB95B41C");

            entity.ToTable("juicios_acusados");

            entity.Property(e => e.IdJuicioAcusado)
                .ValueGeneratedNever()
                .HasColumnName("id_juicio_acusado");
            entity.Property(e => e.IdJuicio).HasColumnName("id_juicio");
            entity.Property(e => e.IdPersona).HasColumnName("id_persona");

            entity.HasOne(d => d.IdJuicioNavigation).WithMany(p => p.JuiciosAcusados)
                .HasForeignKey(d => d.IdJuicio)
                .HasConstraintName("fk_acusados_juicios");

            entity.HasOne(d => d.IdPersonaNavigation).WithMany(p => p.JuiciosAcusados)
                .HasForeignKey(d => d.IdPersona)
                .HasConstraintName("fk_acusados_personas");
        });

        modelBuilder.Entity<PartesPoliciale>(entity =>
        {
            entity.HasKey(e => e.IdParte).HasName("PK__partes_p__3F12D5842FADFC0E");

            entity.ToTable("partes_policiales");

            entity.Property(e => e.IdParte)
                .ValueGeneratedNever()
                .HasColumnName("id_parte");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("descripcion");
            entity.Property(e => e.FechaParte).HasColumnName("fecha_parte");
            entity.Property(e => e.IdDenuncia).HasColumnName("id_denuncia");
            entity.Property(e => e.IdPersonaPolicia).HasColumnName("id_persona_policia");

            entity.HasOne(d => d.IdDenunciaNavigation).WithMany(p => p.PartesPoliciales)
                .HasForeignKey(d => d.IdDenuncia)
                .HasConstraintName("fk_partes_denuncias");

            entity.HasOne(d => d.IdPersonaPoliciaNavigation).WithMany(p => p.PartesPoliciales)
                .HasForeignKey(d => d.IdPersonaPolicia)
                .HasConstraintName("fk_partes_personas");
        });

        modelBuilder.Entity<Persona>(entity =>
        {
            entity.HasKey(e => e.IdPersona).HasName("PK__personas__228148B0D9BBA585");

            entity.ToTable("personas");

            entity.Property(e => e.IdPersona)
                .ValueGeneratedNever()
                .HasColumnName("id_persona");
            entity.Property(e => e.Apellidos)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("apellidos");
            entity.Property(e => e.Cedula)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("cedula");
            entity.Property(e => e.CorreoElectronico)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("correo_electronico");
            entity.Property(e => e.Direccion)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("direccion");
            entity.Property(e => e.FechaNacimiento).HasColumnName("fecha_nacimiento");
            entity.Property(e => e.Genero)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("genero");
            entity.Property(e => e.IdRol).HasColumnName("id_rol");
            entity.Property(e => e.Nombres)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("nombres");
            entity.Property(e => e.Telefono)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("telefono");

            entity.HasOne(d => d.IdRolNavigation).WithMany(p => p.Personas)
                .HasForeignKey(d => d.IdRol)
                .HasConstraintName("fk_personas_roles");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.IdRol).HasName("PK__roles__6ABCB5E0BE2F1518");

            entity.ToTable("roles");

            entity.Property(e => e.IdRol)
                .ValueGeneratedNever()
                .HasColumnName("id_rol");
            entity.Property(e => e.Nombre)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<Sentencia>(entity =>
        {
            entity.HasKey(e => e.IdSentencia).HasName("PK__sentenci__63E1C48B8B1E229F");

            entity.ToTable("sentencias");

            entity.Property(e => e.IdSentencia)
                .ValueGeneratedNever()
                .HasColumnName("id_sentencia");
            entity.Property(e => e.FechaSentencia).HasColumnName("fecha_sentencia");
            entity.Property(e => e.IdJuicio).HasColumnName("id_juicio");
            entity.Property(e => e.Observaciones)
                .HasMaxLength(300)
                .IsUnicode(false)
                .HasColumnName("observaciones");
            entity.Property(e => e.Pena)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("pena");
            entity.Property(e => e.TipoSentencia)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("tipo_sentencia");

            entity.HasOne(d => d.IdJuicioNavigation).WithMany(p => p.Sentencia)
                .HasForeignKey(d => d.IdJuicio)
                .HasConstraintName("fk_sentencias_juicios");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.IdUsuario).HasName("PK__usuarios__4E3E04AD2A4B8E26");

            entity.ToTable("usuarios");

            entity.Property(e => e.IdUsuario)
                .ValueGeneratedNever()
                .HasColumnName("id_usuario");
            entity.Property(e => e.Contraseña)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("contraseña");
            entity.Property(e => e.IdPersona).HasColumnName("id_persona");
            entity.Property(e => e.Token)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("token");
            entity.Property(e => e.Usuario1)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("usuario");

            entity.HasOne(d => d.IdPersonaNavigation).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.IdPersona)
                .HasConstraintName("fk_usuarios_personas");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
