using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using TrashTracker.Web.DTOs.Out;
using TrashTracker.Web.Models;
using TrashTracker.Web.Utils;

namespace TrashTracker.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly TrashTrackerDbContext _context;

        public HomeController(ILogger<HomeController> logger, TrashTrackerDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet("OnMap")]
        public async Task<IActionResult> GetPointsOnMapAsync()
        {
            var points = OnMap.Create(_context.Trashes.AsNoTracking());

            var json = Serializer.Serialize(points);

            return Ok(json);
        }

        public IActionResult Index()
        {
            return View();
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
