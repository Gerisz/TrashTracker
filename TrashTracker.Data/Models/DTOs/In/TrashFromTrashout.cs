namespace TrashTracker.Data.Models.DTOs.In
{
    public class TrashFromTrashout
    {
        public Int32 Id { get; set; }
        public TrashFromTrashoutGps Gps { get; set; } = null!;
        public DateTime Created { get; set; }
        public DateTime UpdateTime { get; set; }
        public Int32 UpdateNeeded { get; set; }
        public String? Note { get; set; }
        public String? Status { get; set; }
        public String? Size { get; set; }
        public List<TrashFromTrashoutImages>? Images { get; set; }
        public List<String>? Types { get; set; }
        public TrashFromTrashoutAccessibility? Accessibility { get; set; }

        public class TrashFromTrashoutGps
        {
            public Double Lat { get; set; }
            public Double Long { get; set; }
            public TrashFromTrashoutGpsArea? Area { get; set; }
        }

        public class TrashFromTrashoutGpsArea
        {
            public String? Country { get; set; }
            public String? Locality { get; set; }
            public String? SubLocality { get; set; }
        }
        public class TrashFromTrashoutImages
        {
            public Uri? FullDownloadUrl { get; set; }
        }

        public class TrashFromTrashoutAccessibility
        {
            public Boolean ByCar { get; set; }
            public Boolean InCave { get; set; }
            public Boolean UnderWater { get; set; }
            public Boolean NotForGeneralCleanup { get; set; }
        }
    }
}
