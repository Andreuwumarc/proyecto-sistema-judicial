using System;
using System.Collections.Generic;

namespace SistemaGestionJudicial.Models;

public partial class Delito
{
    public long IdDelito { get; set; }

    public string? Nombre { get; set; }

    public string? Descripcion { get; set; }

    public string? TipoDelito { get; set; }

    public string? GravedadDelito { get; set; }

    public virtual ICollection<Denuncia> Denuncia { get; set; } = new List<Denuncia>();
}
