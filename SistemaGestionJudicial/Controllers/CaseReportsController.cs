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
            DateOnly? fechaJuicio,
            string judgeName,
            string prosecutorName,
            string denuncianteName,
            string demandadoName
        )
        {
            var query = dbContext.Juicios
                .Include(j => j.IdDenunciaNavigation)
                    .ThenInclude(d => d.IdDelitoNavigation)
                .Include(j => j.IdDenunciaNavigation)
                    .ThenInclude(d => d.IdPersonaDenunciaNavigation)
                .Include(j => j.JuiciosAcusados)
                    .ThenInclude(ja => ja.IdPersonaNavigation)
                .Include(j => j.IdPersonaJuezNavigation)
                .Include(j => j.IdDenunciaNavigation.Fiscales)
                    .ThenInclude(f => f.IdPersonaFiscalNavigation)
                .AsQueryable();

            if (fechaJuicio.HasValue)
            {
                query = query.Where(j => j.FechaInicio.HasValue && j.FechaInicio.Value == fechaJuicio.Value);
            }

            if (!string.IsNullOrEmpty(judgeName))
            {
                var parts = judgeName.Split(' ', System.StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 1)
                {
                    var filtro = $"%{parts[0]}%";
                    query = query.Where(j =>
                        EF.Functions.Like(j.IdPersonaJuezNavigation.Nombres, filtro) ||
                        EF.Functions.Like(j.IdPersonaJuezNavigation.Apellidos, filtro));
                }
                else if (parts.Length >= 2)
                {
                    var nombreFiltro = $"%{parts[0]}%";
                    var apellidoFiltro = $"%{parts[1]}%";
                    query = query.Where(j =>
                        EF.Functions.Like(j.IdPersonaJuezNavigation.Nombres, nombreFiltro) &&
                        EF.Functions.Like(j.IdPersonaJuezNavigation.Apellidos, apellidoFiltro));
                }
            }

            if (!string.IsNullOrEmpty(prosecutorName))
            {
                var parts = prosecutorName.Split(' ', System.StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 1)
                {
                    var filtro = $"%{parts[0]}%";
                    query = query.Where(j => j.IdDenunciaNavigation.Fiscales.Any(f =>
                        EF.Functions.Like(f.IdPersonaFiscalNavigation.Nombres, filtro) ||
                        EF.Functions.Like(f.IdPersonaFiscalNavigation.Apellidos, filtro)));
                }
                else if (parts.Length >= 2)
                {
                    var nombreFiltro = $"%{parts[0]}%";
                    var apellidoFiltro = $"%{parts[1]}%";
                    query = query.Where(j => j.IdDenunciaNavigation.Fiscales.Any(f =>
                        EF.Functions.Like(f.IdPersonaFiscalNavigation.Nombres, nombreFiltro) &&
                        EF.Functions.Like(f.IdPersonaFiscalNavigation.Apellidos, apellidoFiltro)));
                }
            }

            if (!string.IsNullOrEmpty(denuncianteName))
            {
                var parts = denuncianteName.Split(' ', System.StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 1)
                {
                    var filtro = $"%{parts[0]}%";
                    query = query.Where(j =>
                        EF.Functions.Like(j.IdDenunciaNavigation.IdPersonaDenunciaNavigation.Nombres, filtro) ||
                        EF.Functions.Like(j.IdDenunciaNavigation.IdPersonaDenunciaNavigation.Apellidos, filtro));
                }
                else if (parts.Length >= 2)
                {
                    var nombreFiltro = $"%{parts[0]}%";
                    var apellidoFiltro = $"%{parts[1]}%";
                    query = query.Where(j =>
                        EF.Functions.Like(j.IdDenunciaNavigation.IdPersonaDenunciaNavigation.Nombres, nombreFiltro) &&
                        EF.Functions.Like(j.IdDenunciaNavigation.IdPersonaDenunciaNavigation.Apellidos, apellidoFiltro));
                }
            }

            if (!string.IsNullOrEmpty(demandadoName))
            {
                var parts = demandadoName.Split(' ', System.StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 1)
                {
                    var filtro = $"%{parts[0]}%";
                    query = query.Where(j => j.JuiciosAcusados.Any(ja =>
                        EF.Functions.Like(ja.IdPersonaNavigation.Nombres, filtro) ||
                        EF.Functions.Like(ja.IdPersonaNavigation.Apellidos, filtro)));
                }
                else if (parts.Length >= 2)
                {
                    var nombreFiltro = $"%{parts[0]}%";
                    var apellidoFiltro = $"%{parts[1]}%";
                    query = query.Where(j => j.JuiciosAcusados.Any(ja =>
                        EF.Functions.Like(ja.IdPersonaNavigation.Nombres, nombreFiltro) &&
                        EF.Functions.Like(ja.IdPersonaNavigation.Apellidos, apellidoFiltro)));
                }
            }

            var juicios = await query.ToListAsync();

            if (juicios.Count == 0 && (
                fechaJuicio.HasValue ||
                !string.IsNullOrEmpty(judgeName) ||
                !string.IsNullOrEmpty(prosecutorName) ||
                !string.IsNullOrEmpty(denuncianteName) ||
                !string.IsNullOrEmpty(demandadoName)))
            {
                ViewBag.FilterError = "No existen juicios que coincidan con la combinación de filtros aplicados.";
            }

            ViewBag.TotalCases = juicios.Count;
            DateOnly fechaLimite = DateOnly.FromDateTime(DateTime.Today.AddDays(-30));
            ViewBag.RecentCases = juicios.Count(j => j.FechaInicio >= fechaLimite);

            ViewBag.Filtros = new CaseReportsFilterViewModel
            {
                FechaDenuncia = fechaJuicio,
                JudgeName = judgeName,
                ProsecutorName = prosecutorName,
                DenuncianteName = denuncianteName
            };

            return View("~/Views/Home/CaseReports.cshtml", juicios);
        }

        // Métodos de autocompletado igual pueden mantenerse
    }
}