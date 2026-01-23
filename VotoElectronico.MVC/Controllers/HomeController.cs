using Microsoft.AspNetCore.Mvc;
using Models;
using VotoElectronico.MVC.Services;

namespace VotoElectronico.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly IApiConsumer _apiConsumer;

        public HomeController(IApiConsumer apiConsumer)
        {
            _apiConsumer = apiConsumer;
        }

        public async Task<IActionResult> Index()
        {
            // Obtener elecciones activas
            var response = await _apiConsumer.GetAsync<ApiResult<List<Eleccion>>>("api/Elecciones/Activas");

            if (response?.Success == true)
            {
                ViewBag.EleccionesActivas = response.Data;
            }

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}