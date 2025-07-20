using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SistemaGestionJudicial.Models; // Necesario para ProyectoContext y Persona
using System.Linq; // Necesario para .Where() y .ToList()
using Microsoft.EntityFrameworkCore; // Necesario para .Include() si lo usas

namespace SistemaGestionJudicial.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ProyectoContext _context; // Declara una instancia de tu contexto de base de datos

        public HomeController(ILogger<HomeController> logger, ProyectoContext context) // Inyecta ILogger y ProyectoContext
        {
            _logger = logger;
            _context = context; // Asigna el contexto inyectado
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Delincuentes()
        {
            // Obtener la lista de personas con id_rol = 3 (asumiendo que es el rol de delincuente)
            // .ToList() ejecuta la consulta y trae los datos a la memoria.
            var delincuentes = _context.Personas.Where(p => p.IdRol == 3).ToList(); //

            // Pasa la lista de delincuentes a la vista
            return View(delincuentes);
        }

        public IActionResult Crimenes()
        {
            return View();
        }

        public IActionResult Casos()
        {
            return View();
        }

        public IActionResult Jueces()
        {
            return View();
        }

        public IActionResult Fiscales()
        {
            return View();
        }

        public IActionResult CrimeReports()
        {
            return View();
        }

        public IActionResult CaseReports()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}