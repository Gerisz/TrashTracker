using NetTopologySuite.Geometries;
using System.Linq.Expressions;
using TrashTracker.Data.Models.DTOs.Query;
using TrashTracker.Data.Models.Enums;
using TrashTracker.Data.Models.Tables;

namespace TrashTracker.Data.Models.DTOs.Out
{
    public class OnMap
    {
        public String Type { get; set; } = "FeatureCollection";
        public IEnumerable<OnMapFeature> Features { get; set; } = [];

        public OnMap() { }

        public static Expression<Func<IEnumerable<Trash>, OnMap>> Projection { get; }
            = trashes => new OnMap()
            {
                Features = trashes
                    .AsQueryable()
                    .Select(OnMapFeature.Projection)
            };

        public static OnMap Create(IEnumerable<Trash> trashes)
            => Projection.Compile().Invoke(trashes);
    }

    public class OnMapFeature
    {
        public String Type { get; set; } = "Feature";
        public Geometry Geometry { get; set; } = null!;
        public OnMapFeatureProperties Properties { get; set; } = new();

        public static Expression<Func<Trash, OnMapFeature>> Projection { get; }
            = trash => new OnMapFeature()
            {
                Geometry = trash.Location,
                Properties = OnMapFeatureProperties.Create(TrashQuery.Create(trash))
            };

        public static OnMapFeature Create(Trash trash)
            => Projection.Compile().Invoke(trash);
    }

    public class OnMapFeatureProperties
    {
        public Int32 Id { get; set; }
        public List<String> Accessibilities { get; set; } = [];
        public Country? Country { get; set; }
        public Size Size { get; set; }
        public Status Status { get; set; }
        public List<String> Types { get; set; } = [];

        public static Expression<Func<TrashQuery, OnMapFeatureProperties>> Projection { get; }
            = trashQuery => new OnMapFeatureProperties()
            {
                Id = trashQuery.Id,
                Accessibilities = Enum.GetValues<Accessibility>()
                    .Where(a => (a & (Accessibility)trashQuery.Accessibilities) != 0)
                    .Select(a => a.ToString()).ToList(),
                Country = trashQuery.Country,
                Size = trashQuery.Size,
                Status = trashQuery.Status,
                Types = Enum.GetValues<TrashType>()
                    .Where(t => (t & (TrashType)trashQuery.Types) != 0)
                    .Select(t => t.ToString()).ToList()
            };

        public static OnMapFeatureProperties Create(TrashQuery trashQuery)
            => Projection.Compile().Invoke(trashQuery);
    }
}
