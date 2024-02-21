using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using TrashTracker.Web.Models.EnumModels;

namespace TrashTracker.Web.Models
{
    public class TrashTrackerDbContext : IdentityDbContext<TrashTrackerUser, TrashTrackerIdentityRole, String>
    {
        public DbSet<Country> Countries { get; set; } = null!;
        public DbSet<Trash> Trashes { get; set; } = null!;

        public TrashTrackerDbContext() : base(new DbContextOptions<TrashTrackerDbContext>()) { }

        public TrashTrackerDbContext(DbContextOptions<TrashTrackerDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => t.GetTypeInfo().IsClass && t.IsSubclassOf(typeof(EnumTable))).ToList()
            .ForEach(t => GetType().GetMethods().Single(m => m.Name == "BuildEnumTable")
                .MakeGenericMethod((Type)t.GetProperties().Single(p => p.Name == "EnumType").GetValue(null)!, t)
                .Invoke(null, new[] { builder }));
        }

        /// <summary>
        /// Creates a key-value table from an enum
        /// </summary>
        /// <typeparam name="TEnum">The enum to create the table from</typeparam>
        /// <typeparam name="T">The table to create in</typeparam>
        /// <param name="builder"></param>
        public static void BuildEnumTable<TEnum, T>(ModelBuilder builder)
            where TEnum : struct, Enum
            where T : EnumTable, new()
        {
            builder
                .Entity<T>()
                .Property(e => e.Id)
                .HasConversion<int>();
            builder
                .Entity<T>().HasData(
                    Enum.GetValues<TEnum>()
                        .Select(e => new T()
                        {
                            Id = Convert.ToInt32(e),
                            Value = e.ToString() ?? ""
                        }
                    )
            );
        }
    }
}
