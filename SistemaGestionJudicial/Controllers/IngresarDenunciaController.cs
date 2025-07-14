using Microsoft.AspNetCore.Mvc;

namespace SistemaGestionJudicial.Controllers
{
    public class IngresarDenunciaController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        //controlador de la vista ingresar denuncia del Home publico
        public IActionResult CrearDenuncia()
        {
            return View("~/Views/HomePublic/IngresarDenuncia.cshtml");
        }

    }
}
