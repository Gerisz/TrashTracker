using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using NetTopologySuite.Geometries.Utilities;
using System.ComponentModel.DataAnnotations;
using TrashTracker.Data.Models.DTOs.In;
using TrashTracker.Data.Models.Enums;

namespace TrashTracker.Data.Models
{
    [Index(nameof(TrashoutId), IsUnique = true)]
    public class Trash
    {
        public Int32 Id { get; set; }
        public Int32? TrashoutId { get; set; }
        public String? UserId { get; set; }
        public virtual TrashTrackerUser? User { get; set; }

        public Point Location { get; set; } = null!;

        public Country? Country { get; set; }
        public String? Locality { get; set; }
        public String? SubLocality { get; set; }

        public DateTime? CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }
        public Boolean? UpdateNeeded { get; set; }

        [DataType(DataType.MultilineText)]
        public String? Note { get; set; } = null!;

        public Status Status { get; set; }
        public Size Size { get; set; }
        public TrashType Types { get; set; }
        public Accessibility Accessibilities { get; set; }

        public virtual List<TrashImage> Images { get; set; } = new();

        public Trash() { }

        public Trash(TrashFromTrashout trashFromTrashout)
        {
            var gf = NetTopologySuite.NtsGeometryServices.Instance.CreateGeometryFactory(WGS_SRID);

            TrashoutId = trashFromTrashout.Id;
            UserId = null;

            Location = (Point)GeometryFixer.Fix(gf.CreatePoint(new Coordinate((Double)trashFromTrashout.Gps.Long, (Double)trashFromTrashout.Gps.Lat)));
            Country = Enum.Parse<Country>(trashFromTrashout.Gps.Area!.Country!, true);
            Locality = trashFromTrashout.Gps.Area != null ? trashFromTrashout.Gps.Area.Locality : "";
            SubLocality = trashFromTrashout.Gps.Area != null ? trashFromTrashout.Gps.Area.SubLocality : "";
            CreateTime = trashFromTrashout.Created;
            UpdateTime = trashFromTrashout.UpdateTime;
            UpdateNeeded = trashFromTrashout.UpdateNeeded == 1 ? true : false;
            Note = trashFromTrashout.Note;
            Status = Enum.Parse<Status>(trashFromTrashout.Status!, true);
            Size = Enum.Parse<Size>(trashFromTrashout.Size!, true);

            if (trashFromTrashout.Types != null)
                foreach (var item in trashFromTrashout.Types)
                    Types |= Enum.Parse<TrashType>(item, true);

            if (trashFromTrashout.Accessibility != null)
            {
                if (trashFromTrashout.Accessibility.ByCar)
                    Accessibilities |= Accessibility.ByCar;
                if (trashFromTrashout.Accessibility.InCave)
                    Accessibilities |= Accessibility.InCave;
                if (trashFromTrashout.Accessibility.UnderWater)
                    Accessibilities |= Accessibility.UnderWater;
                if (trashFromTrashout.Accessibility.NotForGeneralCleanup)
                    Accessibilities |= Accessibility.NotForGeneralCleanup;
            }

            Images = (trashFromTrashout.Images ?? new())
                .Where(i => i.FullDownloadUrl != null)
                .Select(i => new TrashImage(i.FullDownloadUrl!))
                .ToList();

        }

        private const int WGS_SRID = 4326;
    }
}
