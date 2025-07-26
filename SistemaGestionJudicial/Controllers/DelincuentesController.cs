// Ruta: SistemaGestionJudicial/Controllers/DelincuentesController.cs

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaGestionJudicial.Models; // Asegúrate de que este namespace sea correcto
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic; // Agregado para List<T>
using System; // Agregado para Exception

namespace SistemaGestionJudicial.Controllers
{
    public class DelincuenteController : Controller
    {
        private readonly ProyectoContext _context;

        public DelincuenteController(ProyectoContext context)

        {
            _context = context;
        }

        // GET: Delincuente
        // Muestra la lista de delincuentes
        public async Task<IActionResult> Index()
        {
            // Filtra solo las personas con IdRol = 3 (asumiendo que es el rol de delincuente)
            var delincuentes = await _context.Personas
                                            .Where(p => p.IdRol == 3)
                                            .ToListAsync();
            // La vista de Index ahora mostrará los mensajes de TempData si existen.
            return View(delincuentes);
        }

        // GET: Delincuente/Details/5
        // Muestra los detalles de un delincuente específico
        public async Task<IActionResult> Details(long? id) // Cambiado a long? para coincidir con IdPersona
        {
            if (id == null)
            {
                // Si el ID es nulo, redirige a Index con un mensaje de error.
                TempData["ErrorMessage"] = "ID de delincuente no especificado para ver detalles.";
                return RedirectToAction(nameof(Index));
            }

            // Carga los detalles del delincuente incluyendo sus juicios, delitos y sentencias
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
                // Si no se encuentra, redirige a Index con un mensaje de error.
                TempData["ErrorMessage"] = "Delincuente no encontrado o no tiene el rol correcto para ver detalles.";
                return RedirectToAction(nameof(Index));
            }

            return View(persona); // Devuelve la vista de detalles con el modelo completo.
        }

        // GET: Delincuente/Create
        // Muestra el formulario para crear un nuevo delincuente
        public IActionResult Create()
        {
            return View();
        }

        // POST: Delincuente/Create
        // Acción para manejar el envío del formulario de creación de delincuentes
        [HttpPost]
        [ValidateAntiForgeryToken]
        // Se utiliza [Bind] para seguridad, solo las propiedades listadas pueden ser sobreescritas por la entrada del cliente.
        // Asegúrate de incluir todas las propiedades que el formulario de creación puede enviar.
        public async Task<IActionResult> Create([Bind("Nombres,Apellidos,Cedula,Direccion,Telefono,CorreoElectronico,FechaNacimiento,Genero")] Persona persona)
        {
            persona.IdRol = 3; // Se asegura que es delincuente

            // Obtener el ID más alto existente en la tabla y sumarle 1
            long maxId = 0;
            if (await _context.Personas.AnyAsync())
            {
                maxId = await _context.Personas.MaxAsync(p => p.IdPersona);
            }
            persona.IdPersona = maxId + 1;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(persona);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"El delincuente '{persona.Nombres} {persona.Apellidos}' ha sido añadido correctamente.";
                    return RedirectToAction(nameof(Index)); // Redirige a la acción Index para ver la lista
                }
                catch (Exception ex)
                {
                    // Manejo de errores de base de datos u otros
                    TempData["ErrorMessage"] = $"Error al añadir el delincuente: {ex.Message}";
                    // Aquí podrías loggear el error completo: _logger.LogError(ex, "Error al crear delincuente.");
                }
            }
            // Si ModelState no es válido, se regresa a la misma vista 'Create'
            TempData["WarningMessage"] = "Por favor, corrige los errores en el formulario para añadir el delincuente.";
            return View(persona); // Vuelve a mostrar el formulario con los errores
        }


        // GET: Delincuente/Edit/5
        // Muestra el formulario para editar un delincuente existente
        [HttpGet]
        public async Task<IActionResult> Edit(long? id) // Cambiado a long?
        {
            if (id == null)
            {
                TempData["ErrorMessage"] = "ID de delincuente no especificado para edición.";
                return RedirectToAction(nameof(Index));
            }

            var persona = await _context.Personas.FindAsync(id);
            if (persona == null || persona.IdRol != 3) // Asegúrate de que sea un delincuente
            {
                TempData["ErrorMessage"] = "Delincuente no encontrado o no tiene el rol correcto para edición.";
                return RedirectToAction(nameof(Index));
            }
            return View(persona);
        }

        // POST: Delincuente/Edit/5
        // Acción para manejar el envío del formulario de edición
        [HttpPost]
        [ValidateAntiForgeryToken]
        // Asegúrate de que IdPersona esté en el Bind para que el modelo se vincule correctamente.
        // Asegúrate de incluir todas las propiedades que el formulario de edición puede enviar.
        public async Task<IActionResult> Edit(long id, [Bind("IdPersona,Nombres,Apellidos,Cedula,Direccion,Telefono,CorreoElectronico,FechaNacimiento,Genero")] Persona persona)
        {
            if (id != persona.IdPersona)
            {
                TempData["ErrorMessage"] = "El ID del delincuente proporcionado no coincide.";
                return RedirectToAction(nameof(Index));
            }

            // Aseguramos que el rol no sea modificado accidentalmente desde el cliente

            persona.IdRol = 3;

            if (ModelState.IsValid)
            {
                try
                {

                    // Es bueno cargar la entidad existente para asegurar que el IdRol y otras propiedades no modificables
                    // se mantengan, y luego actualizar solo las propiedades necesarias.
                    // Para un proyecto universitario, 'Update(persona)' directamente es aceptable si 'persona'
                    // contiene todas las propiedades relevantes y 'IdRol' se fuerza a 3.
                    _context.Update(persona); // Esto adjunta la entidad y marca todas las propiedades como modificadas
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"El delincuente '{persona.Nombres} {persona.Apellidos}' ha sido actualizado correctamente.";
                    return RedirectToAction(nameof(Index)); // Redirige a la lista después de editar
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PersonaExists(persona.IdPersona))
                    {
                        TempData["ErrorMessage"] = "El delincuente que intentas editar ya no existe en la base de datos.";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        throw; // Otro error de concurrencia que deberías manejar o dejar que se propague.
                    }
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Error al actualizar el delincuente: {ex.Message}";
                }
            }
            // Si ModelState no es válido
            TempData["WarningMessage"] = "Por favor, corrige los errores en el formulario para actualizar el delincuente.";
            return View(persona); // Vuelve a mostrar el formulario de edición con los errores
        }

        // GET: Delincuente/Delete/5
        // Muestra la página de confirmación para eliminar un delincuente
        public async Task<IActionResult> Delete(long? id) // Cambiado a long?
        {
            if (id == null)
            {
                TempData["ErrorMessage"] = "ID de delincuente no especificado para eliminar.";
                return RedirectToAction(nameof(Index));
            }

            var persona = await _context.Personas
                                        .FirstOrDefaultAsync(m => m.IdPersona == id && m.IdRol == 3);
            if (persona == null)
            {
                TempData["ErrorMessage"] = "Delincuente no encontrado o no tiene el rol correcto para eliminar.";
                return RedirectToAction(nameof(Index));
            }

            return View(persona); // Devuelve la vista de confirmación de eliminación
        }

        // POST: Delincuente/Delete/5
        // Acción para eliminar un delincuente
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id) // Cambiado a long
        {
            var persona = await _context.Personas.FindAsync(id);
            if (persona != null && persona.IdRol == 3) // Confirma que es un delincuente antes de eliminar
            {
                try
                {
                    _context.Personas.Remove(persona);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"El delincuente '{persona.Nombres} {persona.Apellidos}' ha sido eliminado correctamente.";
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Error al eliminar el delincuente: {ex.Message}";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "No se pudo eliminar: Delincuente no encontrado o no tiene el rol correcto (IdRol=3).";
            }
            return RedirectToAction(nameof(Index)); // Redirige siempre a la lista después de intentar eliminar
        }

        private bool PersonaExists(long id) // Cambiado a long
        {
            return _context.Personas.Any(e => e.IdPersona == id);

        }
    }
}