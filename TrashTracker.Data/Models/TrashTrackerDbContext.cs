using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TrashTracker.Data.Models.Tables;

namespace TrashTracker.Data.Models
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

        /// <summary>
        /// Returns the <see cref="DateTime"/> of the latest update of <see cref="Trashes">Trashes</see>
        /// </summary>
        /// <returns><see langword="null"/> if <see cref="Trashes">Trashes</see> is empty;
        /// otherwise, the <see cref="DateTime"/> of the latest update of <see cref="Trashes">Trashes</see></returns>
        public DateTime? LatestUpdate()
        {
            if (Trashes.IsNullOrEmpty())
            {
                return null;
            }
            return Trashes.OrderByDescending(o => o.UpdateTime).FirstOrDefault()!.UpdateTime;
        }
    }
}
