using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using SistemaGestionJudicial.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaGestionJudicial.Models;

namespace SistemaGestionJudicial.Controllers;

public class PersonaController : Controller
{
    private readonly SistemaJudicialContext _context;

    public PersonaController(SistemaJudicialContext context)
    {
        _context = context;
    }

    // GET: Persona
    public async Task<IActionResult> Index()
    {
        var personas = await _context.Personas
            .Include(p => p.IdRolNavigation)
            .ToListAsync();

        ViewData["Roles"] = await _context.Roles.ToListAsync();

        return View(personas);
    }


    // GET: Persona/Details/5
    public async Task<IActionResult> Details(long? id)
    {
        if (id == null)
            return NotFound();

        var persona = await _context.Personas
            .Include(p => p.IdRolNavigation)
            .FirstOrDefaultAsync(p => p.IdPersona == id);

        if (persona == null)
            return NotFound();

        return View(persona);
    }

    // GET: Persona/Create
    public async Task<IActionResult> Create()
    {
        ViewBag.Roles = await ObtenerRolesSelect();
        return View();
    }

    // POST: Persona/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Persona persona)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Roles = await ObtenerRolesSelect();
            return View(persona);
        }

        var ultimoId = await _context.Personas.MaxAsync(p => (long?)p.IdPersona) ?? 0;
        persona.IdPersona = ultimoId + 1;

        _context.Add(persona);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));

    }

    // GET: Persona/Edit/5
    public async Task<IActionResult> Edit(long? id)
    {
        if (id == null)
            return NotFound();

        var persona = await _context.Personas.FindAsync(id);
        if (persona == null)
            return NotFound();

        ViewBag.Roles = await ObtenerRolesSelect();
        return View(persona);
    }

    // POST: Persona/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Persona persona)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Roles = await ObtenerRolesSelect();
            return View(persona);
        }

        var personaExistente = await _context.Personas.FindAsync(persona.IdPersona);
        if (personaExistente == null)
            return NotFound();

        personaExistente.Cedula = persona.Cedula;
        personaExistente.Nombres = persona.Nombres;
        personaExistente.Apellidos = persona.Apellidos;
        personaExistente.FechaNacimiento = persona.FechaNacimiento;
        personaExistente.IdRol = persona.IdRol;
        personaExistente.Genero = persona.Genero;
        personaExistente.Direccion = persona.Direccion;
        personaExistente.Telefono = persona.Telefono;
        personaExistente.CorreoElectronico = persona.CorreoElectronico;

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }



    //// GET: Persona/Delete/5
    //public async Task<IActionResult> Delete(long? id)
    //{
    //    if (id == null)
    //        return NotFound();

    //    var persona = await _context.Personas
    //        .Include(p => p.IdRolNavigation)
    //        .FirstOrDefaultAsync(p => p.IdPersona == id);

    //    if (persona == null)
    //        return NotFound();

    //    return View(persona);
    //}

    //// POST: Persona/DeleteConfirmed
    //[HttpPost]
    //[ValidateAntiForgeryToken]
    //public async Task<IActionResult> DeleteConfirmed(long id)
    //{
    //    var persona = await _context.Personas.FindAsync(id);
    //    if (persona == null)
    //        return NotFound();

    //    _context.Personas.Remove(persona);
    //    await _context.SaveChangesAsync();

    //    return RedirectToAction(nameof(Index));
    //}

    private async Task<List<SelectListItem>> ObtenerRolesSelect()
    {
        return await _context.Roles
            .Select(r => new SelectListItem
            {
                Value = r.IdRol.ToString(),
                Text = r.Nombre
            }).ToListAsync();
    }

    private async Task<bool> PersonaExists(long id)
    {
        return await _context.Personas.AnyAsync(e => e.IdPersona == id);
    }
}