// Ruta: SistemaGestionJudicial/Controllers/DelincuentesController.cs

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaGestionJudicial.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic; // Aunque no para Sentencia, puede ser útil

namespace SistemaGestionJudicial.Controllers
{
    public class DelincuentesController : Controller
    {
        private readonly ProyectoContext _context;

        public DelincuentesController(ProyectoContext context)
        {
            _context = context;
        }

        // GET: Delincuentes - Muestra la lista de delincuentes
        public async Task<IActionResult> Index()
        {
            var delincuentes = await _context.Personas
                                             .Where(p => p.IdRol == 3)
                                             .ToListAsync();

            return View("~/Views/Home/Delincuentes.cshtml", delincuentes);
        }

        // GET: Delincuentes/GetDelincuenteDetails/5 - Obtiene detalles para el modal de vista,
        // incluyendo juicios y delitos asociados.
        [HttpGet]
        public async Task<IActionResult> GetDelincuenteDetails(int id)
        {
            var persona = await _context.Personas
                                        .Where(p => p.IdPersona == id && p.IdRol == 3)
                                        // Incluimos las relaciones para obtener la información completa:
                                        .Include(p => p.JuiciosAcusados)
                                            .ThenInclude(ja => ja.IdJuicioNavigation)
                                                .ThenInclude(j => j.IdDenunciaNavigation)
                                                    .ThenInclude(d => d.IdDelitoNavigation)
                                        // SE ELIMINA LA INCLUSIÓN DE SENTENCIA POR EL ERROR
                                        // .Include(p => p.JuiciosAcusados)
                                        //    .ThenInclude(ja => ja.IdJuicioNavigation)
                                        //        .ThenInclude(j => j.Sentencia)
                                        .FirstOrDefaultAsync();

            if (persona == null)
            {
                return NotFound();
            }

            // Mapeamos los juicios y delitos para enviarlos como un objeto JSON
            var juiciosYDelitos = persona.JuiciosAcusados
                .Select(ja => new
                {
                    idJuicioAcusado = ja.IdJuicioAcusado, // Incluido si lo necesitas en la vista

                    idJuicio = ja.IdJuicioNavigation?.IdJuicio,
                    estadoJuicio = ja.IdJuicioNavigation?.Estado,
                    fechaInicioJuicio = ja.IdJuicioNavigation?.FechaInicio?.ToString("yyyy-MM-dd"),
                    fechaFinJuicio = ja.IdJuicioNavigation?.FechaFin?.ToString("yyyy-MM-dd"),

                    // Información del Delito
                    nombreDelito = ja.IdJuicioNavigation?.IdDenunciaNavigation?.IdDelitoNavigation?.Nombre,
                    tipoDelito = ja.IdJuicioNavigation?.IdDenunciaNavigation?.IdDelitoNavigation?.TipoDelito,
                    descripcionDelito = ja.IdJuicioNavigation?.IdDenunciaNavigation?.IdDelitoNavigation?.Descripcion,
                    gravedadDelito = ja.IdJuicioNavigation?.IdDenunciaNavigation?.IdDelitoNavigation?.GravedadDelito,

                    // SE ELIMINA LA PROYECCIÓN DE SENTENCIAS POR EL ERROR
                    // sentencias = (ja.IdJuicioNavigation?.Sentencia as IEnumerable<Sentencia>)
                    //                ?.Select(s => new
                    //                {
                    //                    idSentencia = s.IdSentencia,
                    //                    fechaSentencia = s.FechaSentencia?.ToString("yyyy-MM-dd"),
                    //                    tipoSentencia = s.TipoSentencia,
                    //                    pena = s.Pena,
                    //                    observaciones = s.Observaciones
                    //                }).ToList() ?? new List<object>()
                })
                .ToList();

            // Retorna un objeto JSON con todos los detalles del delincuente,
            // incluyendo la lista de juicios y sus datos asociados.
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

        // POST: Delincuentes/Create - Agrega un nuevo delincuente (recibe datos de formulario vía AJAX)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nombres,Apellidos,FechaNacimiento,Genero,Cedula,Direccion,Telefono,CorreoElectronico")] Persona persona)
        {
            persona.IdRol = 3;

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

            var existingPersona = await _context.Personas.AsNoTracking().FirstOrDefaultAsync(p => p.IdPersona == id);
            if (existingPersona == null || existingPersona.IdRol != 3)
            {
                return Json(new { success = false, message = "Delincuente no encontrado o rol inválido." });
            }
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
                        return NotFound();
                    }
                    else
                    {
                        throw;
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
            var persona = await _context.Personas.FindAsync(id);

            if (persona == null)
            {
                return NotFound();
            }

            if (persona.IdRol != 3)
            {
                return Json(new { success = false, message = "No se puede eliminar, la persona no es un delincuente." });
            }

            _context.Personas.Remove(persona);
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Delincuente eliminado correctamente." });
        }

        // Acción para recargar solo la tabla (usada por AJAX después de agregar/editar/eliminar)
        public async Task<IActionResult> IndexPartial()
        {
            var delincuentes = await _context.Personas
                                     .Where(p => p.IdRol == 3)
                                     .ToListAsync();
            return PartialView("~/Views/Home/_DelincuentesTableBody.cshtml", delincuentes);
        }
    }
}