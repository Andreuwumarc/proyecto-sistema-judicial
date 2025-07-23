using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaGestionJudicial.Models;

namespace SistemaGestionJudicial.Controllers
{
    public class CaseReportsController : Controller
    {
        private readonly ProyectoContext dbContext;

        public CaseReportsController(ProyectoContext context)
        {
            dbContext = context;
        }

        [Route("/Home/CaseReports")]
        public async Task<IActionResult> Index(
            DateOnly? fechaDenuncia,
            string judgeName,
            string prosecutorName,
            string denuncianteName
        )
        {
            var query = dbContext.Denuncias
                .Include(d => d.IdPersonaDenunciaNavigation)
                .Include(d => d.IdDelitoNavigation)
                .Include(d => d.Juicios)
                    .ThenInclude(j => j.IdPersonaJuezNavigation)
                .Include(d => d.Fiscales)
                    .ThenInclude(f => f.IdPersonaFiscalNavigation)
                .AsQueryable();

            if (fechaDenuncia.HasValue)
            {
                query = query.Where(d => d.FechaDenuncia == fechaDenuncia.Value);
            }

            // Filtrar juez por nombre y apellido
            if (!string.IsNullOrEmpty(judgeName))
            {
                var parts = judgeName.Split(' ', System.StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 1)
                {
                    var filtro = $"%{parts[0]}%";
                    query = query.Where(d => d.Juicios.Any(j =>
                        EF.Functions.Like(j.IdPersonaJuezNavigation.Nombres, filtro) ||
                        EF.Functions.Like(j.IdPersonaJuezNavigation.Apellidos, filtro)
                    ));
                }
                else if (parts.Length >= 2)
                {
                    var nombreFiltro = $"%{parts[0]}%";
                    var apellidoFiltro = $"%{parts[1]}%";
                    query = query.Where(d => d.Juicios.Any(j =>
                        EF.Functions.Like(j.IdPersonaJuezNavigation.Nombres, nombreFiltro) &&
                        EF.Functions.Like(j.IdPersonaJuezNavigation.Apellidos, apellidoFiltro)
                    ));
                }
            }

            // Filtrar fiscal por nombre y apellido
            if (!string.IsNullOrEmpty(prosecutorName))
            {
                var parts = prosecutorName.Split(' ', System.StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 1)
                {
                    var filtro = $"%{parts[0]}%";
                    query = query.Where(d => d.Fiscales.Any(f =>
                        EF.Functions.Like(f.IdPersonaFiscalNavigation.Nombres, filtro) ||
                        EF.Functions.Like(f.IdPersonaFiscalNavigation.Apellidos, filtro)
                    ));
                }
                else if (parts.Length >= 2)
                {
                    var nombreFiltro = $"%{parts[0]}%";
                    var apellidoFiltro = $"%{parts[1]}%";
                    query = query.Where(d => d.Fiscales.Any(f =>
                        EF.Functions.Like(f.IdPersonaFiscalNavigation.Nombres, nombreFiltro) &&
                        EF.Functions.Like(f.IdPersonaFiscalNavigation.Apellidos, apellidoFiltro)
                    ));
                }
            }

            // Filtrar denunciante por nombre y apellido
            if (!string.IsNullOrEmpty(denuncianteName))
            {
                var parts = denuncianteName.Split(' ', System.StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 1)
                {
                    var filtro = $"%{parts[0]}%";
                    query = query.Where(d =>
                        EF.Functions.Like(d.IdPersonaDenunciaNavigation.Nombres, filtro) ||
                        EF.Functions.Like(d.IdPersonaDenunciaNavigation.Apellidos, filtro)
                    );
                }
                else if (parts.Length >= 2)
                {
                    var nombreFiltro = $"%{parts[0]}%";
                    var apellidoFiltro = $"%{parts[1]}%";
                    query = query.Where(d =>
                        EF.Functions.Like(d.IdPersonaDenunciaNavigation.Nombres, nombreFiltro) &&
                        EF.Functions.Like(d.IdPersonaDenunciaNavigation.Apellidos, apellidoFiltro)
                    );
                }
            }


            var denuncias = await query.ToListAsync();

            if (denuncias.Count == 0 && (
                fechaDenuncia.HasValue ||
                !string.IsNullOrEmpty(judgeName) ||
                !string.IsNullOrEmpty(prosecutorName) ||
                !string.IsNullOrEmpty(denuncianteName))
                )
            {
                // No coincidieron los filtros con ningún resultado
                ViewBag.FilterError = "No existen casos que coincidan con la combinación de filtros aplicados. Por favor, ajusta los criterios.";
            }

            // Total de casos filtrados
            int totalCases = denuncias.Count;

            // Casos recientes (últimos 30 días)
            DateOnly fechaLimite = DateOnly.FromDateTime(DateTime.Today.AddDays(-30));
            int recentCases = denuncias.Count(d => d.FechaDenuncia >= fechaLimite);

            ViewBag.TotalCases = totalCases;
            ViewBag.RecentCases = recentCases;

            // Pasar los filtros actuales a la vista para mantener los valores en el formulario
            ViewBag.Filtros = new CaseReportsFilterViewModel
            {
                FechaDenuncia = fechaDenuncia,
                JudgeName = judgeName,
                ProsecutorName = prosecutorName,
                DenuncianteName = denuncianteName
            };

            // Listas extras para filtros o dropdowns
            ViewBag.Personas = await dbContext.Personas
                .Select(p => new { p.IdPersona, p.Nombres, p.Apellidos })
                .ToListAsync();

            ViewBag.Delitos = await dbContext.Delitos
                .Select(d => new { d.IdDelito, d.Nombre })
                .ToListAsync();


            return View("~/Views/Home/CaseReports.cshtml", denuncias);
        }

        [HttpGet]
        public async Task<JsonResult> GetJudgeSuggestions(string term)
        {
            if (string.IsNullOrWhiteSpace(term))
                return Json(new List<string>());

            var results = await dbContext.Personas
                .Where(p => p.Juicios.Any() &&
                       (p.Nombres.Contains(term) || p.Apellidos.Contains(term)))
                .Select(p => p.Nombres + " " + p.Apellidos)
                .Distinct()
                .Take(10)
                .ToListAsync();

            return Json(results);
        }

        [HttpGet]
        public async Task<JsonResult> GetProsecutorSuggestions(string term)
        {
            if (string.IsNullOrWhiteSpace(term))
                return Json(new List<string>());

            var results = await dbContext.Personas
                .Where(p => p.Fiscales.Any() &&
                       (p.Nombres.Contains(term) || p.Apellidos.Contains(term)))
                .Select(p => p.Nombres + " " + p.Apellidos)
                .Distinct()
                .Take(10)
                .ToListAsync();

            return Json(results);
        }

        [HttpGet]
        public async Task<JsonResult> GetDenuncianteSuggestions(string term)
        {
            if (string.IsNullOrWhiteSpace(term))
                return Json(new List<string>());

            var results = await dbContext.Personas
                .Where(p => p.Denuncia.Any() &&
                       (p.Nombres.Contains(term) || p.Apellidos.Contains(term)))
                .Select(p => p.Nombres + " " + p.Apellidos)
                .Distinct()
                .Take(10)
                .ToListAsync();

            return Json(results);
        }



    }
}
