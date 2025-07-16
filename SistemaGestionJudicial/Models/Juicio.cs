using System;
using System.Collections.Generic;

namespace SistemaGestionJudicial.Models;

public partial class Juicio
{
    public long IdJuicio { get; set; }

    public DateOnly? FechaInicio { get; set; }

    public DateOnly? FechaFin { get; set; }

    public long? IdDenuncia { get; set; }

    public long? IdPersonaJuez { get; set; }

    public string? Estado { get; set; }

    public virtual Denuncia? IdDenunciaNavigation { get; set; }

    public virtual Persona? IdPersonaJuezNavigation { get; set; }

    public virtual ICollection<JuiciosAcusado> JuiciosAcusados { get; set; } = new List<JuiciosAcusado>();

    public virtual ICollection<Sentencia> Sentencia { get; set; } = new List<Sentencia>();
}
