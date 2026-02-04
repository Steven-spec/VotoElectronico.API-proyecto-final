using Microsoft.AspNetCore.Mvc;
using VotoElectronico.MVC.Services;
using Models;

namespace VotoElectronico.MVC.Controllers
{
    public class AdminController : Controller
    {
        private readonly IApiConsumer _apiConsumer;

        public AdminController(IApiConsumer apiConsumer)
        {
            _apiConsumer = apiConsumer;
        }

        public async Task<IActionResult> Dashboard()
        {
            var token = HttpContext.Session.GetString("JWTToken");
            var userRole = HttpContext.Session.GetString("UserRole");

            // Verificar autenticación y rol
            if (string.IsNullOrEmpty(token) || userRole != "Administrador")
            {
                TempData["Error"] = "Acceso denegado. Solo administradores pueden acceder.";
                return RedirectToAction("Login", "Auth");
            }

            try
            {
                // Obtener estadísticas
                var eleccionesResponse = await _apiConsumer.GetAsync<ApiResult<List<Eleccion>>>(
                    "api/Elecciones", token);

                var usuariosResponse = await _apiConsumer.GetAsync<ApiResult<List<Usuario>>>(
                    "api/Usuarios", token);

                var candidatosResponse = await _apiConsumer.GetAsync<ApiResult<List<Candidato>>>(
                    "api/Candidatos", token);

                var partidosResponse = await _apiConsumer.GetAsync<ApiResult<List<Partido>>>(
                    "api/Partidos", token);

                // Pasar datos a la vista
                ViewBag.TotalElecciones = eleccionesResponse?.Data?.Count ?? 0;
                ViewBag.TotalUsuarios = usuariosResponse?.Data?.Count ?? 0;
                ViewBag.TotalVotantes = usuariosResponse?.Data?.Count(u => u.Rol == "Votante") ?? 0;
                ViewBag.TotalCandidatos = candidatosResponse?.Data?.Count ?? 0;
                ViewBag.TotalPartidos = partidosResponse?.Data?.Count ?? 0;

                ViewBag.Elecciones = eleccionesResponse?.Data ?? new List<Eleccion>();
                ViewBag.EleccionesActivas = eleccionesResponse?.Data?.Where(e => e.Estado == "EnCurso").ToList() ?? new List<Eleccion>();

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error al cargar datos: {ex.Message}";
                return View();
            }
        }

        public async Task<IActionResult> Elecciones()
        {
            var token = HttpContext.Session.GetString("JWTToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Auth");
            }

            var response = await _apiConsumer.GetAsync<ApiResult<List<Eleccion>>>(
                "api/Elecciones", token);

            return View(response?.Data ?? new List<Eleccion>());
        }

        public async Task<IActionResult> Usuarios()
        {
            var token = HttpContext.Session.GetString("JWTToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Auth");
            }

            var response = await _apiConsumer.GetAsync<ApiResult<List<Usuario>>>(
                "api/Usuarios", token);

            return View(response?.Data ?? new List<Usuario>());
        }

        public async Task<IActionResult> Resultados(int id)
        {
            var token = HttpContext.Session.GetString("JWTToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Auth");
            }

            try
            {
                var response = await _apiConsumer.GetAsync<ApiResult<List<Dictionary<string, object>>>>(
                    $"api/Resultados/Eleccion/{id}", token);

                ViewBag.EleccionId = id;

                if (response?.Success == true && response.Data != null)
                {
                    return View(response.Data);
                }

                return View(new List<Dictionary<string, object>>());
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error al obtener resultados: {ex.Message}";
                return View(new List<Dictionary<string, object>>());
            }
        }
    }
}