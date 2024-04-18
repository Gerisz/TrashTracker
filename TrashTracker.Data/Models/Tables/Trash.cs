using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using NetTopologySuite.Geometries.Utilities;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using TrashTracker.Data.Models.DTOs.In;
using TrashTracker.Data.Models.Enums;

namespace TrashTracker.Data.Models.Tables
{
    [Index(nameof(TrashoutId), IsUnique = true)]
    public class Trash
    {
        public Int32 Id { get; set; }
        public Int32? TrashoutId { get; set; }
        public String? UserId { get; set; }
        public virtual TrashTrackerUser? User { get; set; }

        [DisplayName("Koordináták")]
        public Point Location { get; set; } = null!;

        [DisplayName("Ország")]
        public Country? Country { get; set; }
        [DisplayName("Település")]
        public String? Locality { get; set; }
        [DisplayName("Településrész")]
        public String? SubLocality { get; set; }

        [DisplayName("Bejelentés ideje")]
        public DateTime? CreateTime { get; set; }
        [DisplayName("Legutóbbi frissítés ideje")]
        public DateTime? UpdateTime { get; set; }
        [DisplayName("Frissítésre szorul-e")]
        public Boolean? UpdateNeeded { get; set; }

        [DataType(DataType.MultilineText)]
        [DisplayName("Megjegyzés")]
        public String? Note { get; set; } = null!;

        [DisplayName("Állapot")]
        public Status Status { get; set; }
        [DisplayName("Mennyiség")]
        public Size Size { get; set; }
        [DisplayName("Szeméttípusok")]
        public TrashType Types { get; set; }
        [DisplayName("Hozzáférhetőség")]
        public Accessibility Accessibilities { get; set; }

        public virtual List<TrashImage> Images { get; set; } = new();

        public Trash() { }

        public Trash(TrashFromTrashout trashFromTrashout)
        {
            var gf = NetTopologySuite.NtsGeometryServices.Instance.CreateGeometryFactory(WGS_SRID);

            TrashoutId = trashFromTrashout.Id;
            UserId = null;

            Location = (Point)GeometryFixer.Fix(gf.CreatePoint(
                new Coordinate()
                {
                    X = Math.Round(trashFromTrashout.Gps.Long, 6),
                    Y = Math.Round(trashFromTrashout.Gps.Lat, 6)
                }));
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

            Images = (trashFromTrashout.Images ?? [])
                .Where(i => i.FullDownloadUrl != null)
                .Select(i => new TrashImage(i.FullDownloadUrl!))
                .ToList();

        }

        public Trash(TrashFromUser trashFromUser)
        {
            var gf = NetTopologySuite.NtsGeometryServices.Instance.CreateGeometryFactory(WGS_SRID);

            TrashoutId = null;
            UserId = null;

            Location = (Point)GeometryFixer.Fix(gf.CreatePoint(
                new Coordinate()
                {
                    X = Math.Round(trashFromUser.Long, 6),
                    Y = Math.Round(trashFromUser.Lat, 6)
                }));
            Country = Enums.Country.Hungary;
            Locality = trashFromUser.Locality;
            SubLocality = trashFromUser.SubLocality;
            CreateTime = DateTime.UtcNow;
            UpdateTime = DateTime.UtcNow;
            UpdateNeeded = false;
            Note = trashFromUser.Note;
            Status = Status.StillHere;
            Size = trashFromUser.Size;

            trashFromUser.Accessibilities
                .Where(a => a.IsSelected)
                .Select(a => Accessibilities |= a.Value);

            trashFromUser.Types
                .Where(a => a.IsSelected)
                .Select(a => Types |= a.Value);
        }

        private const int WGS_SRID = 4326;
    }
}
