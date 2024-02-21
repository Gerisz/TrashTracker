using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using TrashTracker.Web.Models.EnumModels;

namespace TrashTracker.Web.Models
{
    [Index(nameof(TrashoutId), IsUnique = true)]
    public class Trash
    {
        public Int32 Id { get; set; }
        public Int32? TrashoutId { get; set; }
        public Point? Location { get; set; } = null!;

        public Int32 CountryId { get; set; }
        public virtual Country? Country { get; set; }
    }
}
