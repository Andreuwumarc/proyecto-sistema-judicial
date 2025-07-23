using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaGestionJudicial.Models;

public partial class Role
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long IdRol { get; set; } 
    public string Nombre { get; set; } = null!;
    public virtual ICollection<Persona> Personas { get; set; } = new List<Persona>();
}
