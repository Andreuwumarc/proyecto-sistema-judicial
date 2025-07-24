using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaGestionJudicial.Models;
using ClosedXML.Excel;
using System.IO;

namespace SistemaGestionJudicial.Controllers
{
    public class CrimeReportsController : Controller
    {
        private readonly ProyectoContext _context;

        public CrimeReportsController(ProyectoContext context)
        {
            _context = context;
        }

        [HttpGet("Home/CrimeReports")]
        public async Task<IActionResult> CrimeReports()
        {
            var model = await BuildFilteredReportsAsync(new CrimeFilterViewModel());
            return View("~/Views/Home/CrimeReports.cshtml", model);
        }

        [HttpPost("Home/CrimeReports")]
        public async Task<IActionResult> CrimeReportsFilter(CrimeFilterViewModel filter)
        {
            var model = await BuildFilteredReportsAsync(filter);
            return View("~/Views/Home/CrimeReports.cshtml", model);
        }

        [HttpGet]
        public async Task<IActionResult> SearchCrimeTypes(string term)
        {
            if (string.IsNullOrWhiteSpace(term))
                return Json(new List<string>());

            var results = await _context.Delitos
                .Where(d => d.Nombre.Contains(term))
                .OrderBy(d => d.Nombre)
                .Select(d => d.Nombre)
                .Take(10)
                .ToListAsync();

            return Json(results);
        }

        private async Task<CrimeFilterViewModel> BuildFilteredReportsAsync(CrimeFilterViewModel filter)
        {
            var query = _context.Denuncias
                .Include(d => d.IdDelitoNavigation)
                .Include(d => d.IdPersonaDenunciaNavigation)
                .AsQueryable();

            if (filter.FromDate.HasValue)
                query = query.Where(d => d.FechaDenuncia >= filter.FromDate.Value);

            if (filter.ToDate.HasValue)
                query = query.Where(d => d.FechaDenuncia <= filter.ToDate.Value);

            if (!string.IsNullOrWhiteSpace(filter.CrimeType))
                query = query.Where(d => d.IdDelitoNavigation.Nombre.Contains(filter.CrimeType));

            var result = await query.Select(d => new CrimeReportViewModel
            {
                NombreDelito = d.IdDelitoNavigation.Nombre,
                FechaDenuncia = d.FechaDenuncia,
                Lugar = d.LugarHecho,
                Demandante = d.IdPersonaDenunciaNavigation.Nombres + " " + d.IdPersonaDenunciaNavigation.Apellidos,
                Demandado = (from j in _context.Juicios
                             where j.IdDenuncia == d.IdDenuncia
                             from ja in _context.JuiciosAcusados
                             where ja.IdJuicio == j.IdJuicio
                             select ja.IdPersonaNavigation.Nombres + " " + ja.IdPersonaNavigation.Apellidos)
                             .FirstOrDefault() ?? "No asignado",
                Severidad = d.IdDelitoNavigation.GravedadDelito
            }).ToListAsync();

            // Donut chart
            filter.CrimeLabels = result
                .GroupBy(r => r.NombreDelito)
                .OrderByDescending(g => g.Count())
                .Take(5)
                .Select(g => g.Key)
                .ToList();

            filter.CrimeCounts = result
                .GroupBy(r => r.NombreDelito)
                .OrderByDescending(g => g.Count())
                .Take(5)
                .Select(g => g.Count())
                .ToList();

            // Line chart: total por mes
            filter.Months = result
                .GroupBy(r => r.FechaDenuncia.ToString("yyyy-MM"))
                .OrderBy(g => g.Key)
                .Select(g => g.Key)
                .ToList();

            filter.MonthlyTotals = result
                .GroupBy(r => r.FechaDenuncia.ToString("yyyy-MM"))
                .OrderBy(g => g.Key)
                .Select(g => g.Count())
                .ToList();

            filter.Results = result;
            return filter;
        }


        [HttpPost]
        public async Task<IActionResult> ExportToExcel(CrimeFilterViewModel filter)
        {
            var query = _context.Denuncias
                .Include(d => d.IdDelitoNavigation)
                .Include(d => d.IdPersonaDenunciaNavigation)
                .AsQueryable();

            if (filter.FromDate.HasValue)
                query = query.Where(d => d.FechaDenuncia >= filter.FromDate.Value);

            if (filter.ToDate.HasValue)
                query = query.Where(d => d.FechaDenuncia <= filter.ToDate.Value);

            if (!string.IsNullOrWhiteSpace(filter.CrimeType))
                query = query.Where(d => d.IdDelitoNavigation.Nombre.Contains(filter.CrimeType));

            var denuncias = await query.ToListAsync();

            // Datos para gráficos
            var crimeDistribution = denuncias
                .GroupBy(d => d.IdDelitoNavigation.Nombre)
                .OrderByDescending(g => g.Count())
                .Take(5)
                .ToDictionary(g => g.Key, g => g.Count());

            var monthlyTrend = denuncias
                .GroupBy(d => d.FechaDenuncia.ToString("yyyy-MM"))
                .OrderBy(g => g.Key)
                .ToDictionary(g => g.Key, g => g.Count());

            using var workbook = new XLWorkbook();

            // === Hoja 1: Datos ===
            var wsData = workbook.Worksheets.Add("Crime Reports");

            wsData.Cell(1, 1).Value = "Delito";
            wsData.Cell(1, 2).Value = "Descripción";
            wsData.Cell(1, 3).Value = "Descripción (Denuncia)";
            wsData.Cell(1, 4).Value = "Fecha";
            wsData.Cell(1, 5).Value = "Lugar";
            wsData.Cell(1, 6).Value = "Denunciante";
            wsData.Cell(1, 7).Value = "Sospechoso";
            wsData.Cell(1, 8).Value = "Severidad";
            wsData.Cell(1, 9).Value = "Juicio en curso";
            wsData.Cell(1, 10).Value = "Juicios concluidos";

            int row = 2;

            foreach (var d in denuncias)
            {
                var sospechoso = await _context.Juicios
                    .Where(j => j.IdDenuncia == d.IdDenuncia)
                    .SelectMany(j => _context.JuiciosAcusados
                        .Where(ja => ja.IdJuicio == j.IdJuicio)
                        .Select(ja => ja.IdPersonaNavigation.Nombres + " " + ja.IdPersonaNavigation.Apellidos))
                    .FirstOrDefaultAsync();

                sospechoso ??= "No asignado";

                bool juicioEnCurso = await _context.Juicios
                    .AnyAsync(j => j.IdDenuncia == d.IdDenuncia && j.Estado == "En progreso");

                int juiciosConcluidos = await _context.Juicios
                    .CountAsync(j => j.IdDenuncia == d.IdDenuncia && j.Estado == "Concluido");

                wsData.Cell(row, 1).Value = d.IdDelitoNavigation.Nombre;
                wsData.Cell(row, 2).Value = d.IdDelitoNavigation.Descripcion ?? "Sin descripción";
                wsData.Cell(row, 3).Value = d.Descripcion ?? "Sin descripción"; // Descripcion denuncia
                wsData.Cell(row, 4).Value = d.FechaDenuncia.ToString("yyyy-MM-dd");
                wsData.Cell(row, 5).Value = d.LugarHecho;
                wsData.Cell(row, 6).Value = d.IdPersonaDenunciaNavigation.Nombres + " " + d.IdPersonaDenunciaNavigation.Apellidos;
                wsData.Cell(row, 7).Value = sospechoso;
                wsData.Cell(row, 8).Value = d.IdDelitoNavigation.GravedadDelito;
                wsData.Cell(row, 9).Value = juicioEnCurso ? "Sí" : "No";
                wsData.Cell(row, 10).Value = juiciosConcluidos;

                row++;
            }

            wsData.Columns().AdjustToContents();

            // === Hoja 2: Gráficos ===
            var wsChart = workbook.Worksheets.Add("Gráficos");

            // Donut data
            int chartRow = 2;
            wsChart.Cell("A1").Value = "Delito";
            wsChart.Cell("B1").Value = "Cantidad";
            foreach (var item in crimeDistribution)
            {
                wsChart.Cell(chartRow, 1).Value = item.Key;
                wsChart.Cell(chartRow, 2).Value = item.Value;
                chartRow++;
            }

            // Línea data
            chartRow += 2;
            int lineStart = chartRow;
            wsChart.Cell(chartRow, 1).Value = "Mes";
            wsChart.Cell(chartRow, 2).Value = "Crímenes";
            chartRow++;
            foreach (var item in monthlyTrend)
            {
                wsChart.Cell(chartRow, 1).Value = item.Key;
                wsChart.Cell(chartRow, 2).Value = item.Value;
                chartRow++;
            }

            // NOTA: ClosedXML no soporta creación de gráficos, pero...
            // Excel los crea automáticamente si lo abres con una plantilla.
            // Puedes agregar manualmente los gráficos a la hoja Excel desde los datos exportados.

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            return File(stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"CrimeReports_{DateTime.Now:yyyyMMdd_HHmm}.xlsx");
        }
    }
}
