using System;
using System.Collections.Generic;

namespace SistemaGestionJudicial.Models;

public partial class PartesPoliciale
{
    public long IdParte { get; set; }

    public DateOnly? FechaParte { get; set; }

    public string? Descripcion { get; set; }

    public long? IdPersonaPolicia { get; set; }

    public long? IdDenuncia { get; set; }

    public virtual Denuncia? IdDenunciaNavigation { get; set; }

    public virtual Persona? IdPersonaPoliciaNavigation { get; set; }
}
