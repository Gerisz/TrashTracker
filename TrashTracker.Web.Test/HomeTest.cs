using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrashTracker.Data.Models;
using TrashTracker.Data.Models.DTOs.Out;
using TrashTracker.Web.Controllers;

namespace TrashTracker.Web.Test
{
    [TestClass]
    public class HomeTest : IDisposable
    {
        private readonly TrashTrackerDbContext _context;
        private readonly HomeController _controller;

        public HomeTest()
        {
            var options = new DbContextOptionsBuilder<TrashTrackerDbContext>()
                .UseInMemoryDatabase("TestDb")
                .Options;

            _context = new TrashTrackerDbContext(options);
            DbInitializer.Initialize(_context, "");

            _context.ChangeTracker.Clear();

            _controller = new HomeController(_context);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [TestMethod]
        public async Task GetPointsOnMapTest()
        {
            // Act
            var result = await _controller.GetPointsOnMapAsync();

            // Assert
            Assert.IsInstanceOfType<ActionResult<TrashMap>>(result);
            Assert.AreEqual(_context.Trashes.Count(), result.Value?.Features.Count());
        }

        [TestMethod]
        public async Task TestOnMapDetails()
        {

            // Act
            var id = (await _context.Trashes.FirstOrDefaultAsync())!.Id;
            var result = await _controller.GetPointByIdOnMapDetailsAsync(id);

            // Assert
            Assert.IsInstanceOfType<ActionResult<TrashMapDetails>>(result);
            Assert.AreEqual(TrashMapDetails.Create((await _context.Trashes.FindAsync(id))!, ""), result.Value);
        }
    }
}
