using Microsoft.AspNetCore.Mvc;
using ParfumBD.Web.Models;
using ParfumBD.Web.Services;

namespace ParfumBD.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUsuarioService _usuarioService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IUsuarioService usuarioService, ILogger<AccountController> logger)
        {
            _usuarioService = usuarioService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(UsuarioLogin model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var usuario = await _usuarioService.LoginAsync(model);

                if (usuario == null)
                {
                    ModelState.AddModelError(string.Empty, "Credenciales inválidas");
                    return View(model);
                }

                // Store user info in session
                HttpContext.Session.SetInt32("UserId", usuario.IdUsuario);
                HttpContext.Session.SetString("UserName", usuario.Nombre ?? string.Empty);
                HttpContext.Session.SetString("UserEmail", usuario.Correo ?? string.Empty);
                HttpContext.Session.SetString("UserType", usuario.TipoUsuario ?? string.Empty);

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al iniciar sesión");
                ModelState.AddModelError(string.Empty, "Error al iniciar sesión. Inténtelo de nuevo más tarde.");
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(UsuarioRegistro model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.Contraseña != model.ConfirmarContraseña)
            {
                ModelState.AddModelError("ConfirmarContraseña", "Las contraseñas no coinciden");
                return View(model);
            }

            try
            {
                var usuario = await _usuarioService.RegisterAsync(model);

                if (usuario == null)
                {
                    ModelState.AddModelError(string.Empty, "Error al registrar el usuario. El correo electrónico podría estar en uso.");
                    return View(model);
                }

                // Automatically log in the user
                HttpContext.Session.SetInt32("UserId", usuario.IdUsuario);
                HttpContext.Session.SetString("UserName", usuario.Nombre ?? string.Empty);
                HttpContext.Session.SetString("UserEmail", usuario.Correo ?? string.Empty);
                HttpContext.Session.SetString("UserType", usuario.TipoUsuario ?? string.Empty);

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al registrar usuario");
                ModelState.AddModelError(string.Empty, "Error al registrar el usuario. Inténtelo de nuevo más tarde.");
                return View(model);
            }
        }

        [HttpPost]
        public IActionResult Logout()
        {
            // Clear session
            HttpContext.Session.Clear();

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login");
            }

            try
            {
                var usuario = await _usuarioService.GetUsuarioByIdAsync(userId.Value);

                if (usuario == null)
                {
                    return RedirectToAction("Login");
                }

                return View(usuario);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el perfil del usuario");
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile(Usuario model)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null || userId != model.IdUsuario)
            {
                return RedirectToAction("Login");
            }

            if (!ModelState.IsValid)
            {
                return View("Profile", model);
            }

            try
            {
                var usuario = await _usuarioService.UpdateUsuarioAsync(userId.Value, model);

                if (usuario == null)
                {
                    ModelState.AddModelError(string.Empty, "Error al actualizar el perfil");
                    return View("Profile", model);
                }

                // Update session
                HttpContext.Session.SetString("UserName", usuario.Nombre ?? string.Empty);
                HttpContext.Session.SetString("UserEmail", usuario.Correo ?? string.Empty);

                TempData["SuccessMessage"] = "Perfil actualizado correctamente";
                return RedirectToAction("Profile");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el perfil del usuario");
                ModelState.AddModelError(string.Empty, "Error al actualizar el perfil. Inténtelo de nuevo más tarde.");
                return View("Profile", model);
            }
        }
    }
}