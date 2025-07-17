using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaGestionJudicial.Models;

namespace SistemaGestionJudicial.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ProyectoContext _context;

        public HomeController(ILogger<HomeController> logger, ProyectoContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult RecuperacionCuenta()
        {
            return View();
        }
        public IActionResult Token()
        {
            return View();
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Delincuentes()
        {
            return View();
        }

        public IActionResult Crimenes()
        {
            return View();
        }
        public IActionResult Polices()
        {
            return View();
        }
        public IActionResult Casos()
        {
            return View();
        }

        //public IActionResult Jueces()
        //{
        //    var jueces = _context.Personas
        //        .Include(p => p.Rol)
        //        .Where(p => p.Rol != null && p.Rol.Nombre == "Juez")
        //        .ToList();
        //    return View(jueces);
        //}

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
