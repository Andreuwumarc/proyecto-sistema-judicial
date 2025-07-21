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
    public class FiscalesController : Controller
    {
        private readonly ProyectoContext _context;

        public FiscalesController(ProyectoContext context)
        {
            _context = context;
        }

        // GET: Fiscales

        public async Task<IActionResult> Fiscales()
        {
            var fiscales = _context.Fiscales
            .Include(f => f.IdPersonaFiscalNavigation)
            .Include(f => f.IdDenunciaNavigation)
            .Include(f => f.IdDenunciaNavigation.IdDelitoNavigation)
            .ToList();

            var denuncias = _context.Denuncias
                .Select(d => new { iddenuncia = d.IdDenuncia, DenunciaInfo = "Denuncia #" + d.IdDenuncia + " - " + d.Descripcion })
                .ToList();
            ViewBag.denuncias = denuncias;

            //var proyectoContext = _context.Fiscales.Include(f => f.IdDenunciaNavigation).Include(f => f.IdPersonaFiscalNavigation);

            return View("~/Views/Home/Fiscales.cshtml", fiscales);
        }


        // GET: Fiscales/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fiscales = await _context.Fiscales
                .Include(f => f.IdDenunciaNavigation)
                .Include(f => f.IdPersonaFiscalNavigation)
                .FirstOrDefaultAsync(m => m.IdFiscal == id);
            if (fiscales == null)
            {
                return NotFound();
            }


            return View(fiscales);
        }

        // GET: Fiscales/Create
        [HttpGet]
        public IActionResult Create()
        {
            var denuncias = _context.Denuncias
                .Select(d => new { iddenuncia = d.IdDenuncia, DenunciaInfo = "Denuncia #" + d.IdDenuncia + " - " + d.Descripcion })
                .ToList();
            ViewBag.denuncias = denuncias;

            return View();
        }

        /*public IActionResult Create()
		{
			ViewData["IdDenuncia"] = new SelectList(_context.Denuncias, "IdDenuncia", "IdDenuncia");
			ViewData["IdPersonaFiscal"] = new SelectList(_context.Personas, "IdPersona", "IdPersona");
			return View();
		}*/

        // POST: Fiscales/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateFiscal(string addcedula, string addnombres, string addapellidos, DateTime? addnacimiento,
            string addgenero, string adddireccion, string addtelefono, string addcorreoelectronico, long IdDenuncia, DateTime? addfechadenuncia)
        {
            var persona = new Persona
            {
                Cedula = addcedula,
                Nombres = addnombres,
                Apellidos = addapellidos,
                FechaNacimiento = addnacimiento.HasValue ? DateOnly.FromDateTime(addnacimiento.Value) : null,
                Genero = addgenero,
                Direccion = adddireccion,
                Telefono = addtelefono,
                CorreoElectronico = addcorreoelectronico,
                IdRol = 2
            };

            long maxId1 = await _context.Personas.MaxAsync(p => (long?)p.IdPersona) ?? 0;
            persona.IdPersona = maxId1 + 1;

            var fiscal = new Fiscale
            {
                IdPersonaFiscal = persona.IdPersona,
                IdDenuncia = IdDenuncia,
                FechaAsignacion = addfechadenuncia.HasValue ? DateOnly.FromDateTime(addfechadenuncia.Value) : null
            };

            long maxId2 = await _context.Fiscales.MaxAsync(f => (long?)f.IdFiscal) ?? 0;
            fiscal.IdFiscal = maxId2 + 1;

            _context.Personas.Add(persona);
            _context.Fiscales.Add(fiscal);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Fiscales));
        }

        // GET: Fiscales/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fiscales = await _context.Fiscales.FindAsync(id);
            if (fiscales == null)
            {
                return NotFound();
            }

            var denuncias = _context.Denuncias
                .Select(d => new { iddenuncia = d.IdDenuncia, DenunciaInfo = "Denuncia #" + d.IdDenuncia + " - " + d.Descripcion })
                .ToList();
            ViewBag.denuncias = denuncias;

            ViewData["IdDenuncia"] = new SelectList(_context.Denuncias, "IdDenuncia", "IdDenuncia", fiscales.IdDenuncia);
            ViewData["IdPersonaFiscal"] = new SelectList(_context.Personas, "IdPersona", "IdPersona", fiscales.IdPersonaFiscal);
            return View(fiscales);

        }

        // POST: Fiscales/Edit/5
        //Editar los datos del fiscal de la tabla Fiscal y Persona
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditFiscal(long editidF, long editidP, string editcedula, string editnombres, string editapellidos,
            DateTime? editnacimiento, string editgenero, string editdireccion, string edittelefono, string editcorreo, long editiddenuncia,
            DateTime? editfechadenuncia)
        {
            var fiscal = await _context.Fiscales.FindAsync(editidF);
            var persona = await _context.Personas.FindAsync(editidP);

            if (persona == null || fiscal == null)
                return NotFound();

            persona.Cedula = editcedula;
            persona.Nombres = editnombres;
            persona.Apellidos = editapellidos;
            persona.FechaNacimiento = editnacimiento.HasValue ? DateOnly.FromDateTime(editnacimiento.Value) : null;
            persona.Genero = editgenero;
            persona.Direccion = editdireccion;
            persona.Telefono = edittelefono;
            persona.CorreoElectronico = editcorreo;

            fiscal.IdDenuncia = editiddenuncia;
            if (editfechadenuncia.HasValue)
                fiscal.FechaAsignacion = DateOnly.FromDateTime(editfechadenuncia.Value);

            await _context.SaveChangesAsync();

            var denuncias = _context.Denuncias
                .Select(d => new { iddenuncia = d.IdDenuncia, DenunciaInfo = "Denuncia #" + d.IdDenuncia + " - " + d.Descripcion })
                .ToList();
            ViewBag.denuncias = denuncias;

            return RedirectToAction(nameof(Fiscales));
        }

        // GET: Fiscales/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fiscales = await _context.Fiscales
                .Include(f => f.IdDenunciaNavigation)
                .Include(f => f.IdPersonaFiscalNavigation)
                .FirstOrDefaultAsync(m => m.IdFiscal == id);
            if (fiscales == null)
            {
                return NotFound();
            }

            return View(fiscales);
        }

        //POST: Fiscales/Delete/5
        //Borrar el Fiscal de la tabla Fiscal y Persona
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long deleteidF, long deleteidP)
        {
            var fiscales = await _context.Fiscales.FindAsync(deleteidF);
            if (fiscales != null)
            {
                _context.Fiscales.Remove(fiscales);
                await _context.SaveChangesAsync();
            }

            var personas = await _context.Personas.FindAsync(deleteidP);
            if (personas != null)
            {
                _context.Personas.Remove(personas);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Fiscales));
        }
    }
}
