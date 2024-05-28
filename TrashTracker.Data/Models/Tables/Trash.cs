using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using NetTopologySuite.Geometries.Utilities;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using TrashTracker.Data.Models.DTOs.In;
using TrashTracker.Data.Models.Enums;

namespace TrashTracker.Data.Models.Tables
{
    /// <summary>
    /// Definition of the table named Trashes, 
    /// containing points downloaded from Trashout or created by users.
    /// </summary>
    [Index(nameof(TrashoutId), IsUnique = true)]
    public class Trash
    {
        /// <summary>
        /// The id of the point in the database.
        /// </summary>
        public Int32 Id { get; set; }

        /// <summary>
        /// The id of the point in the TrashOut database (if from there, otherwise it's null).
        /// </summary>
        public Int32? TrashoutId { get; set; }


        /// <summary>
        /// The user's id of the point's creator (if created by a user, otherwise it's null).
        /// </summary>
        public String? UserId { get; set; }

        /// <summary>
        /// The user of the point's creator (if created by a user, otherwise it's null).
        /// </summary>
        public virtual TrashTrackerUser? User { get; set; }

        /// <summary>
        /// <inheritdoc cref="Point"/>
        /// </summary>
        [DisplayName("Koordináták")]
        public Point Location { get; set; } = null!;

        /// <summary>
        /// The country of the location (possible values defined by <see cref="Enums.Country"/>).
        /// </summary>
        [DisplayName("Ország")]
        public Country? Country { get; set; }

        /// <summary>
        /// Locality of the location, typically in which settlement's borders is the point is in.
        /// </summary>
        [DisplayName("Település")]
        public String? Locality { get; set; }

        /// <summary>
        /// Sublocality of the location, typically in which district is the point is in.
        /// </summary>
        [DisplayName("Településrész")]
        public String? SubLocality { get; set; }

        /// <summary>
        /// Date and time when the point was created.
        /// </summary>
        [DisplayName("Bejelentés ideje")]
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// Date and time when the point was last updated.
        /// </summary>
        [DisplayName("Legutóbbi frissítés ideje")]
        public DateTime? UpdateTime { get; set; }

        /// <summary>
        /// Logical value of the point is in need of update.
        /// </summary>
        [DisplayName("Frissítésre szorul-e")]
        public Boolean? UpdateNeeded { get; set; }

        /// <summary>
        /// Additional information given about the trash.
        /// </summary>
        [DataType(DataType.MultilineText)]
        [DisplayName("Megjegyzés")]
        public String? Note { get; set; } = null!;

        /// <summary>
        /// Current status of the trash (possible values defined by <see cref="Enums.Status"/>).
        /// </summary>
        [DisplayName("Állapot")]
        public Status Status { get; set; }

        /// <summary>
        /// Size of the trash (possible values defined by <see cref="Enums.Size"/>).
        /// </summary>
        [DisplayName("Mennyiség")]
        public Size Size { get; set; }

        /// <summary>
        /// Logical values of different types of waste found at the point
        /// (possible values defined by <see cref="Enums.TrashType"/>).
        /// </summary>
        [DisplayName("Szeméttípusok")]
        public TrashType Types { get; set; }

        /// <summary>
        /// Logical values of different accessibilities of the trash
        /// (possible values defined by <see cref="Enums.Accessibility"/>).
        /// </summary>
        [DisplayName("Hozzáférhetőség")]
        public Accessibility Accessibilities { get; set; }

        /// <summary>
        /// A list of images linked to the point, made about the trash.
        /// </summary>
        public virtual List<TrashImage> Images { get; set; } = [];

        public Trash() { }

        /// <summary>
        /// Creates a <see cref="Trash"/> from the given <see cref="TrashFromTrashout"/>.
        /// </summary>
        /// <param name="trashFromTrashout">
        /// The <see cref="TrashFromTrashout"/> object to copy values from.
        /// </param>
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

        /// <summary>
        /// Creates a <see cref="Trash"/> from the given <see cref="TrashFromTrashout"/>.
        /// </summary>
        /// <param name="trashFromUser">
        /// The <see cref="TrashFromUser"/> object to copy values from.
        /// </param>
        /// <param name="userId">The user's id to create the point with.</param>
        public Trash(TrashFromUser trashFromUser, String userId)
        {
            var gf = NetTopologySuite.NtsGeometryServices.Instance.CreateGeometryFactory(WGS_SRID);

            TrashoutId = null;
            UserId = userId;

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

            Accessibilities |= (trashFromUser.ByCar ? Accessibility.ByCar : 0)
                | (trashFromUser.InCave ? Accessibility.InCave : 0)
                | (trashFromUser.UnderWater ? Accessibility.UnderWater : 0)
                | (trashFromUser.NotForGeneralCleanup ? Accessibility.NotForGeneralCleanup : 0);

            Types |= (trashFromUser.Automotive ? TrashType.Automotive : 0)
                | (trashFromUser.Construction ? TrashType.Construction : 0)
                | (trashFromUser.Dangerous ? TrashType.Dangerous : 0)
                | (trashFromUser.DeadAnimals ? TrashType.DeadAnimals : 0)
                | (trashFromUser.Domestic ? TrashType.Domestic : 0)
                | (trashFromUser.Electronic ? TrashType.Electronic : 0)
                | (trashFromUser.Glass ? TrashType.Glass : 0)
                | (trashFromUser.Liquid ? TrashType.Liquid : 0)
                | (trashFromUser.Metal ? TrashType.Metal : 0)
                | (trashFromUser.Organic ? TrashType.Organic : 0)
                | (trashFromUser.Plastic ? TrashType.Plastic : 0);


            Images = trashFromUser.Images
                .Where(i => i != null)
                .Select(i => new TrashImage(userId, i))
                .ToList();
        }

        /// <summary>
        /// Updates <see langword="this"/>' values with <paramref name="trash"/>'s.
        /// </summary>
        /// <param name="trash">An instance of <see cref="Trash"/> with the new values.</param>
        /// <returns>Returns <see langword="this"/> with it's updated values.</returns>
        public Trash Update(Trash trash)
        {
            TrashoutId = trash.TrashoutId;
            UserId = trash.UserId;
            Location = trash.Location;
            Country = trash.Country;
            Locality = trash.Locality;
            SubLocality = trash.SubLocality;
            CreateTime = trash.CreateTime;
            UpdateTime = trash.UpdateTime;
            UpdateNeeded = trash.UpdateNeeded;
            Note = trash.Note;
            Status = trash.Status;
            Size = trash.Size;
            Types = trash.Types;
            Accessibilities = trash.Accessibilities;

            Images = [];
            foreach(var image in trash.Images) 
                Images.Append(image);

            return this;
        }
        public override Boolean Equals(Object? obj)
        {
            if (obj == null || obj.GetType() != GetType())
                return false;

            return GetType().GetProperties().ToList()
                .Where(p => p.GetValue(this) != null && obj.GetType().GetProperty(p.Name)!.GetValue(obj) != null)
                .All(p => p.GetValue(this)!.Equals(obj.GetType().GetProperty(p.Name)!.GetValue(obj)));
        }

        private const Int32 WGS_SRID = 4326;
    }
}
