using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SistemaGestionJudicial.Models;
using Microsoft.EntityFrameworkCore;

namespace SistemaGestionJudicial.Controllers
{
    public class AccountController : Controller
    {
        private readonly ProyectoContext _context;

        public AccountController(ProyectoContext context)
        {
            _context = context;
        }

        // Mostrar el formulario de login
        [HttpGet]
        public IActionResult Login()
        {
            return View("~/Views/Home/Login.cshtml");
        }


        // 1️⃣ Login
        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var user = await _context.Usuarios
                .Include(u => u.IdPersonaNavigation)
                .FirstOrDefaultAsync(u => u.Usuario1 == username && u.Contraseña == password);

            if (user == null)
            {
                ViewBag.Error = "Usuario o contraseña incorrectos.";
                //ModelState.AddModelError(string.Empty, "Usuario o contraseña incorrectos.");
                return View("~/Views/Home/Login.cshtml");
            }

            // Guardar en sesión
            HttpContext.Session.SetInt32("UsuarioId", (int)user.IdUsuario);
            HttpContext.Session.SetString("NombreUsuario", user.Usuario1);

            return RedirectToAction("Index", "Home");
        }


        /*
        // 2️⃣ Registro
        [HttpPost]
        public async Task<IActionResult> Register(Persona persona, Usuario usuario)
        {
            // Primero guardar persona
            _context.Personas.Add(persona);
            await _context.SaveChangesAsync();

            // Luego guardar usuario
            usuario.PersonaId = persona.Id; // FK
            usuario.TokenRecuperacion = Guid.NewGuid().ToString();
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return Ok("Cuenta creada correctamente");
        }

        // 3️⃣ Generar Token
        [HttpPost]
        public async Task<IActionResult> GenerarToken(string email)
        {
            var user = await _context.Usuarios.FirstOrDefaultAsync(u => u.Correo == email);
            if (user == null)
                return NotFound("No existe usuario con ese correo.");

            user.TokenRecuperacion = Guid.NewGuid().ToString();
            await _context.SaveChangesAsync();

            // Aquí enviarías el token por email, por ahora retornas el token para debug
            return Ok(user.TokenRecuperacion);
        }

        // 4️⃣ Cambiar Contraseña
        [HttpPost]
        public async Task<IActionResult> CambiarPassword(string token, string nuevaPassword)
        {
            var user = await _context.Usuarios.FirstOrDefaultAsync(u => u.TokenRecuperacion == token);
            if (user == null)
                return NotFound("Token inválido");

            user.Contrasena = nuevaPassword; // 🔒 Hashea en real
            user.TokenRecuperacion = null;   // Limpia token
            await _context.SaveChangesAsync();

            return Ok("Contraseña actualizada");
        }*/
    }

}
