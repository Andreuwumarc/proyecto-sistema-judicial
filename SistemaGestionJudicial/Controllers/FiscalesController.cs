using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
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
            // Validaciones
            if (string.IsNullOrWhiteSpace(addcedula) || !long.TryParse(addcedula, out _) || addcedula.Length != 10)
            {
                TempData["ErrorMessage"] = "La cédula debe contener 10 números.";
                return RedirectToAction(nameof(Fiscales));
            }

            if (await _context.Personas.AnyAsync(p => p.Cedula == addcedula))
            {
                TempData["ErrorMessage"] = "La cédula ya está registrada.";
                return RedirectToAction(nameof(Fiscales));
            }

            if (string.IsNullOrWhiteSpace(addnombres))
            {
                TempData["ErrorMessage"] = "El campo nombres es obligatorio.";
                return RedirectToAction(nameof(Fiscales));
            }

            if (string.IsNullOrWhiteSpace(addapellidos))
            {
                TempData["ErrorMessage"] = "El campo apellidos es obligatorio.";
                return RedirectToAction(nameof(Fiscales));
            }

            if (string.IsNullOrWhiteSpace(addcorreoelectronico))
            {
                TempData["ErrorMessage"] = "El campo del correo electrónico es obligatorio.";
                return RedirectToAction(nameof(Fiscales));
            }

            if (addnacimiento.HasValue && addnacimiento.Value > DateTime.Now)
            {
                TempData["ErrorMessage"] = "La fecha de nacimiento no puede ser futura.";
                return RedirectToAction(nameof(Fiscales));
            }

            if (!string.IsNullOrWhiteSpace(addtelefono) && (!long.TryParse(addtelefono, out _)))
            {
                TempData["ErrorMessage"] = "El teléfono debe contener solo números.";
                return RedirectToAction(nameof(Fiscales));
            }

            if (addnacimiento.HasValue && addfechadenuncia.HasValue && addfechadenuncia.Value < addnacimiento.Value)
            {
                TempData["ErrorMessage"] = "La fecha de asignación no puede ser anterior a la fecha de nacimiento.";
                return RedirectToAction(nameof(Fiscales));
            }

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
        [Route("Fiscales/EditarFiscal")]
        public async Task<IActionResult> EditarFiscal(long editidF, long editidP, string editcedula, string editnombres, string editapellidos,
            DateTime? editnacimiento, string editgenero, string editdireccion, string edittelefono, string editcorreo, long editiddenuncia,
            DateTime? editfechadenuncia)
        {
            if (editidF <= 0 || editidP <= 0)
            {
                return NotFound();
            }

            var fiscal = await _context.Fiscales.FindAsync(editidF);
            var persona = await _context.Personas.FindAsync(editidP);

            if (fiscal == null || persona == null)
            {
                return NotFound();
            }

            // Validaciones
            if (string.IsNullOrWhiteSpace(editcedula) || !long.TryParse(editcedula, out _) || editcedula.Length != 10)
            {
                TempData["ErrorMessage"] = "La cédula debe contener 10 números.";
                return RedirectToAction(nameof(Fiscales));
            }

            var existingPersona = await _context.Personas.FirstOrDefaultAsync(p => p.Cedula == editcedula && p.IdPersona != editidP);
            if (existingPersona != null)
            {
                TempData["ErrorMessage"] = "La cédula ya está registrada por otra persona.";
                return RedirectToAction(nameof(Fiscales));
            }

            if (string.IsNullOrWhiteSpace(editnombres))
            {
                TempData["ErrorMessage"] = "El campo nombres es obligatorio.";
                return RedirectToAction(nameof(Fiscales));
            }

            if (string.IsNullOrWhiteSpace(editapellidos))
            {
                TempData["ErrorMessage"] = "El campo apellidos es obligatorio.";
                return RedirectToAction(nameof(Fiscales));
            }

            if (string.IsNullOrWhiteSpace(editcorreo))
            {
                TempData["ErrorMessage"] = "El campo del correo electrónico es obligatorio.";
                return RedirectToAction(nameof(Fiscales));
            }

            if (editnacimiento.HasValue && editnacimiento.Value > DateTime.Now)
            {
                TempData["ErrorMessage"] = "La fecha de nacimiento no puede ser futura.";
                return RedirectToAction(nameof(Fiscales));
            }

            if (!string.IsNullOrWhiteSpace(edittelefono) && (!long.TryParse(edittelefono, out _)))
            {
                TempData["ErrorMessage"] = "El teléfono debe contener solo números.";
                return RedirectToAction(nameof(Fiscales));
            }

            if (editnacimiento.HasValue && editfechadenuncia.HasValue && editfechadenuncia.Value < editnacimiento.Value)
            {
                TempData["ErrorMessage"] = "La fecha de asignación no puede ser anterior a la fecha de nacimiento.";
                return RedirectToAction(nameof(Fiscales));
            }

            persona.Cedula = editcedula;
            persona.Nombres = editnombres;
            persona.Apellidos = editapellidos;
            persona.FechaNacimiento = editnacimiento.HasValue ? DateOnly.FromDateTime(editnacimiento.Value) : null;
            persona.Genero = editgenero;
            persona.Direccion = editdireccion;
            persona.Telefono = edittelefono;
            persona.CorreoElectronico = editcorreo;

            fiscal.IdDenuncia = editiddenuncia;
            fiscal.FechaAsignacion = editfechadenuncia.HasValue ? DateOnly.FromDateTime(editfechadenuncia.Value) : null;


            await _context.SaveChangesAsync();

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
            try
            {
                // Buscar el registro en fiscales
                var fiscal = await _context.Fiscales
                    .FirstOrDefaultAsync(f => f.IdPersonaFiscal == deleteidF);

                if (fiscal == null)
                {
                    TempData["ErrorMessage"] = "El fiscal no fue encontrado.";
                    return RedirectToAction(nameof(Fiscales));
                }

                // Eliminar el registro en fiscales
                _context.Fiscales.Remove(fiscal);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "El fiscal fue eliminado exitosamente.";

                // Verificar si id_persona está siendo usado por otros registros en fiscales
                var otherFiscalExists = await _context.Fiscales
                    .AnyAsync(f => f.IdPersonaFiscal == deleteidP && f.IdPersonaFiscal != deleteidF);

                if (otherFiscalExists)
                {
                    // Si hay otros fiscales asociados a esta persona, no eliminar la persona
                    TempData["WarningMessage"] = "No se eliminó la persona porque está asociada a otros fiscales.";
                    return RedirectToAction(nameof(Fiscales));
                }

                // Si no hay otros fiscales asociados, eliminar la persona
                var persona = await _context.Personas
                    .FirstOrDefaultAsync(p => p.IdPersona == deleteidP);

                if (persona != null)
                {
                    _context.Personas.Remove(persona);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "El fiscal y la persona fueron eliminados exitosamente.";
                }

                return RedirectToAction(nameof(Fiscales));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Ocurrió un error al intentar eliminar el fiscal o la persona. Por favor, intenta de nuevo.";
                return RedirectToAction(nameof(Fiscales));
            }
        }
    }
}