using CleanTiszaMap.Data.Utils;
using NetTopologySuite.Geometries;
using System.Linq.Expressions;
using TrashTracker.Data.Models.Enums;
using TrashTracker.Data.Models.Tables;

namespace TrashTracker.Data.Models.DTOs.Out
{
    /// <summary>
    /// DTO meant to contain an <see cref="IEnumerable{Trash}"/> object in a valid .geojson format,
    /// to be showed on the map.
    /// </summary>
    public class TrashMap
    {
        /// <summary>
        /// Type of object (defaulted to "FeatureCollection").
        /// </summary>
        public String Type { get; set; } = "FeatureCollection";

        /// <summary>
        /// Enumeration of points to be showed on the map.
        /// </summary>
        public IEnumerable<OnMapFeature> Features { get; set; } = [];

        public TrashMap() { }

        /// <summary>
        /// Projects an <see cref="IEnumerable{Trash}"/> to a <see cref="TrashMap"/> object.
        /// </summary>
        public static Expression<Func<IEnumerable<Trash>, TrashMap>> Projection { get; }
            = trashes => new TrashMap()
            {
                Features = trashes
                    .AsQueryable()
                    .Select(OnMapFeature.Projection)
            };

        /// <summary>
        /// Creates a <see cref="TrashMap"/> from an <see cref="IEnumerable{Trash}"/> object.
        /// </summary>
        public static TrashMap Create(IEnumerable<Trash> trashes)
            => Projection.Compile().Invoke(trashes);
    }

    /// <summary>
    /// DTO meant to contain a feature (in this case a <see cref="Trash"/> object)
    /// in a valid .geojson format, to be showed on the map
    /// </summary>
    public class OnMapFeature
    {
        /// <summary>
        /// Type of object (defaulted to "Feature").
        /// </summary>
        public String Type { get; set; } = "Feature";

        /// <summary>
        /// <inheritdoc cref="NetTopologySuite.Geometries.Geometry"/>
        /// </summary>
        public Geometry Geometry { get; set; } = null!;

        /// <summary>
        /// Properties of the feature
        /// (e. g. properties to be filtered by the filter menu on the map).
        /// </summary>
        public OnMapFeatureProperties Properties { get; set; } = new();

        /// <summary>
        /// Projects a <see cref="Trash"/> to an <see cref="OnMapFeature"/> object.
        /// </summary>
        public static Expression<Func<Trash, OnMapFeature>> Projection { get; }
            = trash => new OnMapFeature()
            {
                Geometry = trash.Location,
                Properties = OnMapFeatureProperties.Create(trash)
            };

        /// <summary>
        /// Creates an <see cref="OnMapFeature"/> to a <see cref="Trash"/> object.
        /// </summary>
        /// <param name="trash">The <see cref="Trash"/> object to copy values from.</param>
        /// <returns>The created <see cref="OnMapFeature"/> from <paramref name="trash"/>.</returns>
        public static OnMapFeature Create(Trash trash)
            => Projection.Compile().Invoke(trash);
    }

    /// <summary>
    /// DTO meant to contain a feature's properties
    /// (in this case a <see cref="Trash"/>'s properties), to be showed and filtered by on the map.
    /// </summary>
    public class OnMapFeatureProperties
    {
        /// <summary>
        /// The id of the point in the database.
        /// </summary>
        public Int32 Id { get; set; }

        /// <summary>
        /// The country of the location (possible values defined by <see cref="Enums.Country"/>).
        /// </summary>
        public Country? Country { get; set; }

        /// <summary>
        /// List of different accessibilities of the trash
        /// (possible values defined by <see cref="Enums.Accessibility"/>).
        /// </summary>
        public BitEnum<Accessibility> Accessibilities { get; set; } = [];

        /// <summary>
        /// Size of the trash (possible values defined by <see cref="Enums.Size"/>).
        /// </summary>
        public Size Size { get; set; }

        /// <summary>
        /// Current status of the trash (possible values defined by <see cref="Enums.Status"/>).
        /// </summary>
        public Status Status { get; set; }

        /// <summary>
        /// List of different types of waste found at the point
        /// (possible values defined by <see cref="Enums.TrashType"/>).
        /// </summary>
        public BitEnum<TrashType> Types { get; set; } = [];

        /// <summary>
        /// Projects a <see cref="Trash"/> to an <see cref="OnMapFeatureProperties"/> object.
        /// </summary>
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

        /// <summary>
        /// Creates an <see cref="OnMapFeatureProperties"/> to a <see cref="Trash"/> object.
        /// </summary>
        /// <param name="trash">The <see cref="Trash"/> object to copy values from.</param>
        /// <returns>The created <see cref="TrashDetails"/> from <paramref name="trash"/>.</returns>
        public static OnMapFeatureProperties Create(Trash trash)
            => Projection.Compile().Invoke(trash);
    }
}
