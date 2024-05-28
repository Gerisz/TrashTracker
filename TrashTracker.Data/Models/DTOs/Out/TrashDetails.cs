using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using TrashTracker.Data.Models.Enums;
using TrashTracker.Data.Models.Tables;
using System.Linq.Expressions;

namespace TrashTracker.Data.Models.DTOs.Out
{
    /// <summary>
    /// DTO meant to define every relevant information of a <see cref="Trash"/> object
    /// showed on a trash's details page.
    /// </summary>
    public class TrashDetails
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
        /// The username of the point's creator (if created by a user, otherwise it's null).
        /// </summary>
        public String? UserName { get; set; }

        /// <summary>
        /// Latitude of the location (must be between -180.0 and 180.0).
        /// </summary>
        [DisplayName("Szélesség")]
        public Double Latitude { get; set; }

        /// <summary>
        /// Longitude of the location (must be between -180.0 and 180.0).
        /// </summary>
        [DisplayName("Hosszúság")]
        public Double Longitude { get; set; }

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
        /// Full download URLs of images linked to the point, made about the trash.
        /// </summary>
        [DisplayName("Képek")]
        public IEnumerable<Uri> Images { get; set; } = [];

        /// <summary>
        /// Projects a <see cref="Trash"/> to a <see cref="TrashDetails"/> object.
        /// </summary>
        /// <param name="imageUrlBase">
        /// The base of the image's URL (which endpoint the image can be downloaded from).
        /// </param>
        public static Expression<Func<Trash, TrashDetails>> Projection
            (String imageUrlBase) => trash => new TrashDetails()
            {
                Id = trash.Id,
                TrashoutId = trash.TrashoutId,
                UserName = trash.User != null ? trash.User.UserName : null,
                Latitude = trash.Location.X,
                Longitude = trash.Location.Y,
                Country = trash.Country,
                Locality = trash.Locality,
                SubLocality = trash.SubLocality,
                CreateTime = trash.CreateTime,
                UpdateTime = trash.UpdateTime,
                UpdateNeeded = trash.UpdateNeeded,
                Note = trash.Note,
                Status = trash.Status,
                Size = trash.Size,
                Types = trash.Types,
                Accessibilities = trash.Accessibilities,
                Images = trash.Images.Select(i => i.Url ?? new Uri($"{imageUrlBase}/{i.Id}"))
            };

        /// <summary>
        /// Creates a <see cref="TrashDetails"/> from a <see cref="Trash"/> object.
        /// </summary>
        /// <param name="trash">The <see cref="Trash"/> object to copy values from.</param>
        /// <param name="imageUrlBase">
        /// The base of the image's URL (which endpoint the image can be downloaded from).
        /// </param>
        /// <returns>The created <see cref="TrashDetails"/> from <paramref name="trash"/>.</returns>
        public static TrashDetails Create(Trash trash, String imageUrlBase)
            => Projection(imageUrlBase).Compile().Invoke(trash);

        public override Boolean Equals(Object? obj)
        {
            if (obj == null || obj.GetType() != GetType())
                return false;

            return GetType().GetProperties().ToList()
                .Where(p => p.GetValue(this) != null && obj.GetType().GetProperty(p.Name)!.GetValue(obj) != null)
                .All(p => p.GetValue(this)!.Equals(obj.GetType().GetProperty(p.Name)!.GetValue(obj)));
        }
    }
}
