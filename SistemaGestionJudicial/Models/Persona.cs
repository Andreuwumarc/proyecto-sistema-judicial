using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaGestionJudicial.Models;

public partial class Persona
{
    [Key]  // Esto indica que es la clave primaria
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]  
    public long IdPersona { get; set; }
    public string Cedula { get; set; } = null!;
    public string? Nombres { get; set; }
    public string? Apellidos { get; set; }
    public DateOnly? FechaNacimiento { get; set; }
    public long? IdRol { get; set; }
    public string? Genero { get; set; }
    public string? Direccion { get; set; }
    public string? Telefono { get; set; }
    public string? CorreoElectronico { get; set; }
    public virtual Role? IdRolNavigation { get; set; }
}
