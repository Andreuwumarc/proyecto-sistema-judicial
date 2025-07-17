using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using SistemaGestionJudicial.Models;

namespace SistemaGestionJudicial.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly ProyectoContext _context;

        public UsuarioController(ProyectoContext context)
        {
            _context = context;
        }

        // Listado de usuarios
        public async Task<IActionResult> Index()
        {
            var usuarios = await _context.Usuarios
                .Include(u => u.Persona)
                .ToListAsync();
            return View(usuarios); // Views/Usuario/Index.cshtml
        }

        // Detalle de usuario
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
                return NotFound();

            var usuario = await _context.Usuarios
                .Include(u => u.Persona)
                .FirstOrDefaultAsync(u => u.IdUsuario == id);

            if (usuario == null)
                return NotFound();

            return View(usuario);
        }

        // Crear usuario (GET)
        public IActionResult Create()
        {
            // Aquí puedes cargar un listado de personas para asociar
            ViewBag.Personas = _context.Personas.ToList();
            return View();
        }

        // Crear usuario (POST)
        [HttpPost]
        public async Task<IActionResult> Create([Bind("Usuario,Contrasena,IdPersona")] Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                _context.Add(usuario);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Personas = _context.Personas.ToList();
            return View(usuario);
        }

        // Editar usuario (GET)
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
                return NotFound();

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
                return NotFound();

            ViewBag.Personas = _context.Personas.ToList();
            return View(usuario);
        }

        // Editar usuario (POST)
        [HttpPost]
        public async Task<IActionResult> Edit(long id, [Bind("IdUsuario,Usuario,Contrasena,IdPersona")] Usuario usuario)
        {
            if (id != usuario.IdUsuario)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(usuario);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Usuarios.Any(u => u.IdUsuario == usuario.IdUsuario))
                        return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Personas = _context.Personas.ToList();
            return View(usuario);
        }

        // Eliminar usuario (GET)
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
                return NotFound();

            var usuario = await _context.Usuarios
                .Include(u => u.Persona)
                .FirstOrDefaultAsync(u => u.IdUsuario == id);
            if (usuario == null)
                return NotFound();

            return View(usuario);
        }

        // Eliminar usuario (POST)
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                _context.Usuarios.Remove(usuario);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // UTILIDAD: Comprueba si una persona tiene usuario asociado
        public bool PersonaTieneUsuario(long idPersona)
        {
            return _context.Usuarios.Any(u => u.IdPersona == idPersona);
        }
    }
}
