using Microsoft.AspNetCore.Mvc;
using SistemaGestionJudicial.Models;

using AspNetCoreGeneratedDocument;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace SistemaGestionJudicial.Controllers
{
    public class IngresarDenunciaController : Controller
    {
        private readonly ProyectoContext _context;

        public IngresarDenunciaController(ProyectoContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        //controlador de la vista ingresar denuncia del Home publico
        public async Task<IActionResult> CrearDenuncia()
        {
            // 1. Obtener nombre de usuario actual
            var username = HttpContext.Session.GetString("NombreUsuario");

            if (string.IsNullOrEmpty(username))
            {

                return RedirectToAction("Login");
            }

            //buscar usuario en la bdd
            var usuario = await _context.Usuarios
                .Include(u => u.IdPersonaNavigation) //trae los datos de la persona
                .FirstOrDefaultAsync(u => u.Usuario1 == username);

            if (usuario == null || usuario.IdPersonaNavigation == null)
            {
                return RedirectToAction("Login");
            }

            //obtener los tipos de delitos
            var tiposDelito = await _context.Delitos.ToListAsync();

            var modelo = new CrearDenunciaViewModel
            {
                DatosPersona = usuario.IdPersonaNavigation,
                TiposDelito = tiposDelito,
                NuevaDenuncia = new Denuncia()
            };

            return View("~/Views/Home/CrearDenuncia.cshtml", modelo);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnviarDenuncia(CrearDenunciaViewModel model)
        {
            //Obtener el usuario de sesion
            var username = HttpContext.Session.GetString("NombreUsuario");

            ModelState.Remove("TiposDelito");

            if (string.IsNullOrEmpty(username))
            {
                TempData["Error"] = "No se pudo identificar al usuario.";
                return RedirectToAction("~/Views/Home/Login.cshtml");
            }

            var usuario = await _context.Usuarios
                .Include(u => u.IdPersonaNavigation)
                .FirstOrDefaultAsync(u => u.Usuario1 == username);

            if (usuario == null || usuario.IdPersonaNavigation == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                TempData["Error"] = "Errores de validación: " + string.Join(", ", errors);
                // Recargar datos
                model.DatosPersona = usuario.IdPersonaNavigation;
                model.TiposDelito = await _context.Delitos.ToListAsync();
                return View("~/Views/Home/CrearDenuncia.cshtml", model);
            }

            if (ModelState.IsValid)
            {

                // 1 Obtener el último ID existente
                var ultimoId = await _context.Denuncias
                    .OrderByDescending(d => d.IdDenuncia)
                    .Select(d => d.IdDenuncia)
                    .FirstOrDefaultAsync();

                // 2 Asignar el nuevo ID manualmente
                model.NuevaDenuncia.IdDenuncia = ultimoId + 1;

                var nuevaDenuncia = new Denuncia
                {
                    IdDenuncia = model.NuevaDenuncia.IdDenuncia,
                    FechaDenuncia = model.NuevaDenuncia.FechaDenuncia, // fecha seleccionada por el usuario en el view
                    LugarHecho = model.NuevaDenuncia.LugarHecho,
                    Descripcion = model.NuevaDenuncia.Descripcion,
                    IdPersonaDenuncia = usuario.IdPersonaNavigation.IdPersona, //quien denuncia
                    IdDelito = model.NuevaDenuncia.IdDelito
                };

                _context.Denuncias.Add(nuevaDenuncia);
                await _context.SaveChangesAsync();

                //confirmacion de nueva denuncia
                TempData["Success"] = "La denuncia fue registrada con éxito.";
                return RedirectToAction("CrearDenuncia");
            }

            return View("~/Views/Home/CrearDenuncia.cshtml", model);

        }



    }
}