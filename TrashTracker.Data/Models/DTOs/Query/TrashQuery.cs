using System.Linq.Expressions;
using TrashTracker.Data.Models.Enums;
using TrashTracker.Data.Models.Tables;

namespace TrashTracker.Data.Models.DTOs.Query
{
    public class TrashQuery
    {
        public Int32 Id { get; set; }
        public Int32? TrashoutId { get; set; }
        public String? UserName { get; set; }

        public Double Latitude { get; set; }
        public Double Longitude { get; set; }

        public DateTime? CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }
        public Boolean? UpdateNeeded { get; set; }

        public Int32 Accessibilities { get; set; }
        public Country? Country { get; set; }
        public Size Size { get; set; }
        public Status Status { get; set; }
        public Int32 Types { get; set; }

        public static Expression<Func<Trash, TrashQuery>> Projection { get; }
            = trash => new TrashQuery()
            {
                Id = trash.Id,
                TrashoutId = trash.TrashoutId,
                UserName = trash.User != null ? trash.User.UserName : null,
                Latitude = Math.Round(trash.Location.X, 6),
                Longitude = Math.Round(trash.Location.Y, 6),
                CreateTime = trash.CreateTime,
                UpdateTime = trash.UpdateTime,
                UpdateNeeded = trash.UpdateNeeded,
                Accessibilities = (Int32)trash.Accessibilities,
                Country = trash.Country,
                Size = trash.Size,
                Status = trash.Status,
                Types = (Int32)trash.Types
            };

        public static TrashQuery Create(Trash trash) => Projection.Compile().Invoke(trash);
    }
}
