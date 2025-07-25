using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaGestionJudicial.Models;

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
            if (juezId.HasValue || fiscalId.HasValue)
            {
                consulta = consulta.Where(j =>
                    (juezId.HasValue && j.IdPersonaJuez == juezId) ||
                    (fiscalId.HasValue && _context.Fiscales.Any(f => f.IdDenuncia == j.IdDenuncia && f.IdPersonaFiscal == fiscalId))
                );
            }


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


        // GET: Juicios/Details/id
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null) return NotFound();

            var juicio = await _context.Juicios
                .Include(j => j.IdDenunciaNavigation)
                    .ThenInclude(d => d.IdDelitoNavigation)
                .Include(j => j.IdDenunciaNavigation)
                    .ThenInclude(d => d.IdPersonaDenunciaNavigation)
                .Include(j => j.IdPersonaJuezNavigation)
                .Include(j => j.Sentencia)
                .Include(j => j.JuiciosAcusados)
                    .ThenInclude(ja => ja.IdPersonaNavigation)
                .Include(j => j.IdDenunciaNavigation.Fiscales)
                    .ThenInclude(f => f.IdPersonaFiscalNavigation)
                .FirstOrDefaultAsync(m => m.IdJuicio == id);

            if (juicio == null) return NotFound();

            return View(juicio);
        }


        // GET: Juicios/Create
        public IActionResult Create()
        {
            CargarDropdownsParaCreate();
            return View();
        }

        private void CargarDropdownsParaCreate()
        {
            ViewBag.IdDenuncia = new SelectList(
                _context.Denuncias.Include(d => d.IdPersonaDenunciaNavigation)
                    .Select(d => new {
                        Id = d.IdDenuncia,
                        Texto = "Denuncia #" + d.IdDenuncia + " - " + d.IdPersonaDenunciaNavigation.Nombres + " " + d.IdPersonaDenunciaNavigation.Apellidos
                    }),
                "Id", "Texto"
            );

            ViewBag.IdPersonaJuez = new SelectList(
                _context.Personas.Where(p => p.IdRol == 1)
                    .Select(p => new {
                        Id = p.IdPersona,
                        NombreCompleto = p.Nombres + " " + p.Apellidos
                    }),
                "Id", "NombreCompleto"
            );

            ViewBag.Fiscales = new SelectList(
                _context.Personas.Where(p => p.IdRol == 2)
                    .Select(p => new {
                        Id = p.IdPersona,
                        NombreCompleto = p.Nombres + " " + p.Apellidos
                    }),
                "Id", "NombreCompleto"
            );

            ViewBag.Acusados = new SelectList(
                _context.Personas.Where(p => p.IdRol == 3)
                    .Select(p => new {
                        Id = p.IdPersona,
                        NombreCompleto = p.Nombres + " " + p.Apellidos
                    }),
                "Id", "NombreCompleto"
            );
        }



        // POST: Juicios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
    [Bind("IdJuicio,FechaInicio,FechaFin,IdDenuncia,IdPersonaJuez,Estado")] Juicio juicio,
    long IdPersonaFiscal,
    long IdPersonaAcusado
)
        {
            if (juicio.Estado == "Concluido" && juicio.FechaFin == null)
            {
                ModelState.AddModelError("FechaFin", "La fecha de fin es obligatoria si el juicio está concluido.");
            }

            if (ModelState.IsValid)
            {
                using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    // 1️⃣ Crear ID de Juicio
                    juicio.IdJuicio = _context.Juicios.Any()
                        ? _context.Juicios.Max(j => j.IdJuicio) + 1
                        : 1;

                    _context.Add(juicio);
                    await _context.SaveChangesAsync();

                    // 2️⃣ Crear ID de Fiscal
                    long fiscalId = _context.Fiscales.Any()
                        ? _context.Fiscales.Max(f => f.IdFiscal) + 1
                        : 1;

                    var fiscal = new Fiscale
                    {
                        IdFiscal = fiscalId, // Asignar ID manual
                        IdDenuncia = juicio.IdDenuncia,
                        IdPersonaFiscal = IdPersonaFiscal,
                        FechaAsignacion = DateOnly.FromDateTime(DateTime.Now)
                    };
                    _context.Fiscales.Add(fiscal);

                    // 3️⃣ Crear ID de JuicioAcusado
                    long juicioAcusadoId = _context.JuiciosAcusados.Any()
                        ? _context.JuiciosAcusados.Max(ja => ja.IdJuicioAcusado) + 1
                        : 1;

                    var juicioAcusado = new JuiciosAcusado
                    {
                        IdJuicioAcusado = juicioAcusadoId, // Asignar ID manual
                        IdJuicio = juicio.IdJuicio,
                        IdPersona = IdPersonaAcusado
                    };
                    _context.JuiciosAcusados.Add(juicioAcusado);

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return RedirectToAction(nameof(Index));
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }

            CargarDropdownsParaCreate();
            return View(juicio);
        }




        // GET: Juicios/Edit/id
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null) return NotFound();

            var juicio = await _context.Juicios
                .Include(j => j.IdDenunciaNavigation)
                    .ThenInclude(d => d.IdPersonaDenunciaNavigation)
                .Include(j => j.IdPersonaJuezNavigation)
                .Include(j => j.JuiciosAcusados)
                .Include(j => j.Sentencia)
                .FirstOrDefaultAsync(j => j.IdJuicio == id);

            if (juicio == null) return NotFound();

            CargarDropdownsParaEdit(juicio);
            return View(juicio);
        }


        private void CargarDropdownsParaEdit(Juicio juicio)
        {
            ViewBag.IdDenuncia = new SelectList(
                _context.Denuncias.Include(d => d.IdPersonaDenunciaNavigation)
                    .Select(d => new {
                        Id = d.IdDenuncia,
                        Texto = "Denuncia #" + d.IdDenuncia + " - " + d.IdPersonaDenunciaNavigation.Nombres + " " + d.IdPersonaDenunciaNavigation.Apellidos
                    }),
                "Id", "Texto",
                juicio.IdDenuncia
            );

            ViewBag.IdPersonaJuez = new SelectList(
                _context.Personas.Where(p => p.IdRol == 1)
                    .Select(p => new {
                        IdPersona = p.IdPersona,
                        NombreCompleto = p.Nombres + " " + p.Apellidos
                    }),
                "IdPersona", "NombreCompleto",
                juicio.IdPersonaJuez
            );

            // Obtener el fiscal actual
            var fiscal = _context.Fiscales.FirstOrDefault(f => f.IdDenuncia == juicio.IdDenuncia);
            var idFiscalSeleccionado = fiscal?.IdPersonaFiscal;

            ViewBag.Fiscales = new SelectList(
                _context.Personas.Where(p => p.IdRol == 2)
                    .Select(p => new { p.IdPersona, NombreCompleto = p.Nombres + " " + p.Apellidos }),
                "IdPersona", "NombreCompleto",
                idFiscalSeleccionado
            );

            // Obtener el acusado actual
            var acusado = juicio.JuiciosAcusados.FirstOrDefault();
            var idAcusadoSeleccionado = acusado?.IdPersona;

            ViewBag.Acusados = new SelectList(
                _context.Personas.Where(p => p.IdRol == 3)
                    .Select(p => new { p.IdPersona, NombreCompleto = p.Nombres + " " + p.Apellidos }),
                "IdPersona", "NombreCompleto",
                idAcusadoSeleccionado
            );

            // Obtener el delito de la denuncia
            var idDelitoSeleccionado = juicio.IdDenunciaNavigation?.IdDelito;

            ViewBag.Delitos = new SelectList(
                _context.Delitos,
                "IdDelito", "Nombre",
                idDelitoSeleccionado
            );

            // Obtener el denunciante
            var idDenunciante = juicio.IdDenunciaNavigation?.IdPersonaDenuncia;

            ViewBag.Denunciantes = new SelectList(
                _context.Personas.Where(p => _context.Denuncias.Select(d => d.IdPersonaDenuncia).Contains(p.IdPersona))
                    .Select(p => new { p.IdPersona, NombreCompleto = p.Nombres + " " + p.Apellidos }),
                "IdPersona", "NombreCompleto",
                idDenunciante
            );

            // Obtener sentencia actual
            var tipoSentencia = juicio.Sentencia.FirstOrDefault()?.TipoSentencia;

            ViewBag.Sentencias = new SelectList(
                new List<string> { "Culpable", "Inocente" },
                tipoSentencia
            );
        }



        // POST: Juicios/Edit/id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id,
            [Bind("IdJuicio,FechaInicio,FechaFin,IdDenuncia,IdPersonaJuez,Estado")] Juicio juicio,
            long IdPersonaFiscal,
            long IdPersonaAcusado,
            long IdDelito,
            long IdPersonaDenunciante,
            string TipoSentencia,
            string Pena,
            string Observaciones)
        {
            if (id != juicio.IdJuicio) return NotFound();

            Pena ??= "-";

            if (Pena.Length > 200)
                ModelState.AddModelError("Pena", "La pena no debe superar los 200 caracteres.");

            Observaciones ??= "-";

            if (Observaciones.Length > 300)
                ModelState.AddModelError("Observaciones", "Las observaciones no deben superar los 300 caracteres.");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(juicio);

                    var sentencia = _context.Sentencias.FirstOrDefault(s => s.IdJuicio == juicio.IdJuicio);
                    if (sentencia != null)
                    {
                        sentencia.TipoSentencia = TipoSentencia;
                        sentencia.Pena = Pena;
                        sentencia.Observaciones = Observaciones;
                    }
                    else
                    {
                        _context.Sentencias.Add(new Sentencia
                        {
                            IdJuicio = juicio.IdJuicio,
                            TipoSentencia = TipoSentencia,
                            Pena = Pena,
                            Observaciones = Observaciones
                        });
                    }


                    var fiscal = _context.Fiscales.FirstOrDefault(f => f.IdDenuncia == juicio.IdDenuncia);
                    if (fiscal != null)
                    {
                        fiscal.IdPersonaFiscal = IdPersonaFiscal;
                    }
                    else
                    {
                        _context.Fiscales.Add(new Fiscale
                        {
                            IdDenuncia = juicio.IdDenuncia,
                            IdPersonaFiscal = IdPersonaFiscal,
                            FechaAsignacion = DateOnly.FromDateTime(DateTime.Now)
                        });
                    }

                    var juicioAcusado = _context.JuiciosAcusados.FirstOrDefault(ja => ja.IdJuicio == juicio.IdJuicio);
                    if (juicioAcusado != null)
                    {
                        juicioAcusado.IdPersona = IdPersonaAcusado;
                    }
                    else
                    {
                        _context.JuiciosAcusados.Add(new JuiciosAcusado
                        {
                            IdJuicio = juicio.IdJuicio,
                            IdPersona = IdPersonaAcusado
                        });
                    }

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!JuicioExists(juicio.IdJuicio)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            CargarDropdownsParaEdit(juicio);
            return View(juicio);
        }

        private bool JuicioExists(long id)
        {
            return _context.Juicios.Any(e => e.IdJuicio == id);
        }



        // GET: Juicios/Delete/id
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var juicio = await _context.Juicios
                .Include(j => j.IdDenunciaNavigation)
                    .ThenInclude(d => d.IdPersonaDenunciaNavigation)
                .Include(j => j.IdDenunciaNavigation)
                    .ThenInclude(d => d.IdDelitoNavigation)
                .Include(j => j.IdDenunciaNavigation.Fiscales)
                    .ThenInclude(f => f.IdPersonaFiscalNavigation)
                .Include(j => j.IdPersonaJuezNavigation)
                .Include(j => j.Sentencia)
                .Include(j => j.JuiciosAcusados)
                    .ThenInclude(ja => ja.IdPersonaNavigation)
                .FirstOrDefaultAsync(m => m.IdJuicio == id);

            if (juicio == null)
            {
                return NotFound();
            }

            return View(juicio);
        }

        // POST: Juicios/Delete/id
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var juicio = await _context.Juicios
                .Include(j => j.JuiciosAcusados)
                .Include(j => j.Sentencia)
                .FirstOrDefaultAsync(m => m.IdJuicio == id);

            if (juicio == null)
            {
                return NotFound();
            }

            // 1. Eliminar sentencias relacionadas
            if (juicio.Sentencia != null && juicio.Sentencia.Any())
            {
                _context.Sentencias.RemoveRange(juicio.Sentencia);
            }

            // 2. Eliminar juicios_acusados relacionados
            if (juicio.JuiciosAcusados != null && juicio.JuiciosAcusados.Any())
            {
                _context.JuiciosAcusados.RemoveRange(juicio.JuiciosAcusados);
            }

            // 3. Eliminar el juicio
            _context.Juicios.Remove(juicio);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}