using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaGestionJudicial.Models;

public partial class PartePolicial
{
    public long IdParte { get; set; }

    public DateOnly? FechaParte { get; set; }

    public string? Descripcion { get; set; }

    public long? IdPersonaPolicia { get; set; }

    public long? IdDenuncia { get; set; }

    [ForeignKey(nameof(IdPersonaPolicia))]
    public virtual Persona? IdPersonaPoliciaNavigation { get; set; }

    [ForeignKey(nameof(IdDenuncia))]
    public virtual Denuncia? IdDenunciaNavigation { get; set; }
}
