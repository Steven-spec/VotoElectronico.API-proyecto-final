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
            try
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.Error = "Por favor complete todos los campos correctamente";
                    return View(usuario);
                }

                // Validar que se haya seleccionado un rol
                if (string.IsNullOrEmpty(usuario.Rol))
                {
                    ViewBag.Error = "Debe seleccionar un tipo de cuenta";
                    return View(usuario);
                }

                // NO forzar el rol, usar el que seleccionó el usuario
                usuario.Activo = true;
                usuario.HaVotado = false;
                usuario.FechaRegistro = DateTime.Now;

                var response = await _apiConsumer.PostAsync<ApiResult<Usuario>>(
                    "api/Auth/Register", usuario);

                if (response == null)
                {
                    ViewBag.Error = "Error de conexión con la API. Verifique que la API esté corriendo en http://localhost:5050";
                    return View(usuario);
                }

                if (response.Success == true)
                {
                    TempData["SuccessMessage"] = $"¡Cuenta creada exitosamente!";
                    TempData["SuccessDetails"] = $"Se ha registrado como <strong>{usuario.Rol}</strong>. Ya puede iniciar sesión con su correo y contraseña.";
                    TempData["ShowSuccessModal"] = true;
                    return RedirectToAction("Login");
                }

                ViewBag.Error = response.Message ?? "Error al registrar usuario";
                return View(usuario);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error inesperado: {ex.Message}";
                return View(usuario);
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}