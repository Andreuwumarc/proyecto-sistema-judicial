using System;
using System.Collections.Generic;

namespace SistemaGestionJudicial.Models;

public partial class Sentencia
{
    public long IdSentencia { get; set; }

    public long? IdJuicio { get; set; }

    public DateOnly? FechaSentencia { get; set; }

    public string? TipoSentencia { get; set; }

    public string? Pena { get; set; }

    public string? Observaciones { get; set; }

    public virtual Juicio? IdJuicioNavigation { get; set; }
}
