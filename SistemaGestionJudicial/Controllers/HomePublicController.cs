using Microsoft.AspNetCore.Mvc;

namespace SistemaGestionJudicial.Controllers
{
    public class HomePublicController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        //Controlador del Home Publico que se muestra antes del Login
        public IActionResult HomePage()
        {

            return View();
        }


    }
}
