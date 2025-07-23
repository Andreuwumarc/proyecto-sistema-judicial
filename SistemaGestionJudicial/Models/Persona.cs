using System;
using System.Collections.Generic;

namespace SistemaGestionJudicial.Models;

public partial class Persona
{
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

    public virtual ICollection<Denuncia> Denuncia { get; set; } = new List<Denuncia>();

    public virtual ICollection<Fiscale> Fiscales { get; set; } = new List<Fiscale>();

    public virtual Role? IdRolNavigation { get; set; }

    public virtual ICollection<Juicio> Juicios { get; set; } = new List<Juicio>();

    public virtual ICollection<JuiciosAcusado> JuiciosAcusados { get; set; } = new List<JuiciosAcusado>();

    public virtual ICollection<PartesPoliciale> PartesPoliciales { get; set; } = new List<PartesPoliciale>();

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
