namespace SistemaGestionJudicial.Models
{
    public class PoliceViewModel
    {
        // Parte Policial
        public long Id_Parte { get; set; }
        public DateTime Fecha_Parte { get; set; }
        public string? Descripcion { get; set; }

        // Policia
        public long Id_Persona_Policia { get; set; } // FK
        public string? CedulaPolicia { get; set; }
        public string? NombresPolicia { get; set; }
        public string? ApellidosPolicia { get; set; }

        // Denuncia
        public long Id_Denuncia { get; set; } // FK
        public string? DescripcionDenuncia { get; set; }
        public List<PartePolicial> Partes { get; set; } = new List<PartePolicial>();

    }
}
