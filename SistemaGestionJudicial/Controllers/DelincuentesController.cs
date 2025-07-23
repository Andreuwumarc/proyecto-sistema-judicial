// Ruta: SistemaGestionJudicial/Controllers/DelincuentesController.cs

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaGestionJudicial.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SistemaGestionJudicial.Controllers
{
    public class DelincuentesController : Controller
    {
        private readonly ProyectoContext _context;

        public DelincuentesController(ProyectoContext context)
        {
            _context = context;
        }

        // Modificamos la acción Index para cargar y pasar el modelo a la vista principal
        public async Task<IActionResult> Index()
        {
            var delincuentes = await _context.Personas
                                             .Where(p => p.IdRol == 3)
                                             .ToListAsync();
            // Devuelve la vista principal (Delincuentes.cshtml) con el modelo de delincuentes
            return View("~/Views/Home/Delincuentes.cshtml", delincuentes);
        }

        // Nueva acción para obtener la lista de delincuentes como JSON, usada por JavaScript
        [HttpGet]
        public async Task<IActionResult> GetDelincuentesList()
        {
            var delincuentes = await _context.Personas
                                             .Where(p => p.IdRol == 3)
                                             .ToListAsync();

            // Proyectamos a un tipo anónimo para enviar solo los datos necesarios al cliente
            return Json(delincuentes.Select(p => new
            {
                idPersona = p.IdPersona,
                cedula = p.Cedula,
                nombres = p.Nombres,
                apellidos = p.Apellidos,
                fechaNacimiento = p.FechaNacimiento?.ToString("yyyy-MM-dd"),
                genero = p.Genero,
                // No incluyas detalles de juicio/delito/sentencia aquí, ya que se obtienen con GetDelincuenteDetails
            }).ToList());
        }

        [HttpGet]
        public async Task<IActionResult> GetDelincuenteDetails(int id)
        {
            var persona = await _context.Personas
                                         .Where(p => p.IdPersona == id && p.IdRol == 3) // Aseguramos que sea un delincuente
                                         .Include(p => p.JuiciosAcusados)
                                             .ThenInclude(ja => ja.IdJuicioNavigation)
                                                 .ThenInclude(j => j.IdDenunciaNavigation)
                                                     .ThenInclude(d => d.IdDelitoNavigation)
                                         .Include(p => p.JuiciosAcusados)
                                             .ThenInclude(ja => ja.IdJuicioNavigation)
                                                 .ThenInclude(j => j.Sentencia) // Aquí 'Sentencia' es tu ICollection<Sentencia>
                                         .FirstOrDefaultAsync();

            if (persona == null)
            {
                return NotFound();
            }

            var juiciosYDelitos = persona.JuiciosAcusados
                .Select(ja => new
                {
                    idJuicioAcusado = ja.IdJuicioAcusado,
                    idJuicio = ja.IdJuicioNavigation?.IdJuicio,
                    estadoJuicio = ja.IdJuicioNavigation?.Estado,
                    fechaInicioJuicio = ja.IdJuicioNavigation?.FechaInicio?.ToString("yyyy-MM-dd"),
                    fechaFinJuicio = ja.IdJuicioNavigation?.FechaFin?.ToString("yyyy-MM-dd"),

                    // Información del Delito
                    nombreDelito = ja.IdJuicioNavigation?.IdDenunciaNavigation?.IdDelitoNavigation?.Nombre,
                    tipoDelito = ja.IdJuicioNavigation?.IdDenunciaNavigation?.IdDelitoNavigation?.TipoDelito,
                    descripcionDelito = ja.IdJuicioNavigation?.IdDenunciaNavigation?.IdDelitoNavigation?.Descripcion,
                    gravedadDelito = ja.IdJuicioNavigation?.IdDenunciaNavigation?.IdDelitoNavigation?.GravedadDelito,

                    // Proyección de la colección de Sentencias
                    sentencias = ja.IdJuicioNavigation?.Sentencia? // Accede a la colección 'Sentencia'
                                   .Select(s => new // Itera sobre cada sentencia en la colección
                                   {
                                       idSentencia = s.IdSentencia,
                                       fechaSentencia = s.FechaSentencia?.ToString("yyyy-MM-dd"),
                                       tipoSentencia = s.TipoSentencia,
                                       pena = s.Pena,
                                       observaciones = s.Observaciones
                                   })
                                   .ToList() // Convierte la selección a una lista
                })
                .ToList();

            return Json(new
            {
                id = persona.IdPersona,
                nombres = persona.Nombres,
                apellidos = persona.Apellidos,
                fechaNacimiento = persona.FechaNacimiento?.ToString("yyyy-MM-dd"),
                genero = persona.Genero,
                cedula = persona.Cedula,
                direccion = persona.Direccion,
                telefono = persona.Telefono,
                correoElectronico = persona.CorreoElectronico,
                juiciosYDelitos = juiciosYDelitos
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nombres,Apellidos,FechaNacimiento,Genero,Cedula,Direccion,Telefono,CorreoElectronico")] Persona persona)
        {
            persona.IdRol = 3; // Asegura que se crea como delincuente

            // Si el IdPersona viene con un valor (ej. 0 por defecto), lo reseteamos a 0 para que la DB le asigne uno.
            // Si tu BD usa IDENTITY para IdPersona, esto podría no ser estrictamente necesario, pero es una buena práctica.
            if (persona.IdPersona != 0)
            {
                persona.IdPersona = 0;
            }

            if (ModelState.IsValid)
            {
                _context.Add(persona);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Delincuente añadido correctamente." });
            }
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            return Json(new { success = false, message = "Errores de validación", errors = errors });
        }


        // POST: Delincuentes/Edit - Actualiza un delincuente existente (recibe datos de formulario vía AJAX)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdPersona,Nombres,Apellidos,FechaNacimiento,Genero,Cedula,Direccion,Telefono,CorreoElectronico")] Persona persona)
        {
            if (id != persona.IdPersona)
            {
                return NotFound();
            }

            // Buscar la persona existente y asegurarse de que sea un delincuente (IdRol = 3)
            var existingPersona = await _context.Personas.AsNoTracking().FirstOrDefaultAsync(p => p.IdPersona == id && p.IdRol == 3);
            if (existingPersona == null)
            {
                // Mensaje más claro si no se encuentra o no es un delincuente válido para editar aquí
                return Json(new { success = false, message = "Delincuente no encontrado o no tiene el rol correcto para ser editado desde esta vista." });
            }

            // Aseguramos que el rol permanezca como 3, ya que esta vista es para delincuentes
            persona.IdRol = 3;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(persona);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "Delincuente actualizado correctamente." });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Personas.Any(e => e.IdPersona == persona.IdPersona))
                    {
                        return NotFound(); // Delincuente ya no existe
                    }
                    else
                    {
                        throw; // Otro error de concurrencia
                    }
                }
            }
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            return Json(new { success = false, message = "Errores de validación", errors = errors });
        }

        // POST: Delincuentes/Delete/5 - Elimina un delincuente (recibe ID vía AJAX)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Busca la persona por ID y asegura que su IdRol sea 3.
            // Si no se encuentra o el rol no es 3, 'persona' será null.
            var persona = await _context.Personas.FirstOrDefaultAsync(p => p.IdPersona == id && p.IdRol == 3);

            if (persona == null)
            {
                // Si la persona no se encontró con el ID y el rol de delincuente,
                // devuelve un mensaje indicando que no se puede eliminar.
                return Json(new { success = false, message = "No se pudo eliminar: Delincuente no encontrado o no tiene el rol correcto (IdRol=3)." });
            }

            // Si la persona fue encontrada y tiene IdRol = 3, procede con la eliminación.
            _context.Personas.Remove(persona);
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Delincuente eliminado correctamente." });
        }
    }
}