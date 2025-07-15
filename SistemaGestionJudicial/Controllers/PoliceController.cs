using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaGestionJudicial.Models;
using System.Threading.Tasks;

namespace SistemaGestionJudicial.Controllers
{
    public class PoliceController : Controller
    {
        private readonly SistemaContext _context;

        public PoliceController(SistemaContext context)
        {
            _context = context;
        }

        //public async Task<IActionResult> Polices()
        //{
        //    var partes = await _context.PartesPoliciales
        //        .Include(p => p.PersonaPolicia)
        //        .Include(p => p.Denuncia)
        //        .ToListAsync();

        //    // Cargar ViewBag para el modal de Crear y Editar en la misma vista
        //    var personas = _context.Personas
        //        .Select(p => new
        //        {
        //            p.Id_Persona,
        //            NombreCompleto = p.Nombres + " " + p.Apellidos + " - " + p.Cedula
        //        })
        //        .ToList();
        //    ViewBag.Personas = new SelectList(personas, "Id_Persona", "NombreCompleto");

        //    var denuncias = _context.Denuncias
        //        .Select(d => new
        //        {
        //            d.Id_Denuncia,
        //            Texto = d.Descripcion
        //        })
        //        .ToList();
        //    ViewBag.Denuncias = new SelectList(denuncias, "Id_Denuncia", "Texto");

        //    return View("~/Views/Home/Police/Polices.cshtml", partes);
        //}

        public async Task<IActionResult> Polices()
        {
            var partes = await _context.PartesPoliciales
                .Include(p => p.PersonaPolicia)
                .Include(p => p.Denuncia)
                .ToListAsync();

            // Cargar ViewBag para el modal de Crear y Editar en la misma vista
            var personas = await _context.Personas // Usar await
                .Where(p => p.Id_Persona == 5)
                .Select(p => new
                {
                    p.Id_Persona,
                    NombreCompleto = p.Nombres + " " + p.Apellidos + " - " + p.Cedula
                })
                .ToListAsync(); // Usar ToListAsync
            ViewBag.Personas = new SelectList(personas, "Id_Persona", "NombreCompleto");

            var denuncias = await _context.Denuncias // Usar await
                .Select(d => new
                {
                    d.Id_Denuncia,
                    Texto = d.Descripcion
                })
                .ToListAsync(); // Usar ToListAsync
            ViewBag.Denuncias = new SelectList(denuncias, "Id_Denuncia", "Texto");

            // ===> RUTA EXPLÍCITA A LA VISTA MOVIDA <===
            return View("~/Views/Home/Polices.cshtml", partes);
        }


        // ========== Crear ==========

        // GET: Police/Create


        [HttpGet]
        public IActionResult Create()
        {
            // Personas: todas
            var personas = _context.Personas
                .Where(p => p.Id_Persona == 5)
                .Select(p => new {
                    p.Id_Persona,
                    NombreCompleto = p.Nombres + " " + p.Apellidos + " - " + p.Cedula
                })
                .ToList();
            ViewBag.Personas = new SelectList(personas, "Id_Persona", "NombreCompleto");

            // Denuncias: mostrar descripción y cédula de quien denunció
            var denuncias = _context.Denuncias
                .Select(d => new {
                    d.Id_Denuncia,
                    Texto = d.Descripcion
                })
                .ToList();

            ViewBag.Denuncias = new SelectList(denuncias, "Id_Denuncia", "Texto");

            return View("~/Views/Home/Police/Create.cshtml");
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create(PartePolicial parte)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        // Generar el ID manualmente
        //        var ultimoId = await _context.PartesPoliciales
        //            .OrderByDescending(p => p.Id_Parte)
        //            .Select(p => p.Id_Parte)
        //            .FirstOrDefaultAsync();

        //        parte.Id_Parte = ultimoId + 1;

        //        _context.Add(parte);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Polices));
        //    }

        //    // Volver a cargar combos si hay error en el formulario
        //    var personas = _context.Personas
        //        .Select(p => new {
        //            p.Id_Persona,
        //            NombreCompleto = p.Nombres + " " + p.Apellidos + " - " + p.Cedula
        //        })
        //        .ToList();
        //    ViewBag.Personas = new SelectList(personas, "Id_Persona", "NombreCompleto", parte.Id_Persona_Policia);

        //    var denuncias = _context.Denuncias
        //        .Select(d => new {
        //            d.Id_Denuncia,
        //            Texto = d.Descripcion
        //        })
        //        .ToList();
        //    ViewBag.Denuncias = new SelectList(denuncias, "Id_Denuncia", "Texto", parte.Id_Denuncia);

        //    return View("~/Views/Home/Police/Create.cshtml", parte);
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PartePolicial parte)
        {
            if (ModelState.IsValid)
            {
                var ultimoId = await _context.PartesPoliciales
                    .OrderByDescending(p => p.Id_Parte)
                    .Select(p => p.Id_Parte)
                    .FirstOrDefaultAsync();

                parte.Id_Parte = ultimoId + 1;

                _context.Add(parte);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Parte policial creado exitosamente."; // Agrega feedback
                // ===> REDIRIGE A LA ACCIÓN POLICES EN EL MISMO CONTROLADOR <===
                return RedirectToAction(nameof(Polices));
            }

            TempData["ErrorMessage"] = "Error al crear el parte policial. Verifique los datos."; // Agrega feedback
            // Si la validación falla, redirigimos a la vista principal.
            // Para mostrar errores en el modal sin recargar, se necesita AJAX.
            return RedirectToAction(nameof(Polices));
        }


        // ========== EDIT ==========

        [HttpGet]
        public async Task<IActionResult> Edit(long id)
        {
            var parte = await _context.PartesPoliciales.FindAsync(id);
            if (parte == null)
            {
                return NotFound();
            }

            // Lista de policías (personas)
            var personas = _context.Personas
                .Where(p => p.Id_Persona == 5)
                .Select(p => new
                {
                    p.Id_Persona,
                    NombreCompleto = p.Nombres + " " + p.Apellidos
                })
                .ToList();
            ViewBag.Personas = new SelectList(personas, "Id_Persona", "NombreCompleto", parte.Id_Persona_Policia);

            // Lista de denuncias (solo descripción)
            var denuncias = _context.Denuncias
                .Select(d => new
                {
                    d.Id_Denuncia,
                    Texto = d.Descripcion
                })
                .ToList();
            ViewBag.Denuncias = new SelectList(denuncias, "Id_Denuncia", "Texto", parte.Id_Denuncia);

            return View("~/Views/Home/Police/Edit.cshtml", parte);
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(PartePolicial parte)
        //{
        //    if (parte == null || parte.Id_Parte == 0)
        //    {
        //        return BadRequest();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        _context.Update(parte);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Polices));
        //    }

        //    // Recargar combos si hay error de validación
        //    var personas = _context.Personas
        //        .Select(p => new {
        //            p.Id_Persona,
        //            NombreCompleto = p.Nombres + " " + p.Apellidos
        //        }).ToList();
        //    ViewBag.Personas = new SelectList(personas, "Id_Persona", "NombreCompleto", parte.Id_Persona_Policia);

        //    var denuncias = _context.Denuncias
        //        .Select(d => new {
        //            d.Id_Denuncia,
        //            Texto = d.Descripcion
        //        }).ToList();
        //    ViewBag.Denuncias = new SelectList(denuncias, "Id_Denuncia", "Texto", parte.Id_Denuncia);

        //    return View("~/Views/Home/Police/Edit.cshtml", parte);
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PartePolicial parte)
        {
            if (parte == null || parte.Id_Parte == 0)
            {
                TempData["ErrorMessage"] = "Datos de parte policial inválidos.";
                return RedirectToAction(nameof(Polices));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(parte);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Parte policial actualizado exitosamente.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.PartesPoliciales.Any(e => e.Id_Parte == parte.Id_Parte))
                    {
                        TempData["ErrorMessage"] = "El parte policial que intenta editar no existe.";
                    }
                    else
                    {
                        throw;
                    }
                }
                // ===> REDIRIGE A LA ACCIÓN POLICES EN EL MISMO CONTROLADOR <===
                return RedirectToAction(nameof(Polices));
            }

            TempData["ErrorMessage"] = "Error al actualizar el parte policial. Verifique los datos.";
            return RedirectToAction(nameof(Polices));
        }

        // ========== DELETE ==========
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(long id)
        {
            var parte = await _context.PartesPoliciales.FindAsync(id);
            if (parte != null)
            {
                _context.PartesPoliciales.Remove(parte);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Parte policial eliminado exitosamente.";
            }
            else
            {
                TempData["ErrorMessage"] = "Parte policial no encontrado para eliminar.";
            }
            // ===> REDIRIGE A LA ACCIÓN POLICES EN EL MISMO CONTROLADOR <===
            return RedirectToAction(nameof(Polices));
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Delete(long id)
        //{
        //    var parte = await _context.PartesPoliciales.FindAsync(id);
        //    if (parte != null)
        //    {
        //        _context.PartesPoliciales.Remove(parte);
        //        await _context.SaveChangesAsync();
        //    }
        //    return RedirectToAction(nameof(Polices));
        //}
    }
}
