using TrashTracker.Data.Models.Enums;
using TrashTracker.Data.Models.Tables;

namespace TrashTracker.Data.Models.DTOs.In
{
    public class TrashFromUser
    {
        public Double Lat { get; set; }
        public Double Long { get; set; }
        public String? Locality { get; set; }
        public String? SubLocality { get; set; }
        public List<TrashFromUserAccessibility> Accessibilities { get; set; } = [];
        public Size Size { get; set; }
        public List<TrashFromUserTrashType> Types { get; set; } = [];
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
