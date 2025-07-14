using System.ComponentModel.DataAnnotations;

namespace SistemaGestionJudicial.Models
{
    public class PartePolicial
    {
        public long Id_Parte { get; set; }
        public DateTime Fecha_Parte { get; set; }
        public string? Descripcion { get; set; }

        public long Id_Persona_Policia { get; set; }
        public Persona? PersonaPolicia { get; set; }
        public long Id_Denuncia { get; set; }
        public Denuncia? Denuncia { get; set; }

    }
}
