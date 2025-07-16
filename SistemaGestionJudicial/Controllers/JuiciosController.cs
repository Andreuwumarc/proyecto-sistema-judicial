using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaGestionJudicial.Models;

namespace SistemaGestionJudicial.Controllers
{
    public class JuiciosController : Controller
    {
        private readonly ProyectoContext _context;

        public JuiciosController(ProyectoContext context)
        {
            _context = context;
        }

        // GET: Juicios
        public async Task<IActionResult> Index(long? juezId, long? fiscalId, string estado, string ordenFecha, DateOnly? fechaDesde, DateOnly? fechaHasta)
        {
            // Lista de jueces (Persona asociada al Juez en Juicio)
            ViewData["JuezId"] = new SelectList(
                _context.Personas
                    .Where(p => _context.Juicios.Any(j => j.IdPersonaJuez == p.IdPersona))
                    .Select(p => new { Id = p.IdPersona, NombreCompleto = p.Nombres + " " + p.Apellidos }),
                "Id", "NombreCompleto", juezId
            );

            // Lista de fiscales (Persona asociada al Fiscal vía Fiscale)
            ViewData["FiscalId"] = new SelectList(
                _context.Personas
                    .Where(p => _context.Fiscales.Any(f => f.IdPersonaFiscal == p.IdPersona))
                    .Select(p => new { Id = p.IdPersona, NombreCompleto = p.Nombres + " " + p.Apellidos }),
                "Id", "NombreCompleto", fiscalId
            );

            ViewData["Estados"] = new SelectList(
                new List<string> { "Programado", "En Progreso", "Concluido", "Cancelado" },
                estado
            );

            ViewData["OrdenFecha"] = ordenFecha ?? "";
            ViewData["FechaDesde"] = fechaDesde?.ToString("yyyy-MM-dd");
            ViewData["FechaHasta"] = fechaHasta?.ToString("yyyy-MM-dd");



            // Consulta base con includes
            var consulta = _context.Juicios
                .Include(j => j.IdPersonaJuezNavigation)
                .Include(j => j.IdDenunciaNavigation)
                    .ThenInclude(d => d.IdDelitoNavigation)
                .Include(j => j.IdDenunciaNavigation)
                    .ThenInclude(d => d.IdPersonaDenunciaNavigation)
                .Include(j => j.IdDenunciaNavigation)
                    .ThenInclude(d => d.Fiscales)
                        .ThenInclude(f => f.IdPersonaFiscalNavigation)
                .Include(j => j.JuiciosAcusados)
                    .ThenInclude(a => a.IdPersonaNavigation)
                .Include(j => j.Sentencia)
                .AsQueryable();



            // Filtros
            if (juezId.HasValue)
                consulta = consulta.Where(j => j.IdPersonaJuez == juezId);

            if (fiscalId.HasValue)
                consulta = consulta.Where(j =>
                    _context.Fiscales.Any(f => f.IdDenuncia == j.IdDenuncia && f.IdPersonaFiscal == fiscalId)
                );

            //if (!string.IsNullOrEmpty(estado))
            //{
            //    consulta = consulta.Where(j => j.Estado != null && j.Estado.ToLower() == estado.ToLower());
            //}

            // Ordenamiento por fecha
            if (ordenFecha == "asc")
                consulta = consulta.OrderBy(j => j.FechaInicio);
            else if (ordenFecha == "desc")
                consulta = consulta.OrderByDescending(j => j.FechaInicio);

            // Filtro por rango de fechas
            if (fechaDesde.HasValue)
                consulta = consulta.Where(j => j.FechaInicio >= fechaDesde.Value);

            if (fechaHasta.HasValue)
                consulta = consulta.Where(j => j.FechaInicio <= fechaHasta.Value);


            return View(await consulta.ToListAsync());
        }


        // GET: Juicios/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var juicio = await _context.Juicios
                .Include(j => j.IdDenunciaNavigation)
                .Include(j => j.IdPersonaJuezNavigation)
                .FirstOrDefaultAsync(m => m.IdJuicio == id);
            if (juicio == null)
            {
                return NotFound();
            }

            return View(juicio);
        }

        // GET: Juicios/Create
        public IActionResult Create()
        {
            ViewBag.IdDenuncia = new SelectList(
                _context.Denuncias
                    .Include(d => d.IdPersonaDenunciaNavigation)
                    .Select(d => new {
                        Id = d.IdDenuncia,
                        Texto = "Denuncia #" + d.IdDenuncia + " - " + d.IdPersonaDenunciaNavigation.Nombres + " " + d.IdPersonaDenunciaNavigation.Apellidos
                    }),
                "Id", "Texto"
            );

            ViewBag.IdPersonaJuez = new SelectList(
                _context.Personas.Select(p => new {
                    Id = p.IdPersona,
                    NombreCompleto = p.Nombres + " " + p.Apellidos
                }),
                "Id", "NombreCompleto"
            );

            return View();
        }


        // POST: Juicios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdJuicio,FechaInicio,FechaFin,IdDenuncia,IdPersonaJuez,Estado")] Juicio juicio)
        {
            if (ModelState.IsValid)
            {
                _context.Add(juicio);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdDenuncia"] = new SelectList(_context.Denuncias, "IdDenuncia", "IdDenuncia", juicio.IdDenuncia);
            ViewData["IdPersonaJuez"] = new SelectList(_context.Personas, "IdPersona", "IdPersona", juicio.IdPersonaJuez);
            return View(juicio);
        }

        // GET: Juicios/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var juicio = await _context.Juicios.FindAsync(id);
            if (juicio == null)
            {
                return NotFound();
            }
            ViewData["IdDenuncia"] = new SelectList(_context.Denuncias, "IdDenuncia", "IdDenuncia", juicio.IdDenuncia);
            ViewData["IdPersonaJuez"] = new SelectList(_context.Personas, "IdPersona", "IdPersona", juicio.IdPersonaJuez);
            return View(juicio);
        }

        // POST: Juicios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("IdJuicio,FechaInicio,FechaFin,IdDenuncia,IdPersonaJuez,Estado")] Juicio juicio)
        {
            if (id != juicio.IdJuicio)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(juicio);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!JuicioExists(juicio.IdJuicio))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdDenuncia"] = new SelectList(_context.Denuncias, "IdDenuncia", "IdDenuncia", juicio.IdDenuncia);
            ViewData["IdPersonaJuez"] = new SelectList(_context.Personas, "IdPersona", "IdPersona", juicio.IdPersonaJuez);
            return View(juicio);
        }

        // GET: Juicios/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var juicio = await _context.Juicios
                .Include(j => j.IdDenunciaNavigation)
                .Include(j => j.IdPersonaJuezNavigation)
                .FirstOrDefaultAsync(m => m.IdJuicio == id);
            if (juicio == null)
            {
                return NotFound();
            }

            return View(juicio);
        }

        // POST: Juicios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var juicio = await _context.Juicios.FindAsync(id);
            if (juicio != null)
            {
                _context.Juicios.Remove(juicio);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool JuicioExists(long id)
        {
            return _context.Juicios.Any(e => e.IdJuicio == id);
        }
    }
}