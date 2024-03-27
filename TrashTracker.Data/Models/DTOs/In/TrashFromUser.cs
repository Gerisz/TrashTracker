using TrashTracker.Data.Models.Enums;

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
        public List<TrashFromUserType> Types { get; set; } = [];
        public String? Note { get; set; }

        public TrashFromUser()
        {
            Accessibilities = Enum.GetValues<Accessibility>()
                .Select(v => new TrashFromUserAccessibility()
                {
                    Accessibility = v,
                    IsSelected = false
                }).ToList();
            Types = Enum.GetValues<TrashType>()
                .Select(v => new TrashFromUserType()
                {
                    Type = v,
                    IsSelected = false
                }).ToList();
        }
    }

    public class TrashFromUserAccessibility
    {
        public Accessibility Accessibility { get; set; }
        public Boolean IsSelected { get; set; }
    }

    public class TrashFromUserType
    {
        public TrashType Type { get; set; }
        public Boolean IsSelected { get; set; }
    }
}
