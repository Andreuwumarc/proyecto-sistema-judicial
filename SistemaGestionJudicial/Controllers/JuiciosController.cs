using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaGestionJudicial.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaGestionJudicial.Controllers
{
    public class JuiciosController : Controller
    {
        private readonly ProyectoContext _context;
        public JuiciosController(ProyectoContext context)
        {
            _context = context;
        }

        // GET: /Home/Juicios
        [HttpGet("/Home/Juicios")]
        public async Task<IActionResult> Juicios(
            long? juezId,
            long? fiscalId,
            string ordenFecha,
            DateTime? fechaDesde,
            DateTime? fechaHasta)
        {
            // Cargar dropdowns
            ViewData["Jueces"] = await _context.Personas.Where(p => p.IdRol == 1).ToListAsync();
            ViewData["Fiscales"] = await _context.Personas.Where(p => p.IdRol == 2).ToListAsync();
            ViewData["Delitos"] = await _context.Delitos.ToListAsync();
            ViewData["PotencialesAcusados"] = await _context.Personas.Where(p => p.IdRol == 3).ToListAsync();
            ViewData["Denunciantes"] = await _context.Personas.Where(p => p.IdRol == 4).ToListAsync();
            ViewData["Estados"] = new List<string> { "Programado", "En proceso", "Concluido" };
            ViewData["SentenciasPosibles"] = new List<string> { "Pendiente", "Culpable", "Inocente" };

            // Base query
            var q = _context.Juicios
                .Include(j => j.IdPersonaJuezNavigation)
                .Include(j => j.IdDenunciaNavigation)
                    .ThenInclude(d => d.Fiscales)
                        .ThenInclude(f => f.IdPersonaFiscalNavigation)
                .Include(j => j.IdDenunciaNavigation)
                    .ThenInclude(d => d.IdDelitoNavigation)
                .Include(j => j.IdDenunciaNavigation)
                    .ThenInclude(d => d.IdPersonaDenunciaNavigation)
                .Include(j => j.JuiciosAcusados).ThenInclude(a => a.IdPersonaNavigation)
                .Include(j => j.Sentencia)
                .AsQueryable();

            // Aplicar filtros
            if (juezId.HasValue)
                q = q.Where(j => j.IdPersonaJuez == juezId);
            if (fiscalId.HasValue)
                q = q.Where(j => j.IdDenunciaNavigation!.Fiscales.Any(f => f.IdPersonaFiscal == fiscalId));
            if (fechaDesde.HasValue)
                q = q.Where(j => j.FechaInicio >= DateOnly.FromDateTime(fechaDesde.Value));
            if (fechaHasta.HasValue)
                q = q.Where(j => j.FechaFin <= DateOnly.FromDateTime(fechaHasta.Value));

            // Orden
            q = ordenFecha switch
            {
                "asc" => q.OrderBy(j => j.FechaInicio),
                "desc" => q.OrderByDescending(j => j.FechaInicio),
                _ => q
            };

            var lista = await q.ToListAsync();
            return View("~/Views/Home/Juicios.cshtml", lista);
        }

        // POST: Crear
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("FechaInicio,FechaFin,IdPersonaJuez,Estado")] Juicio j,
            long IdDelito,
            long IdPersonaDenuncia,
            long IdPersonaFiscal,
            long IdPersonaAcusado)
        {
            // 1) Crear Denuncia
            long maxDenunciaId = await _context.Denuncias.MaxAsync(d => (long?)d.IdDenuncia) ?? 0;
            var nuevaDenuncia = new Denuncia
            {
                IdDenuncia = maxDenunciaId + 1,
                FechaDenuncia = j.FechaInicio ?? DateOnly.FromDateTime(DateTime.Today),
                IdDelito = IdDelito,
                IdPersonaDenuncia = IdPersonaDenuncia,
                Descripcion = "Auto-generada desde juicio",
                LugarHecho = "Desconocido",
                Fiscales = new List<Fiscale>
        {
            new Fiscale
            {
                IdPersonaFiscal = IdPersonaFiscal,
                FechaAsignacion = DateOnly.FromDateTime(DateTime.Today)
            }
        }
            };
            _context.Denuncias.Add(nuevaDenuncia);
            await _context.SaveChangesAsync();

            // 2) Crear Juicio con la nueva denuncia
            long maxJuicioId = await _context.Juicios.MaxAsync(x => (long?)x.IdJuicio) ?? 0;
            j.IdJuicio = maxJuicioId + 1;
            j.IdDenuncia = nuevaDenuncia.IdDenuncia;
            _context.Juicios.Add(j);
            await _context.SaveChangesAsync();

            // 3) Agregar acusado
            _context.JuiciosAcusados.Add(new JuiciosAcusado
            {
                IdJuicio = j.IdJuicio,
                IdPersona = IdPersonaAcusado
            });

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Juicios));
        }



        // POST: Editar
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            long id,
            [Bind("IdJuicio,FechaInicio,FechaFin,IdDenuncia,IdPersonaJuez,Estado")] Juicio j,
            long IdDelito,
            long IdPersonaDenuncia,
            long IdPersonaFiscal,
            long IdPersonaAcusado)
        {
            if (id != j.IdJuicio) return NotFound();
            if (!ModelState.IsValid) return RedirectToAction(nameof(Juicios));

            // 1) Actualiza el juicio (ya enlazado con IdDenuncia)
            _context.Update(j);
            await _context.SaveChangesAsync();

            // 2) Actualiza la denuncia asociada
            var denuncia = await _context.Denuncias
                .Include(d => d.Fiscales)
                .FirstOrDefaultAsync(d => d.IdDenuncia == j.IdDenuncia);

            if (denuncia != null)
            {
                denuncia.IdDelito = IdDelito;
                denuncia.IdPersonaDenuncia = IdPersonaDenuncia;

                // Reemplazar fiscal
                denuncia.Fiscales.Clear();
                denuncia.Fiscales.Add(new Fiscale
                {
                    IdDenuncia = denuncia.IdDenuncia,
                    IdPersonaFiscal = IdPersonaFiscal,
                    FechaAsignacion = DateOnly.FromDateTime(DateTime.Today)
                });

                _context.Denuncias.Update(denuncia);
            }

            // 3) Reemplazar acusado
            var antiguosAcusados = _context.JuiciosAcusados.Where(x => x.IdJuicio == j.IdJuicio);
            _context.JuiciosAcusados.RemoveRange(antiguosAcusados);
            _context.JuiciosAcusados.Add(new JuiciosAcusado
            {
                IdJuicio = j.IdJuicio,
                IdPersona = IdPersonaAcusado
            });

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Juicios));
        }





        // POST: Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(long id)
        {
            var j = await _context.Juicios.FindAsync(id);
            if (j != null)
            {
                _context.Juicios.Remove(j);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Juicios));
        }
    }
}
