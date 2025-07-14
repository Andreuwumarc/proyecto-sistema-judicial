using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaGestionJudicial.Context;
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

        public async Task<IActionResult> Polices()
        {
            var partes = await _context.PartesPoliciales
                .Include(p => p.PersonaPolicia)
                .Include(p => p.Denuncia)
                .ToListAsync();

            return View("~/Views/Home/Police/Polices.cshtml", partes);
        }

        // ========== CREATE ==========
        //[HttpGet]
        //public IActionResult Create()
        //{
        //    return View();
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create(PartePolicial parte)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.PartesPoliciales.Add(parte);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Polices));
        //    }
        //    return View("~/Views/Home/Police/Polices.cshtml", parte);
        //}

        //// ========== EDIT ==========
        //[HttpGet]
        //public async Task<IActionResult> Edit(long id)
        //{
        //    var parte = await _context.PartesPoliciales.FindAsync(id);
        //    if (parte == null)
        //    {
        //        return NotFound();
        //    }
        //    return View("~/Views/Home/Police/Polices.cshtml", parte); ;
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(long id, PartePolicial parte)
        //{
        //    if (id != parte.Id_Parte)
        //    {
        //        return BadRequest();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        _context.Update(parte);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Polices));
        //    }

        //    return View("~/Views/Home/Police/Polices.cshtml", parte);
        //}

        //// ========== DELETE ==========
        //[HttpGet]
        //public async Task<IActionResult> Delete(long id)
        //{
        //    var parte = await _context.PartesPoliciales
        //        .Include(p => p.PersonaPolicia)
        //        .Include(p => p.Denuncia)
        //        .FirstOrDefaultAsync(p => p.Id_Parte == id);

        //    if (parte == null)
        //    {
        //        return NotFound();
        //    }

        //    return View("~/Views/Home/Police/Polices.cshtml", parte);
        //}

        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(long id)
        //{
        //    var parte = await _context.PartesPoliciales.FindAsync(id);
        //    if (parte != null)
        //    {
        //        _context.PartesPoliciales.Remove(parte);
        //        await _context.SaveChangesAsync();
        //    }
        //    return RedirectToAction(nameof(Polices));
        //}

        // ========== CREATE ==========
        //[HttpGet]
        //public IActionResult Create()
        //{
        //    // Personas: todas
        //    var personas = _context.Personas
        //        .Select(p => new {
        //            p.Id_Persona,
        //            NombreCompleto = p.Nombres + " " + p.Apellidos + " - " + p.Cedula
        //        })
        //        .ToList();
        //    ViewBag.Personas = new SelectList(personas, "Id_Persona", "NombreCompleto");

        //    // Denuncias: mostrar descripción y cédula de quien denunció
        //    var denuncias = _context.Denuncias
        //              .Select (d => new {d.Id_Denuncia,
        //                  Texto = d.Descripcion
        //              })
        //        .ToList();
        //    ViewBag.Denuncias = new SelectList(denuncias, "Id_Denuncia", "Texto");

        //    return View("~/Views/Home/Police/Create.cshtml");



        // ========== EDIT ==========

        //[HttpGet]
        //public async Task<IActionResult> Edit(long id)
        //{
        //    var parte = await _context.PartesPoliciales.FindAsync(id);
        //    if (parte == null)
        //    {
        //        return NotFound();
        //    }

        //    // Lista de policías (personas)
        //    var personas = _context.Personas
        //        .Select(p => new
        //        {
        //            p.Id_Persona,
        //            NombreCompleto = p.Nombres + " " + p.Apellidos
        //        })
        //        .ToList();
        //    ViewBag.Personas = new SelectList(personas, "Id_Persona", "NombreCompleto", parte.Id_Persona_Policia);

        //    // Lista de denuncias (solo descripción)
        //    var denuncias = _context.Denuncias
        //        .Select(d => new
        //        {
        //            d.Id_Denuncia,
        //            Texto = d.Descripcion
        //        })
        //        .ToList();
        //    ViewBag.Denuncias = new SelectList(denuncias, "Id_Denuncia", "Texto", parte.Id_Denuncia);

        //    return View("~/Views/Home/Police/Edit.cshtml", parte);
        //}

        // GET: Police/Create
        [HttpGet]
        public IActionResult Create()
        {
            // Personas: todas
            var personas = _context.Personas
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PartePolicial parte)
        {
            if (ModelState.IsValid)
            {
                _context.Add(parte);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Polices)); // Asegúrate de tener esta acción o cámbiala por Index()
            }

            // Volver a cargar combos si hay error en el formulario
            var personas = _context.Personas
                .Select(p => new {
                    p.Id_Persona,
                    NombreCompleto = p.Nombres + " " + p.Apellidos + " - " + p.Cedula
                })
                .ToList();
            ViewBag.Personas = new SelectList(personas, "Id_Persona", "NombreCompleto", parte.Id_Persona_Policia);

            var denuncias = _context.Denuncias
                .Select(d => new {
                    d.Id_Denuncia,
                    Texto = d.Descripcion
                })
                .ToList();
            ViewBag.Denuncias = new SelectList(denuncias, "Id_Denuncia", "Texto", parte.Id_Denuncia);

            return View("~/Views/Home/Police/Create.cshtml", parte);
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, PartePolicial parte)
        {
            if (id != parte.Id_Parte)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                _context.Update(parte);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Polices));
            }

            // Si hay error de validación, volver a cargar las listas
            var personas = _context.Personas
                .Select(p => new
                {
                    p.Id_Persona,
                    NombreCompleto = p.Nombres + " " + p.Apellidos
                })
                .ToList();
            ViewBag.Personas = new SelectList(personas, "Id_Persona", "NombreCompleto", parte.Id_Persona_Policia);

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


        //[HttpGet]
        //public async Task<IActionResult> Edit(long id)
        //{
        //    var parte = await _context.PartesPoliciales.FindAsync(id);
        //    if (parte == null)
        //    {
        //        return NotFound();
        //    }
        //    return View("~/Views/Home/Police/Edit.cshtml", parte);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(long id, PartePolicial parte)
        //{
        //    if (id != parte.Id_Parte)
        //    {
        //        return BadRequest();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        _context.Update(parte);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Polices));
        //    }
        //    return View("~/Views/Home/Police/Edit.cshtml", parte);
        //}

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
            }
            return RedirectToAction(nameof(Polices));
        }
    }
}
