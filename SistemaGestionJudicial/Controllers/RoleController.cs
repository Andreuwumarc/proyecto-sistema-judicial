using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using SistemaGestionJudicial.Models;

namespace SistemaGestionJudicial.Controllers
{
    public class RoleController : Controller
    {
        private readonly ProyectoContext _context;

        public RoleController(ProyectoContext context)
        {
            _context = context;
        }

        // GET: Role
        public async Task<IActionResult> Index()
        {
            return View(await _context.Roles.ToListAsync());
        }

        // GET: Role/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Role/Create

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Role role)
        {
            if (ModelState.IsValid)
            {
                // Obtener el último ID existente
                var ultimoId = await _context.Roles
                    .OrderByDescending(r => r.IdRol)
                    .Select(r => r.IdRol)
                    .FirstOrDefaultAsync();

                // Forzar ID siguiente
                role.IdRol = ultimoId + 1;

                // Agregar rol
                _context.Roles.Add(role);
                await _context.SaveChangesAsync();

                return Json(new { success = true, role });
            }

            return Json(new
            {
                success = false,
                errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
            });
        }





        // GET: Role/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var role = await _context.Roles.FindAsync(id);
            if (role == null)
            {
                return NotFound();
            }
            return View(role);
        }

        // POST: Role/Edit/5
        [HttpPost]
        public async Task<IActionResult> Edit(long id, [FromBody] Role role)
        {
            if (id != role.IdRol)
            {
                return Json(new { success = false, message = "ID del rol no coincide." });
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(role);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RoleExists(role.IdRol))
                    {
                        return Json(new { success = false, message = "Rol no encontrado." });
                    }
                    else
                    {
                        return Json(new { success = false, message = "Error al actualizar el rol." });
                    }
                }
            }
            return Json(new { success = false, message = "Modelo inválido." });
        }



        // GET: Role/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var role = await _context.Roles
                .FirstOrDefaultAsync(m => m.IdRol == id);
            if (role == null)
            {
                return NotFound();
            }

            return View(role);
        }

        // POST: Role/Delete/5
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role != null)
            {
                _context.Roles.Remove(role);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            return Json(new { success = false, message = "Error al eliminar el rol." });
        }
        private bool RoleExists(long id)
        {
            return _context.Roles.Any(e => e.IdRol == id);
        }

    }
}
