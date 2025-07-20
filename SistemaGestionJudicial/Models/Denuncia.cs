using System;
using System.Collections.Generic;

namespace SistemaGestionJudicial.Models;

public partial class Denuncia
{
    public long IdDenuncia { get; set; }

    public DateOnly FechaDenuncia { get; set; }

    public string? LugarHecho { get; set; }

    public string? Descripcion { get; set; }

    public long? IdPersonaDenuncia { get; set; }

    public long? IdDelito { get; set; }

    public virtual ICollection<Fiscale> Fiscales { get; set; } = new List<Fiscale>();

    public virtual Delito? IdDelitoNavigation { get; set; }

    public virtual Persona? IdPersonaDenunciaNavigation { get; set; }

    public virtual ICollection<Juicio> Juicios { get; set; } = new List<Juicio>();

    public virtual ICollection<PartesPoliciale> PartesPoliciales { get; set; } = new List<PartesPoliciale>();
}
