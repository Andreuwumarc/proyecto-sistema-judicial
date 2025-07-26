// Ruta: SistemaGestionJudicial/Controllers/HomeController.cs

using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaGestionJudicial.Models;


namespace SistemaGestionJudicial.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ProyectoContext _context; // <<-- CORRECCIÓN CLAVE: Tipo correcto y readonly

        // <<-- CORRECCIÓN CLAVE: Inyecta ProyectoContext aquí
        public HomeController(ILogger<HomeController> logger, ProyectoContext context)
        {
            _logger = logger;
            _context = context; // <<-- CORRECCIÓN CLAVE: Asigna el contexto inyectado
        }


        public IActionResult Home()
        {
            return View();
        }


        public IActionResult CrearDenuncia()
        {
            return View();
        }


        
        public IActionResult RecuperacionCuenta()
        {
            return View();
        }

        public IActionResult Index()
        {
            return View();
        }

        // <<-- AQUÍ ES DONDE ESTABA EL PROBLEMA ORIGINAL DEL "NO EXISTE DELINCUENTE"
        // Porque _context era null. Ahora, al inyectarlo, ya no será null.
        // En HomeController.cs
        public IActionResult Delincuentes()
        {
            return RedirectToAction("Index", "Delincuente"); // Redirige al DelincuenteController

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

        public IActionResult Jueces()
        {
            return View();
        }

        /*public IActionResult Fiscales()
        {
            
            var fiscales = _context.Fiscales
                .Include(f => f.IdPersonaFiscalNavigation) // Datos de la tabla 'Persona'
                .Include(f => f.IdDenunciaNavigation) // Datos de la tabla 'Denuncia'
                .Include(f => f.IdDenunciaNavigation.IdDelitoNavigation)
                .ToList();
            return View(fiscales);
        }*/

        public IActionResult CrimeReports()
        {
            return View();
        }

        /*public IActionResult CaseReports()
        {
            return View();
        }*/

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