namespace SistemaGestionJudicial.Models
{
    public class CrimeFilterViewModel
    {
        public DateOnly? FromDate { get; set; }
        public DateOnly? ToDate { get; set; }
        public string? CrimeType { get; set; }

        public List<CrimeReportViewModel> Results { get; set; } = new();
        public List<string> CrimeLabels { get; set; } = new();
        public List<int> CrimeCounts { get; set; } = new();
        public List<string> Months { get; set; } = new();
        public List<int> MonthlyTotals { get; set; } = new();

    }
}
