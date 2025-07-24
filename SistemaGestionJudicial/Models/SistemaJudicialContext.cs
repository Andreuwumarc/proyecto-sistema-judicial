using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace SistemaGestionJudicial.Models;

public partial class SistemaJudicialContext : DbContext
{
    public SistemaJudicialContext()
    {
    }

    public SistemaJudicialContext(DbContextOptions<SistemaJudicialContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Persona> Personas { get; set; }

    public virtual DbSet<Role> Roles { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { 
    
        /*#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=sistema_judicial;Integrated Security=True;TrustServerCertificate=True");
        */
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<Persona>(entity =>
        {
            entity.HasKey(e => e.IdPersona).HasName("PK__personas__228148B0E4A3F376");

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
            entity.HasKey(e => e.IdRol).HasName("PK__roles__6ABCB5E079CD5394");

            entity.ToTable("roles");

            entity.Property(e => e.IdRol)
                .ValueGeneratedNever()
                .HasColumnName("id_rol");
            entity.Property(e => e.Nombre)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("nombre");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}