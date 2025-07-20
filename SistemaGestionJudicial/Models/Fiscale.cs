using System;
using System.Collections.Generic;

namespace SistemaGestionJudicial.Models;

public partial class Fiscale
{
    public long IdFiscal { get; set; }

    public long? IdPersonaFiscal { get; set; }

    public long? IdDenuncia { get; set; }

    public DateOnly? FechaAsignacion { get; set; }

    public virtual Denuncia? IdDenunciaNavigation { get; set; }

    public virtual Persona? IdPersonaFiscalNavigation { get; set; }
}
