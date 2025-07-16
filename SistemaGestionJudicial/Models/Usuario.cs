using System;
using System.Collections.Generic;

namespace SistemaGestionJudicial.Models;

public partial class Usuario
{
    public long IdUsuario { get; set; }

    public long? IdPersona { get; set; }

    public string Usuario1 { get; set; } = null!;

    public string Contraseña { get; set; } = null!;

    public string? Token { get; set; }

    public virtual Persona? IdPersonaNavigation { get; set; }
}
