using CleanTiszaMap.Data.Utils;
using NetTopologySuite.Geometries;
using System.Linq.Expressions;
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
                Properties = OnMapFeatureProperties.Create(trash)
            };

        public static OnMapFeature Create(Trash trash)
            => Projection.Compile().Invoke(trash);
    }

    public class OnMapFeatureProperties
    {
        public Int32 Id { get; set; }
        public Country? Country { get; set; }
        public BitEnum<Accessibility> Accessibilities { get; set; } = [];
        public Size Size { get; set; }
        public Status Status { get; set; }
        public BitEnum<TrashType> Types { get; set; } = [];
        public static Expression<Func<Trash, OnMapFeatureProperties>> Projection { get; }
            = trash => new OnMapFeatureProperties()
            {
                Id = trash.Id,
                Country = trash.Country,
                Accessibilities = new BitEnum<Accessibility>(trash.Accessibilities),
                Size = trash.Size,
                Status = trash.Status,
                Types = new BitEnum<TrashType>(trash.Types)
            };

        public static OnMapFeatureProperties Create(Trash trash)
            => Projection.Compile().Invoke(trash);
    }
}
