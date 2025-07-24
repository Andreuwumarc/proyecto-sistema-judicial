namespace SistemaGestionJudicial.Models
{
    public class CaseReportsFilterViewModel
    {
        public DateOnly? FechaDenuncia { get; set; } 
        public string JudgeName { get; set; }
        public string ProsecutorName { get; set; }
        public string DenuncianteName { get; set; }
    }

}
