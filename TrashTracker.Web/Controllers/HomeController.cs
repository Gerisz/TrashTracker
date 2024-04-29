using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using TrashTracker.Data.Models;
using TrashTracker.Data.Models.DTOs.Out;
using TrashTracker.Web.Utils;

namespace TrashTracker.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly TrashTrackerDbContext _context;

        private string ImageDownloadURL => Url
            .Action(action: "DownloadPlaceImage", controller: "Places",
            values: new { id = "0" }, protocol: Request.Scheme)![0..^2];

        public HomeController(ILogger<HomeController> logger, TrashTrackerDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet("OnMap")]
        public IActionResult GetPointsOnMap()
        {
            var points = OnMap.Create(_context.Trashes.AsNoTracking());

            var json = Serializer.Serialize(points);

            return Ok(json);
        }

        [HttpGet("OnMapDetails/{id}")]
        public async Task<IActionResult> GetPointByIdOnMapDetailsAsync(Int32 id)
        {
            var trash = await _context.Trashes.FindAsync(id);
            if (trash == null)
                return NotFound();
            return Ok(Serializer.Serialize(OnMapDetails.Create(trash, ImageDownloadURL)));
        }


        public IActionResult Index(Int32? lat, Int32? lon)
        {
            // TODO: implement zoom to coord when parameters are fully given
            if (lat == null || lon == null)
                return View();
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new Error
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}
