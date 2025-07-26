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
            HttpContext.Session.SetInt32("RolUsuario", (int)user.IdPersonaNavigation.IdRol);

            // 🟢 Aquí guardas el primer nombre
            string primerNombre = user.IdPersonaNavigation.Nombres.Split(' ')[0];
            HttpContext.Session.SetString("PrimerNombre", primerNombre);

            /*return RedirectToAction("Index", "Home");*/
            return RedirectToAction("Index", "Dashboard");
        }



        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Datos inválidos");
            }

            // Obtener último id para simular autoincremento
            var lastPersona = await _context.Personas.OrderByDescending(p => p.IdPersona).FirstOrDefaultAsync();
            long newIdPersona = (lastPersona != null) ? lastPersona.IdPersona + 1 : 1;  // Evita null


            // 1️⃣ Crear persona con rol
            var persona = new Persona
            {
                IdPersona = newIdPersona,  // Aquí asignas el id simulado
                Cedula = model.Cedula,
                Nombres = model.Nombres,
                Apellidos = model.Apellidos,
                FechaNacimiento = DateOnly.FromDateTime(model.FechaNacimiento),
                Genero = model.Genero,
                Direccion = model.Direccion,
                Telefono = model.Telefono,
                CorreoElectronico = model.CorreoElectronico,
                IdRol = model.IdRol
            };

            _context.Personas.Add(persona);
            await _context.SaveChangesAsync();

            // Obtener último id de usuario para simular autoincremento
            var lastUsuario = await _context.Usuarios.OrderByDescending(u => u.IdUsuario).FirstOrDefaultAsync();
            long newIdUsuario = (lastUsuario != null) ? lastUsuario.IdUsuario + 1 : 1;  // Evita null

            long newUserId = (lastUsuario?.IdUsuario ?? 0) + 1;

            // Crear usuario relacionado a persona
            var usuario = new Usuario
            {
                IdUsuario = newUserId,      // Aquí asignas el id simulado
                IdPersona = persona.IdPersona,  // FK
                Usuario1 = model.Nombres,       // Usar nombre como usuario
                Contraseña = model.Contrasena,  // ⚠️ Recuerda que sin hash es inseguro
                Token = Guid.NewGuid().ToString()
            };


            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return Ok("Cuenta creada correctamente");
        }


        [HttpPost]
        public async Task<IActionResult> GenerarToken([FromBody] string correo)
        {
            if (string.IsNullOrEmpty(correo))
                return BadRequest("Correo requerido");

            // Busca persona
            var persona = await _context.Personas.FirstOrDefaultAsync(p => p.CorreoElectronico == correo);
            if (persona == null)
                return NotFound("Correo electrónico incorrecto");

            // Busca usuario de esa persona
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.IdPersona == persona.IdPersona);
            if (usuario == null)
                return NotFound("No existe usuario para esa persona");

            // Si ya tiene token, usarlo
            if (!string.IsNullOrEmpty(usuario.Token))
            {
                return Ok(new { token = usuario.Token });
            }

            // Si no tiene token, genera uno
            var nuevoToken = Guid.NewGuid().ToString().Substring(0, 6).ToUpper();
            usuario.Token = nuevoToken;
            await _context.SaveChangesAsync();

            return Ok(new { token = nuevoToken });
        }


        [HttpPost]
        public async Task<IActionResult> ValidarToken([FromBody] TokenValidationModel model)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.IdPersonaNavigation)
                .FirstOrDefaultAsync(u =>
                    u.IdPersonaNavigation.CorreoElectronico == model.Correo &&
                    u.Token == model.Token);

            if (usuario == null)
                return NotFound("Token incorrecto o expirado");

            return Ok("Token válido");
        }

        public class TokenValidationModel
        {
            public string Correo { get; set; }
            public string Token { get; set; }
        }


        [HttpPost]
        public async Task<IActionResult> CambiarPassword([FromBody] PasswordChangeModel model)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.IdPersonaNavigation)
                .FirstOrDefaultAsync(u =>
                    u.IdPersonaNavigation.CorreoElectronico == model.Correo &&
                    u.Token == model.Token);

            if (usuario == null)
                return NotFound("Token inválido");

            usuario.Contraseña = model.NewPassword; // 🔒 Siempre hashea real
            usuario.Token = GenerateToken(20); // 🔑 Genera token nuevo igual de abstracto

            await _context.SaveChangesAsync();

            return Ok("Contraseña actualizada, se generó un nuevo token.");
        }

        private static string GenerateToken(int length = 20)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }




        public class PasswordChangeModel
        {
            public string Correo { get; set; }
            public string Token { get; set; }
            public string NewPassword { get; set; }
        }


        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); // Elimina todos los datos de sesión
            return RedirectToAction("Login", "Account"); // Redirige al login
        }


    }

}
