using Microsoft.AspNetCore.Mvc;
using VotoElectronico.MVC.Services;
using Models;
using Models.DTOs;

namespace VotoElectronico.MVC.Controllers
{
    public class AuthController : Controller
    {
        private readonly IApiConsumer _apiConsumer;

        public AuthController(IApiConsumer apiConsumer)
        {
            _apiConsumer = apiConsumer;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return View(loginDto);
            }

            var response = await _apiConsumer.PostAsync<LoginResponseDto>("api/Auth/Login", loginDto);

            if (response?.Success == true && !string.IsNullOrEmpty(response.Token))
            {
                // Guardar token en sesión
                HttpContext.Session.SetString("JWTToken", response.Token);
                HttpContext.Session.SetString("UserName", response.Usuario?.Nombres ?? "Usuario");
                HttpContext.Session.SetString("UserRole", response.Usuario?.Rol ?? "");
                HttpContext.Session.SetInt32("UserId", response.Usuario?.Id ?? 0);

                // Redirigir según el rol
                if (response.Usuario?.Rol == "Administrador")
                {
                    return RedirectToAction("Dashboard", "Admin");
                }
                else if (response.Usuario?.Rol == "Votante")
                {
                    return RedirectToAction("Votar", "Voto");
                }

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = response?.Message ?? "Error al iniciar sesión";
            return View(loginDto);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(Usuario usuario)
        {
            if (!ModelState.IsValid)
            {
                return View(usuario);
            }

            usuario.Rol = "Votante"; // Por defecto los registros son votantes
            usuario.Activo = true;

            var response = await _apiConsumer.PostAsync<ApiResult<Usuario>>(
                "api/Auth/Register", usuario);

            if (response?.Success == true)
            {
                TempData["Success"] = "Usuario registrado exitosamente. Ya puede iniciar sesión.";
                return RedirectToAction("Login");
            }

            ViewBag.Error = response?.Message ?? "Error al registrar usuario";
            return View(usuario);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}