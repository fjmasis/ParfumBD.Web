using Microsoft.AspNetCore.Mvc;
using ParfumBD.Web.Models;
using ParfumBD.Web.Services;

namespace ParfumBD.Web.Controllers
{
    public class PerfumesController : Controller
    {
        private readonly IPerfumeService _perfumeService;
        private readonly ICarritoService _carritoService;
        private readonly ILogger<PerfumesController> _logger;

        public PerfumesController(
            IPerfumeService perfumeService,
            ICarritoService carritoService,
            ILogger<PerfumesController> logger)
        {
            _perfumeService = perfumeService;
            _carritoService = carritoService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var perfumes = await _perfumeService.GetActivePerfumesAsync();
                return View(perfumes ?? new List<Perfume>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los perfumes");
                TempData["ErrorMessage"] = "Error al cargar los perfumes. Por favor, inténtelo de nuevo más tarde.";
                return View(new List<Perfume>());
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var perfume = await _perfumeService.GetPerfumeByIdAsync(id);

                if (perfume == null)
                {
                    _logger.LogWarning($"Perfume with ID {id} not found");
                    return NotFound();
                }

                return View(perfume);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener el perfume con ID {id}");
                TempData["ErrorMessage"] = "Error al cargar los detalles del perfume. Por favor, inténtelo de nuevo más tarde.";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(AgregarAlCarritoViewModel model)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                _logger.LogWarning("Attempt to add to cart without being logged in");
                return RedirectToAction("Login", "Account");
            }

            try
            {
                _logger.LogInformation($"Adding product {model.IdPerfume} to cart for user {userId} with quantity {model.Cantidad}");

                if (model.Cantidad <= 0)
                {
                    _logger.LogWarning($"Invalid quantity {model.Cantidad} for product {model.IdPerfume}");
                    TempData["ErrorMessage"] = "La cantidad debe ser mayor que cero.";
                    return RedirectToAction("Details", new { id = model.IdPerfume });
                }

                var result = await _carritoService.AddToCarritoAsync(userId.Value, model.IdPerfume, model.Cantidad);

                if (result == null)
                {
                    _logger.LogWarning($"Failed to add product {model.IdPerfume} to cart");
                    TempData["ErrorMessage"] = "No se pudo agregar el producto al carrito. Por favor, inténtelo de nuevo.";
                }
                else
                {
                    _logger.LogInformation($"Successfully added product {model.IdPerfume} to cart");
                    TempData["SuccessMessage"] = "Producto agregado al carrito correctamente.";
                }

                return RedirectToAction("Details", new { id = model.IdPerfume });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error adding product {model.IdPerfume} to cart");
                TempData["ErrorMessage"] = "Error al agregar el producto al carrito. Por favor, inténtelo de nuevo más tarde.";
                return RedirectToAction("Details", new { id = model.IdPerfume });
            }
        }
    }
}