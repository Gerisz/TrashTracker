using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrashTracker.Data.Models;
using TrashTracker.Data.Models.DTOs.Out;
using TrashTracker.Web.Controllers;
using TrashTracker.Web.Utils;

namespace TrashTracker.Web.Test
{
    /// <summary>
    /// Test class meant to test API endpoints of <see cref="HomeController"/>.
    /// </summary>
    public class HomeTest : IDisposable
    {
        private static TrashTrackerDbContext _context = null!;
        private static HomeController _controller = null!;

        public HomeTest()
        {
            var options = new DbContextOptionsBuilder<TrashTrackerDbContext>()
                .UseInMemoryDatabase("TestDb")
                .Options;

            _context = new TrashTrackerDbContext(options);
            Task.Run(() => DbInitializer.InitializeAsync(_context)).Wait();

            _context.ChangeTracker.Clear();

            _controller = new HomeController(_context);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Fact]
        public void GetPointsOnMapTest()
        {
            // Act
            var result = _controller.GetPointsOnMap().Result as OkObjectResult;

            // Assert
            var content = Assert.IsAssignableFrom<TrashMap>(Serializer.Deserialize<TrashMap>((String)result?.Value!));
            Assert.Equal(_context.Trashes.Count(), content.Features.Count());
        }

        [Fact]
        public async Task GetPointByIdOnMapDetailsTest()
        {
            // Act
            var id = (await _context.Trashes.FirstOrDefaultAsync())!.Id;
            var result = (await _controller.GetPointByIdOnMapDetailsAsync(id)).Result
                as OkObjectResult;

            // Assert
            var content = Assert.IsAssignableFrom<TrashMapDetails>(Serializer.Deserialize<TrashMapDetails>((String)result?.Value!));
            Assert.Equal(TrashMapDetails.Create((await _context.Trashes.FindAsync(id))!, "").Id, content.Id);
            Assert.Equal(TrashMapDetails.Create((await _context.Trashes.FindAsync(id))!, "").Location, content.Location);
            Assert.Equal(TrashMapDetails.Create((await _context.Trashes.FindAsync(id))!, "").Note, content.Note);
        }

        [Fact]
        public async Task GetTrashCsvTest()
        {

        }
    }
}
