using SistemaGestionJudicial.Models;
using SistemaGestionJudicial.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

[SessionCheck]
public class DashboardController : Controller
{
    private readonly ProyectoContext _context;

    public DashboardController(ProyectoContext context)
    {
        _context = context;
    }

    [HttpGet]
    [Route("Dashboard")]
    public async Task<IActionResult> Index()
    {

        var viewModel = new DashboardViewModel
        {
            TotalOffenders = await _context.Personas.CountAsync(o => o.IdRol == 3),
            ActiveCases = await _context.Juicios.CountAsync(j => j.Estado == "En Progreso"),
            Convictions = await _context.Juicios.CountAsync(j => j.Estado == "Concluido"),
            PendingTrials = await _context.Juicios.CountAsync(j => j.Estado == "Programado"),
            RecentCases = await _context.Juicios
                .Include(j => j.IdDenunciaNavigation)
                    .ThenInclude(d => d.IdDelitoNavigation)
                .Include(j => j.IdPersonaJuezNavigation)
                .Include(j => j.JuiciosAcusados)
                    .ThenInclude(ja => ja.IdPersonaNavigation)
                .OrderByDescending(j => j.FechaInicio)
                .Take(4)
                .Select(j => new RecentCaseViewModel
                {
                    IdJuicio = (int)j.IdJuicio,
                    Infractor = j.JuiciosAcusados.FirstOrDefault().IdPersonaNavigation.Nombres + " " + j.JuiciosAcusados.FirstOrDefault().IdPersonaNavigation.Apellidos, // First or Defalut es en caso de que haya mas de un delincuente asociado a un juicio, solo muestre el primero
                    Crimen = j.IdDenunciaNavigation.IdDelitoNavigation.Nombre,
                    Juez = j.IdPersonaJuezNavigation.Nombres + " " + j.IdPersonaJuezNavigation.Apellidos,
                    Status = j.Estado
                }).ToListAsync(),
        };

        return View("~/Views/Home/Panel.cshtml",viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> GetData(int days = 365)
    {
        DateOnly cutoff = DateOnly.FromDateTime(DateTime.Today.AddDays(-days));
        Console.WriteLine(cutoff.ToString());   

        var crimeStatsFilter = await _context.Denuncias
            .Where(d => d.FechaDenuncia != null && d.FechaDenuncia >= cutoff)
            .GroupBy(j => j.FechaDenuncia.Month)
            .Select(g => new {
                Month = g.Key,
                Count = g.Count()
            }).ToListAsync();

        return Json(new
        {
            crimeStatsFilter
        });
    }

}
