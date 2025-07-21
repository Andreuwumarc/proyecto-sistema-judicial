using Microsoft.AspNetCore.Mvc;
using SistemaGestionJudicial.Models;

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
        public IActionResult CrearDenuncia()
        {
            return View("~/Views/Home/CrearDenuncia.cshtml");
        }



    }
}
