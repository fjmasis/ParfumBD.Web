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
                return View(perfumes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los perfumes");
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
                    return NotFound();
                }

                return View(perfume);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el perfume");
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(AgregarAlCarritoViewModel model)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                await _carritoService.AddToCarritoAsync(userId.Value, model.IdPerfume, model.Cantidad);
                TempData["SuccessMessage"] = "Producto agregado al carrito";
                return RedirectToAction("Details", new { id = model.IdPerfume });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al agregar al carrito");
                TempData["ErrorMessage"] = "Error al agregar al carrito";
                return RedirectToAction("Details", new { id = model.IdPerfume });
            }
        }
    }
}