using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using SistemaGestionJudicial.Models;

namespace SistemaGestionJudicial.Controllers
{
    public class JuecesController : Controller
    {
        private readonly ProyectoContext _context;

        public JuecesController(ProyectoContext context)
        {
            _context = context;
        }

        // -----------------------
        // Métodos de PersonaController (Jueces)
        // -----------------------
        [Route("/Home/Jueces")]
        public IActionResult Jueces()
        {
            var jueces = _context.Personas.Where(p => p.IdRol == 1).ToList();
            return View("~/Views/Home/Jueces.cshtml", jueces);
        }

        /*
        public async Task<IActionResult> Jueces()
        {
            var jueces = await _context.Personas
                .Include(p => p.IdRolNavigation)
                .Where(p => p.IdRol == 1)
                .ToListAsync();
            return View("~/Views/Home/Jueces.cshtml", jueces);
        }*/

        [HttpPost]
        public async Task<IActionResult> CreateJuez([Bind("Cedula,Nombres,Apellidos,FechaNacimiento,Genero,Direccion,Telefono,CorreoElectronico")] Persona persona)
        {
            persona.IdRol = 1;

            long maxId = await _context.Personas.MaxAsync(p => (long?)p.IdPersona) ?? 0;
            persona.IdPersona = maxId + 1;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(persona);
                    await _context.SaveChangesAsync();
                }
                catch (Exception)
                {
                    TempData["ErrorMensaje"] = "No se pudo crear el juez. Verifique los datos.";
                }
            }

            return RedirectToAction(nameof(Jueces));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(long IdPersona, [Bind("IdPersona,Cedula,Nombres,Apellidos,FechaNacimiento,Genero,Direccion,Telefono,CorreoElectronico")] Persona persona)
        {
            if (IdPersona != persona.IdPersona)
                return NotFound();

            var personaDb = await _context.Personas.FindAsync(IdPersona);
            if (personaDb == null || personaDb.IdRol != 1)
                return NotFound();

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
                catch (DbUpdateException)
                {
                    mensaje = "No se puede eliminar el juez porque tiene datos relacionados en el sistema.";
                }
            }

            if (mensaje != null)
                TempData["ErrorMensaje"] = mensaje;

            return RedirectToAction(nameof(Jueces));
        }

        // -----------------------
        // Métodos de RolController
        // -----------------------

        public async Task<IActionResult> IndexRoles()
        {
            var roles = await _context.Roles.ToListAsync();
            return View(roles);
        }

        public async Task<IActionResult> DetailsRol(long? id)
        {
            if (id == null)
                return NotFound();

            var rol = await _context.Roles.FirstOrDefaultAsync(m => m.IdRol == id);
            if (rol == null)
                return NotFound();

            return View(rol);
        }

        public IActionResult CreateRol()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRol([Bind("Nombre")] Role rol)
        {
            if (ModelState.IsValid)
            {
                _context.Add(rol);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(IndexRoles));
            }
            return View(rol);
        }

        public async Task<IActionResult> EditRol(long? id)
        {
            if (id == null)
                return NotFound();

            var rol = await _context.Roles.FindAsync(id);
            if (rol == null)
                return NotFound();

            return View(rol);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRol(long id, [Bind("IdRol,Nombre")] Role rol)
        {
            if (id != rol.IdRol)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(rol);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Roles.Any(e => e.IdRol == rol.IdRol))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(IndexRoles));
            }
            return View(rol);
        }

        public async Task<IActionResult> DeleteRol(long? id)
        {
            if (id == null)
                return NotFound();

            var rol = await _context.Roles.FirstOrDefaultAsync(m => m.IdRol == id);
            if (rol == null)
                return NotFound();

            return View(rol);
        }

        [HttpPost, ActionName("DeleteRol")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmedRol(long id)
        {
            var rol = await _context.Roles.FindAsync(id);
            if (rol != null)
            {
                _context.Roles.Remove(rol);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(IndexRoles));
        }

        // -----------------------
        // Métodos de UsuarioController
        // -----------------------

        public async Task<IActionResult> IndexUsuarios()
        {
            var usuarios = await _context.Usuarios
                .Include(u => u.IdPersona)
                .ToListAsync();
            return View(usuarios);
        }

        public async Task<IActionResult> DetailsUsuario(long? id)
        {
            if (id == null)
                return NotFound();

            var usuario = await _context.Usuarios
                .Include(u => u.IdPersona)
                .FirstOrDefaultAsync(u => u.IdUsuario == id);

            if (usuario == null)
                return NotFound();

            return View(usuario);
        }

        public IActionResult CreateUsuario()
        {
            ViewBag.Personas = _context.Personas.ToList();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateUsuario([Bind("Usuario,Contrasena,IdPersona")] Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                _context.Add(usuario);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(IndexUsuarios));
            }
            ViewBag.Personas = _context.Personas.ToList();
            return View(usuario);
        }

        public async Task<IActionResult> EditUsuario(long? id)
        {
            if (id == null)
                return NotFound();

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
                return NotFound();

            ViewBag.Personas = _context.Personas.ToList();
            return View(usuario);
        }

        [HttpPost]
        public async Task<IActionResult> EditUsuario(long id, [Bind("IdUsuario,Usuario,Contrasena,IdPersona")] Usuario usuario)
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
                return RedirectToAction(nameof(IndexUsuarios));
            }
            ViewBag.Personas = _context.Personas.ToList();
            return View(usuario);
        }

        public async Task<IActionResult> DeleteUsuario(long? id)
        {
            if (id == null)
                return NotFound();

            var usuario = await _context.Usuarios
                .Include(u => u.IdPersona)
                .FirstOrDefaultAsync(u => u.IdUsuario == id);
            if (usuario == null)
                return NotFound();

            return View(usuario);
        }

        [HttpPost, ActionName("DeleteUsuario")]
        public async Task<IActionResult> DeleteConfirmedUsuario(long id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                _context.Usuarios.Remove(usuario);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(IndexUsuarios));
        }

        public bool PersonaTieneUsuario(long idPersona)
        {
            return _context.Usuarios.Any(u => u.IdPersona == idPersona);
        }
    }
}
