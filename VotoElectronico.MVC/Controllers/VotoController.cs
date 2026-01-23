using Microsoft.AspNetCore.Mvc;
using Models;
using Models.DTOs;
using VotoElectronico.MVC.Services;

namespace VotoElectronico.MVC.Controllers
{
    public class VotoController : Controller
    {
        private readonly IApiConsumer _apiConsumer;

        public VotoController(IApiConsumer apiConsumer)
        {
            _apiConsumer = apiConsumer;
        }

        public async Task<IActionResult> Votar()
        {
            var token = HttpContext.Session.GetString("JWTToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Auth");
            }

            // Obtener elecciones activas
            var eleccionesResponse = await _apiConsumer.GetAsync<ApiResult<List<Eleccion>>>(
                "api/Elecciones/Activas", token);

            if (eleccionesResponse?.Success == true && eleccionesResponse.Data?.Any() == true)
            {
                var eleccion = eleccionesResponse.Data.First();

                // Obtener candidatos de la elección
                var candidatosResponse = await _apiConsumer.GetAsync<ApiResult<Eleccion>>(
                    $"api/Elecciones/{eleccion.Id}", token);

                ViewBag.Eleccion = candidatosResponse?.Data;
                return View(candidatosResponse?.Data);
            }

            ViewBag.Mensaje = "No hay elecciones activas en este momento.";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegistrarVoto(int eleccionId, int candidatoId)
        {
            var token = HttpContext.Session.GetString("JWTToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Auth");
            }

            var votoDto = new RegistrarVotoDto
            {
                EleccionId = eleccionId,
                CandidatoId = candidatoId,
                TokenVotante = token
            };

            var response = await _apiConsumer.PostAsync<VotoResponseDto>(
                "api/Votos/RegistrarVoto", votoDto, token);

            if (response?.Success == true)
            {
                ViewBag.Success = "¡Su voto ha sido registrado exitosamente!";
                ViewBag.ComprobanteHash = response.ComprobanteHash;
                return View("VotoExitoso");
            }

            ViewBag.Error = response?.Message ?? "Error al registrar el voto";
            return RedirectToAction("Votar");
        }

        public IActionResult VotoExitoso()
        {
            return View();
        }
    }
}