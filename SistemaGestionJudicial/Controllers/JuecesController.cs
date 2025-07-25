using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using SistemaGestionJudicial.Models;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace SistemaGestionJudicial.Controllers
{
    public class JuecesController : Controller
    {
        private readonly ProyectoContext _context;

        /// <summary>
        /// Constructor que recibe el contexto de la base de datos.
        /// </summary>
        /// <param name="context"></param>
        public JuecesController(ProyectoContext context)
        {
            _context = context;
        }

        #region Métodos de PersonaController (Jueces)
        // -----------------------
        // Métodos de PersonaController (Jueces)
        // -----------------------

        /// <summary>
        /// Muestra la lista de jueces (Personas con rol de juez).
        /// </summary>
        /// <returns></returns>
        [Route("/Home/Jueces")]
        public IActionResult Jueces()
        {
            var jueces = _context.Personas.Where(p => p.IdRol == 1).ToList();
            return View("~/Views/Home/Jueces.cshtml", jueces);
        }

        /// <summary>
        /// Comprueba si ya existe un juez con esta cédula (excluyendo opcionalmente un IdPersona).
        /// </summary>
        private bool CedulaDuplicada(string cedula, long? idAExcluir = null)
        {
            return _context.Personas
                           .Any(p => p.IdRol == 1
                                  && p.Cedula == cedula
                                  && (!idAExcluir.HasValue || p.IdPersona != idAExcluir.Value));
        }

        /// <summary>
        /// Valida que la cédula sea solo dígitos y hasta 10 caracteres.
        /// </summary>
        private bool CedulaValidaFormato(string cedula)
            => !string.IsNullOrEmpty(cedula)
               && Regex.IsMatch(cedula, @"^\d{1,10}$");

        /// <summary>
        /// Crea un nuevo juez (Persona con rol de juez).
        /// </summary>
        /// <param name="persona"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateJuez(
    [Bind("Cedula,Nombres,Apellidos,FechaNacimiento,Genero,Direccion,Telefono,CorreoElectronico")]
    Persona persona)
        {
            persona.IdRol = 1;

            // 1) Validaciones
            if (string.IsNullOrWhiteSpace(persona.Cedula))
                ModelState.AddModelError("Cedula", "La cédula es obligatoria.");
            else if (!CedulaValidaFormato(persona.Cedula))
                ModelState.AddModelError("Cedula", "La cédula debe contener solo dígitos y hasta 10 caracteres.");
            else if (CedulaDuplicada(persona.Cedula))
                ModelState.AddModelError("Cedula", "Ya existe un juez con esa cédula.");

            // 2) Si hay errores, vuelve a la vista y abre el modal de Crear
            if (!ModelState.IsValid)
            {
                ViewBag.OpenCreateModal = true;
                var jueces = _context.Personas.Where(p => p.IdRol == 1).ToList();
                return View("~/Views/Home/Jueces.cshtml", jueces);
            }

            // 3) Guardar
            long maxId = await _context.Personas.MaxAsync(p => (long?)p.IdPersona) ?? 0;
            persona.IdPersona = maxId + 1;
            _context.Add(persona);
            await _context.SaveChangesAsync();

            // 4) Redirigir al listado
            return RedirectToAction(nameof(Jueces));
        }

        /// <summary>
        /// Muestra el formulario de edición de un juez (Persona con rol de juez).
        /// </summary>
        /// <param name="IdPersona"></param>
        /// <param name="persona"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Edit(long IdPersona,
    [Bind("IdPersona,Cedula,Nombres,Apellidos,FechaNacimiento,Genero,Direccion,Telefono,CorreoElectronico")]
    Persona persona)
        {
            if (IdPersona != persona.IdPersona)
                return BadRequest();

            // 1) Validaciones
            if (string.IsNullOrWhiteSpace(persona.Cedula))
                ModelState.AddModelError("Cedula", "La cédula es obligatoria.");
            else if (!CedulaValidaFormato(persona.Cedula))
                ModelState.AddModelError("Cedula", "La cédula debe contener solo dígitos y hasta 10 caracteres.");
            else if (CedulaDuplicada(persona.Cedula, persona.IdPersona))
                ModelState.AddModelError("Cedula", "Ya existe otro juez con esa cédula.");

            // 2) Si hay errores, vuelve a la vista y abre el modal de Editar
            if (!ModelState.IsValid)
            {
                ViewBag.OpenEditModal = true;
                var jueces = _context.Personas.Where(p => p.IdRol == 1).ToList();
                return View("~/Views/Home/Jueces.cshtml", jueces);
            }

            // 3) Guardar cambios
            var db = await _context.Personas.FindAsync(IdPersona);
            db.Cedula = persona.Cedula;
            db.Nombres = persona.Nombres;
            db.Apellidos = persona.Apellidos;
            db.FechaNacimiento = persona.FechaNacimiento;
            db.Genero = persona.Genero;
            db.Direccion = persona.Direccion;
            db.Telefono = persona.Telefono;
            db.CorreoElectronico = persona.CorreoElectronico;
            await _context.SaveChangesAsync();

            // 4) Redirigir al listado
            return RedirectToAction(nameof(Jueces));
        }

        /// <summary>
        /// Elimina un juez (Persona con rol de juez) por su IdPersona.
        /// </summary>
        /// <param name="IdPersona"></param>
        /// <returns></returns>
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

#endregion

        #region Métodos de RolController
        // -----------------------
        // Métodos de RolController
        // -----------------------

        /// <summary>
        /// Muestra la lista de roles disponibles en el sistema.
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> IndexRoles()
        {
            var roles = await _context.Roles.ToListAsync();
            return View(roles);
        }

        /// <summary>
        /// Muestra los detalles de un rol específico por su IdRol.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> DetailsRol(long? id)
        {
            if (id == null)
                return NotFound();

            var rol = await _context.Roles.FirstOrDefaultAsync(m => m.IdRol == id);
            if (rol == null)
                return NotFound();

            return View(rol);
        }

        /// <summary>
        /// Muestra el formulario para crear un nuevo rol.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Muestra el formulario para editar un rol existente por su IdRol.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> EditRol(long? id)
        {
            if (id == null)
                return NotFound();

            var rol = await _context.Roles.FindAsync(id);
            if (rol == null)
                return NotFound();

            return View(rol);
        }

        /// <summary>
        /// Actualiza un rol existente en la base de datos.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="rol"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Muestra el formulario de confirmación para eliminar un rol por su IdRol.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> DeleteRol(long? id)
        {
            if (id == null)
                return NotFound();

            var rol = await _context.Roles.FirstOrDefaultAsync(m => m.IdRol == id);
            if (rol == null)
                return NotFound();

            return View(rol);
        }

        /// <summary>
        /// Elimina un rol de la base de datos por su IdRol.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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

        #endregion

        #region Métodos de UsuarioController
        // -----------------------
        // Métodos de UsuarioController
        // -----------------------

        /// <summary>
        /// Muestra la lista de usuarios registrados en el sistema.
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> IndexUsuarios()
        {
            var usuarios = await _context.Usuarios
                .Include(u => u.IdPersona)
                .ToListAsync();
            return View(usuarios);
        }

        /// <summary>
        /// Muestra los detalles de un usuario específico por su IdUsuario.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Muestra el formulario para crear un nuevo usuario.
        /// </summary>
        /// <returns></returns>
        public IActionResult CreateUsuario()
        {
            ViewBag.Personas = _context.Personas.ToList();
            return View();
        }

        /// <summary>
        /// Crea un nuevo usuario en el sistema.
        /// </summary>
        /// <param name="usuario"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Muestra el formulario para editar un usuario existente por su IdUsuario.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Actualiza un usuario existente en la base de datos.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="usuario"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Muestra el formulario de confirmación para eliminar un usuario por su IdUsuario.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Elimina un usuario de la base de datos por su IdUsuario.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Verifica si una persona tiene un usuario asociado en el sistema.
        /// </summary>
        /// <param name="idPersona"></param>
        /// <returns></returns>
        public bool PersonaTieneUsuario(long idPersona)
        {
            return _context.Usuarios.Any(u => u.IdPersona == idPersona);
        }

        #endregion
    }
}
