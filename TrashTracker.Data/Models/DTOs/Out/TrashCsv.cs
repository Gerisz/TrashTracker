using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using TrashTracker.Data.Models.Enums;
using TrashTracker.Data.Models.Tables;
using System.Linq.Expressions;

namespace TrashTracker.Data.Models.DTOs.Out
{
    /// <summary>
    /// DTO meant to define every relevant information of a <see cref="Trash"/> object
    /// to be contained in a .csv file, when downloading them as one.
    /// </summary>
    public class TrashCsv
    {
        /// <summary>
        /// The id of the point in the database.
        /// </summary>
        public Int32 Id { get; set; }

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
        /// Projects a <see cref="Trash"/> to a <see cref="TrashCsv"/> object.
        /// </summary>
        public static Expression<Func<Trash, TrashCsv>> Projection { get; }
            = trash => new TrashCsv()
            {
                Id = trash.Id,
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
                Accessibilities = trash.Accessibilities
            };
    }
}
