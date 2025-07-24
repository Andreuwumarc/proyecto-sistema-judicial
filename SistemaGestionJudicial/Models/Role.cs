using System;
using System.Collections.Generic;

namespace SistemaGestionJudicial.Models;

public partial class Role
{
    public long IdRol { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<Persona> Personas { get; set; } = new List<Persona>();
}
