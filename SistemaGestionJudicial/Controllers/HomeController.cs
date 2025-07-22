using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaGestionJudicial.Models;
using System.Diagnostics;

namespace SistemaGestionJudicial.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        //Lo intenté. Si puedes arreglarlo, te debo el mundo entero.
        //22/7/2025: SIIIII LO LOGRÉ. Todo comentario puede ser eliminado.
        //private readonly ProyectoContext _context;

        public HomeController(ILogger<HomeController> logger/*, ProyectoContext context*/)
        {
            _logger = logger;
            //_context = context;
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

        //Línea original
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
