using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using SistemaGestionJudicial.Models;

namespace SistemaGestionJudicial.Controllers
{
    public class PersonaController : Controller
    {
        private readonly ProyectoContext _context;

        public PersonaController(ProyectoContext context)
        {
            _context = context;
        }

        // List judges (Jueces)
        public async Task<IActionResult> Jueces()
        {
            var jueces = await _context.Personas
                .Include(p => p.Rol)
                .Where(p => p.IdRol == 1) // Only judges
                .ToListAsync();
            return View(jueces); // Views/Persona/Jueces.cshtml
        }

        // POST: CreateJuez
        [HttpPost]
        public async Task<IActionResult> CreateJuez([Bind("Cedula,Nombres,Apellidos,FechaNacimiento,Genero,Direccion,Telefono,CorreoElectronico")] Persona persona)
        {
            // 🔒 Asignación obligatoria del Rol
            persona.IdRol = 1;

            // ✅ Asignar un nuevo ID único
            long maxId = await _context.Personas.MaxAsync(p => (long?)p.IdPersona) ?? 0;
            persona.IdPersona = maxId + 1;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(persona);
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    // Podrías loguear el error aquí
                    TempData["ErrorMensaje"] = "No se pudo crear el juez. Verifique los datos.";
                }
            }

            return RedirectToAction(nameof(Jueces));
        }




        // POST: Edit
        [HttpPost]
        public async Task<IActionResult> Edit(long IdPersona, [Bind("IdPersona,Cedula,Nombres,Apellidos,FechaNacimiento,Genero,Direccion,Telefono,CorreoElectronico")] Persona persona)
        {
            if (IdPersona != persona.IdPersona)
                return NotFound();

            // Fetch the existing persona (attached)
            var personaDb = await _context.Personas.FindAsync(IdPersona);
            if (personaDb == null || personaDb.IdRol != 1)
                return NotFound();

            // Update fields manually
            personaDb.Cedula = persona.Cedula;
            personaDb.Nombres = persona.Nombres;
            personaDb.Apellidos = persona.Apellidos;
            personaDb.FechaNacimiento = persona.FechaNacimiento;
            personaDb.Genero = persona.Genero;
            personaDb.Direccion = persona.Direccion;
            personaDb.Telefono = persona.Telefono;
            personaDb.CorreoElectronico = persona.CorreoElectronico;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Jueces));
        }


        // POST: Delete
        [HttpPost]
        public async Task<IActionResult> Delete(long IdPersona)
        {
            var persona = await _context.Personas.FindAsync(IdPersona);
            string mensaje = null;

            if (persona != null && persona.IdRol == 1)
            {
                try
                {
                    _context.Personas.Remove(persona);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException ex)
                {
                    // Puedes filtrar más el error si lo deseas, pero así es universal
                    mensaje = "No se puede eliminar el juez porque tiene datos relacionados en el sistema.";
                }
            }

            // Redirige pasando el mensaje como TempData (dura una sola redirección)
            if (mensaje != null)
                TempData["ErrorMensaje"] = mensaje;

            return RedirectToAction(nameof(Jueces));
        }



    }
}
