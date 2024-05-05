using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using TrashTracker.Data.Models.Enums;
using TrashTracker.Data.Models.Tables;

namespace TrashTracker.Data.Models.DTOs.In
{
    public class TrashFromUser 
    {
        [DisplayFormat(DataFormatString = "{0:0.00}", ApplyFormatInEditMode = true)]
        [DisplayName("Szélesség")]
        [Localizable(false)]
        [Range(-180.0, 180.0,
            ErrorMessage = "A koordináta nem lehet kevesebb, mint {0} és több, mint {1}.")]
        [Required(ErrorMessage = "Szélesség megadása kötelező!")]
        public Double Lat { get; set; }
        [DisplayFormat(DataFormatString = "{0:0.00}", ApplyFormatInEditMode = true)]
        [DisplayName("Hosszúság")]
        [Localizable(false)]
        [Range(-180.0, 180.0,
            ErrorMessage = "A koordináta nem lehet kevesebb, mint -180 és több, mint 180.")]
        [Required(ErrorMessage = "Hosszúság megadása kötelező!")]
        public Double Long { get; set; }
        [DisplayName("Település")]
        public String? Locality { get; set; }
        [DisplayName("Településrész")]
        public String? SubLocality { get; set; }
        [DisplayName("Hozzáférhetőség")]
        public List<TrashFromUserAccessibility> Accessibilities { get; set; } = [];
        [DisplayName("Méret")]
        [Required(ErrorMessage = "Méret megadása közelező!")]
        public Size Size { get; set; }
        [DisplayName("Szeméttípus")]
        public List<TrashFromUserTrashType> Types { get; set; } = [];
        [DisplayName("Megjegyzés")]
        [StringLength(2000, ErrorMessage = "{0} karaternél nem lehet hosszabb a megjegyzés!")]
        public String? Note { get; set; }

        public TrashFromUser()
        {
            Accessibilities = Enum.GetValues<Accessibility>()
                .Select(v => new TrashFromUserAccessibility(v, false))
                .ToList();
            Types = Enum.GetValues<TrashType>()
                .Select(v => new TrashFromUserTrashType(v, false))
                .ToList();
        }

        protected TrashFromUser(Trash trash)
        {
            Lat = trash.Location.X;
            Long = trash.Location.Y;
            Locality = trash.Locality;
            SubLocality = trash.Locality;
            Accessibilities = Enum.GetValues<Accessibility>()
                .Select(v => new TrashFromUserAccessibility(v, (trash.Accessibilities & v) == 0))
                .ToList();
            Types = Enum.GetValues<TrashType>()
                .Select(v => new TrashFromUserTrashType(v, (trash.Types & v) == 0))
                .ToList();
        }
    }

    public class TrashFromUserAccessibility
    {
        public Accessibility Value { get; set; }
        public Boolean IsSelected { get; set; }

        public TrashFromUserAccessibility(Accessibility value, Boolean isSelected)
        {
            Value = value;
            IsSelected = isSelected;
        }
    }

    public class TrashFromUserTrashType
    {
        public TrashType Value { get; set; }
        public Boolean IsSelected { get; set; }

        public TrashFromUserTrashType(TrashType value, Boolean isSelected)
        {
            Value = value;
            IsSelected = isSelected;
        }
    }
}
