using Microsoft.AspNetCore.Mvc;
using VotoElectronico.MVC.Services;
using Models;

namespace VotoElectronico.MVC.Controllers
{
    public class CandidatoController : Controller
    {
        private readonly IApiConsumer _apiConsumer;

        public CandidatoController(IApiConsumer apiConsumer)
        {
            _apiConsumer = apiConsumer;
        }

        public async Task<IActionResult> Dashboard()
        {
            var token = HttpContext.Session.GetString("JWTToken");
            var userRole = HttpContext.Session.GetString("UserRole");
            var userId = HttpContext.Session.GetInt32("UserId");

            if (string.IsNullOrEmpty(token) || userRole != "Candidato")
            {
                TempData["Error"] = "Acceso denegado. Solo candidatos pueden acceder.";
                return RedirectToAction("Login", "Auth");
            }

            try
            {
                // Obtener información del candidato
                var usuarioResponse = await _apiConsumer.GetAsync<ApiResult<Usuario>>(
                    $"api/Usuarios/{userId}", token);

                var candidatosResponse = await _apiConsumer.GetAsync<ApiResult<List<Candidato>>>(
                    "api/Candidatos", token);

                ViewBag.Usuario = usuarioResponse?.Data;
                ViewBag.MisCandidaturas = candidatosResponse?.Data?
                    .Where(c => c.UsuarioId == userId).ToList();

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error al cargar datos: {ex.Message}";
                return View();
            }
        }
    }
}