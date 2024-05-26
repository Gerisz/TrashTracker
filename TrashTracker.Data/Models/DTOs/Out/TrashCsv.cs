using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using TrashTracker.Data.Models.Enums;
using TrashTracker.Data.Models.Tables;
using System.Linq.Expressions;

namespace TrashTracker.Data.Models.DTOs.Out
{
    public class TrashCsv
    {
        public Int32 Id { get; set; }

        [DisplayName("Koordináták")]
        public Double Latitude { get; set; }
        public Double Longitude { get; set; }

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
