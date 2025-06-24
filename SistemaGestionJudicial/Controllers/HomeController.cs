using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SistemaGestionJudicial.Models;

namespace SistemaGestionJudicial.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
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
            return View();
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
