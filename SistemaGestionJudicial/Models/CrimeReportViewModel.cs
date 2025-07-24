namespace SistemaGestionJudicial.Models
{
    public class CrimeReportViewModel
    {
        public string? NombreDelito { get; set; }
        public DateOnly FechaDenuncia { get; set; }
        public string? Lugar { get; set; }
        public string? Demandante { get; set; }
        public string? Demandado { get; set; }
        public string? Severidad { get; set; }

    }
}
