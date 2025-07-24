using Microsoft.AspNetCore.Mvc;
using SistemaGestionJudicial.Models;

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

        return View("~/Views/Home/Panel.cshtml", viewModel);
    }

    //public IActionResult Index()
    //{
    //    var now = DateTime.Now;
    //    DateOnly cutoff = DateOnly.FromDateTime(DateTime.Today.AddMonths(-11));
    //    //var cutoff = now.AddMonths(-11); // últimos 12 meses incluyendo actual

    //    var stats = _context.Juicios
    //        .Where(j => j.FechaInicio >= cutoff)
    //        .AsEnumerable()
    //        .GroupBy(j => j.FechaInicio.Value.ToString("MMMM", new CultureInfo("es-ES")))
    //        .OrderBy(g => DateTime.ParseExact(g.Key, "MMMM", new CultureInfo("es-ES")))
    //        .Select(g => new {
    //            Month = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(g.Key),
    //            Count = g.Count()
    //        })
    //        .ToList();

    //    ViewBag.Labels = stats.Select(s => s.Month).ToList();
    //    ViewBag.Values = stats.Select(s => s.Count).ToList();

    //    return View();
    //}


    public async Task<IActionResult> GetDashboardData()
    {
        var totalOffenders = await _context.Personas.CountAsync(o => o.IdRol == 3);
        var activeCases = await _context.Juicios.CountAsync(c => c.Estado == "En progreso");
        var convictions = await _context.Juicios.CountAsync(c => c.Estado == "Concluido");
        var pendingTrials = await _context.Juicios.CountAsync(c => c.Estado != "Concluido");

        var crimeStats = await _context.Juicios
            .Where(j => j.FechaInicio != null)
            .GroupBy(j => j.FechaInicio.Value.Month)
            .Select(g => new
            {
                Month = g.Key,
                Count = g.Count()
            })
            .OrderBy(g => g.Month)
            .ToListAsync();


        var recentCases = await _context.Juicios
            .Include(j => j.IdDenunciaNavigation)
                .ThenInclude(d => d.IdDelitoNavigation)
            .Include(j => j.IdPersonaJuezNavigation)
            //.ThenInclude(p => p.IdPersona)
            .Include(j => j.JuiciosAcusados)
                .ThenInclude(ja => ja.IdPersonaNavigation)
            .OrderByDescending(j => j.FechaInicio)
            .Take(4)
            .Select(j => new
            {
                j.IdJuicio,
                Infractor = j.JuiciosAcusados
                            .Select(ja => ja.IdPersonaNavigation.Nombres + " " + ja.IdPersonaNavigation.Apellidos)
                            .FirstOrDefault(), // Si hay más de un acusado, tomamos el primero
                Crimen = j.IdDenunciaNavigation.IdDelitoNavigation.Nombre,
                Juez = j.IdPersonaJuezNavigation.Nombres + " " + j.IdPersonaJuezNavigation.Apellidos,
                Status = j.Estado
            })
            .ToListAsync();



        return Json(new
        {
            totalOffenders,
            activeCases,
            convictions,
            pendingTrials,
            crimeStats,
            recentCases
        });
    }

    [HttpGet]
    public async Task<IActionResult> GetData(int days = 365)
    {
        DateOnly cutoff = DateOnly.FromDateTime(DateTime.Today.AddDays(-days));
        Console.WriteLine(cutoff.ToString());

        var crimeStatsFilter = await _context.Juicios
            .Where(j => j.FechaInicio.HasValue && j.FechaInicio >= cutoff)
            .GroupBy(j => j.FechaInicio.Value.Month)
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