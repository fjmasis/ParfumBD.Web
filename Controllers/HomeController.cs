using Microsoft.AspNetCore.Mvc;
using ParfumBD.Web.Models;
using ParfumBD.Web.Services;
using System.Diagnostics;

namespace ParfumBD.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IPerfumeService _perfumeService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(IPerfumeService perfumeService, ILogger<HomeController> logger)
        {
            _perfumeService = perfumeService;
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

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}