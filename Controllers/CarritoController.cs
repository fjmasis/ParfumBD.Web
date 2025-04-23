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
                _logger.LogWarning("Attempt to view cart without being logged in");
                return RedirectToAction("Login", "Account");
            }

            try
            {
                _logger.LogInformation($"Getting cart for user {userId}");
                var carrito = await _carritoService.GetCarritoAsync(userId.Value);
                return View(carrito);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting cart for user {userId}");
                TempData["ErrorMessage"] = "Error al cargar el carrito. Por favor, inténtelo de nuevo más tarde.";
                return View(null);
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int idDetalle, int cantidad)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                _logger.LogWarning("Attempt to update cart without being logged in");
                return RedirectToAction("Login", "Account");
            }

            try
            {
                _logger.LogInformation($"Updating cart item {idDetalle} to quantity {cantidad} for user {userId}");

                if (cantidad <= 0)
                {
                    _logger.LogInformation($"Removing cart item {idDetalle} because quantity is {cantidad}");
                    var removed = await _carritoService.RemoveFromCarritoAsync(idDetalle);
                    if (!removed)
                    {
                        _logger.LogWarning($"Failed to remove cart item {idDetalle}");
                        TempData["ErrorMessage"] = "Error al eliminar el producto del carrito.";
                    }
                }
                else
                {
                    var updated = await _carritoService.UpdateCarritoItemAsync(idDetalle, cantidad);
                    if (!updated)
                    {
                        _logger.LogWarning($"Failed to update cart item {idDetalle}");
                        TempData["ErrorMessage"] = "Error al actualizar la cantidad.";
                    }
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating cart item {idDetalle}");
                TempData["ErrorMessage"] = "Error al actualizar la cantidad. Por favor, inténtelo de nuevo más tarde.";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> RemoveItem(int idDetalle)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                _logger.LogWarning("Attempt to remove from cart without being logged in");
                return RedirectToAction("Login", "Account");
            }

            try
            {
                _logger.LogInformation($"Removing cart item {idDetalle} for user {userId}");
                var removed = await _carritoService.RemoveFromCarritoAsync(idDetalle);

                if (!removed)
                {
                    _logger.LogWarning($"Failed to remove cart item {idDetalle}");
                    TempData["ErrorMessage"] = "Error al eliminar el producto del carrito.";
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error removing cart item {idDetalle}");
                TempData["ErrorMessage"] = "Error al eliminar el producto del carrito. Por favor, inténtelo de nuevo más tarde.";
                return RedirectToAction("Index");
            }
        }
    }
}