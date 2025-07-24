namespace SistemaGestionJudicial.Models
{
    public class DashboardViewModel
{
    public int TotalOffenders { get; set; }
    public int ActiveCases { get; set; }
    public int Convictions { get; set; }
    public int PendingTrials { get; set; }
    public int SelectedDays { get; set; }

    public List<RecentCaseViewModel> RecentCases { get; set; }
    public List<CrimeStat> CrimeStats { get; set; }
}

public class RecentCaseViewModel
{
    public int IdJuicio { get; set; }
    public string Infractor { get; set; }
    public string Crimen { get; set; }
    public string Juez { get; set; }
    public string Status { get; set; }
}

public class CrimeStat
{
    public int Month { get; set; }
    public int Count { get; set; }
}

}