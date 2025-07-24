using System;
using System.Collections.Generic;

namespace SistemaGestionJudicial.Models;

public partial class JuiciosAcusado
{
    public long IdJuicioAcusado { get; set; }

    public long? IdJuicio { get; set; }

    public long? IdPersona { get; set; }

    public virtual Juicio? IdJuicioNavigation { get; set; }

    public virtual Persona? IdPersonaNavigation { get; set; }
}
