using CleanTiszaMap.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace TrashTracker.Web.Models
{
    public class TrashTrackerDbContext : IdentityDbContext<TrashTrackerUser, TrashTrackerIdentityRole, String>
    {
        public DbSet<Trash> Trashes { get; set; } = null!;
        public DbSet<TrashImage> TrashImages { get; set; } = null!;

        public TrashTrackerDbContext() : base(new DbContextOptions<TrashTrackerDbContext>()) { }

        public TrashTrackerDbContext(DbContextOptions<TrashTrackerDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
