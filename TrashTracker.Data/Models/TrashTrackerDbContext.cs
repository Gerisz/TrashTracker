using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TrashTracker.Data.Models.Tables;

namespace TrashTracker.Data.Models
{
    /// <summary>
    /// The definition of the database's tables, derived from
    /// <see cref="IdentityDbContext{TrashTrackerUser, TrashTrackerIdentityRole, String}"/>,
    /// making it contain some extra default tables used for user-, role- and sign-in management.
    /// </summary>
    public class TrashTrackerDbContext : IdentityDbContext<TrashTrackerUser, TrashTrackerIdentityRole, String>
    {
        /// <summary>
        /// Table containing <see cref="Trash"/> objects.
        /// </summary>
        public DbSet<Trash> Trashes { get; set; } = null!;

        /// <summary>
        /// Table containing <see cref="TrashImage"/> objects.
        /// </summary>
        public DbSet<TrashImage> TrashImages { get; set; } = null!;

        /// <summary>
        /// Table containing <see cref="UserImage"/> objects.
        /// </summary>
        public DbSet<UserImage> UserImages { get; set; } = null!;

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
