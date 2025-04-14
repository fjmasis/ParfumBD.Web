using Microsoft.AspNetCore.Mvc;
using ParfumBD.Web.Services;

namespace ParfumBD.Web.Controllers
{
    public class CarritoController : Controller
    {
        private readonly ICarritoService _carritoService;
        private readonly ILogger<CarritoController> _logger;

        public CarritoController(ICarritoService carritoService, ILogger<CarritoController> logger)
        {
            _carritoService = carritoService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var carrito = await _carritoService.GetCarritoAsync(userId.Value);
                return View(carrito);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el carrito");
                return View(null);
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int idDetalle, int cantidad)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                if (cantidad <= 0)
                {
                    await _carritoService.RemoveFromCarritoAsync(idDetalle);
                }
                else
                {
                    await _carritoService.UpdateCarritoItemAsync(idDetalle, cantidad);
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar la cantidad");
                TempData["ErrorMessage"] = "Error al actualizar la cantidad";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> RemoveItem(int idDetalle)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                await _carritoService.RemoveFromCarritoAsync(idDetalle);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el item del carrito");
                TempData["ErrorMessage"] = "Error al eliminar el item del carrito";
                return RedirectToAction("Index");
            }
        }
    }
}