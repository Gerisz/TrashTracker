using System.Linq.Expressions;
using TrashTracker.Web.DTOs.Out;
using TrashTracker.Web.Models;
using TrashTracker.Web.Models.Enums;

namespace TrashTracker.Web.DTOs.Query
{
    public class TrashQuery
    {
        public Int32 Id { get; set; }
        public Int32 Accessibilities { get; set; }
        public Country? Country { get; set; }
        public Size Size { get; set; }
        public Status Status { get; set; }
        public Int32 Types { get; set; }

        public static Expression<Func<Trash, TrashQuery>> Projection { get; }
            = trash => new TrashQuery()
            {
                Id = trash.Id,
                Accessibilities = (Int32)trash.Accessibilities,
                Country = trash.Country,
                Size = trash.Size,
                Status = trash.Status,
                Types = (Int32)trash.Types
            };

        public static TrashQuery Create(Trash trash) => Projection.Compile().Invoke(trash);
    }
}
