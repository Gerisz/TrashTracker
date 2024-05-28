namespace TrashTracker.Data.Models.DTOs.In
{
    /// <summary>
    /// DTO meant to contain a trash's data received from TrashOut.
    /// </summary>
    public class TrashFromTrashout
    {
        /// <summary>
        /// Id of the point on TrashOut servers.
        /// </summary>
        public Int32 Id { get; set; }

        /// <summary>
        /// Information about the location of the point.
        /// </summary>
        public TrashFromTrashoutGps Gps { get; set; } = null!;

        /// <summary>
        /// Date and time when the point was created.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Date and time when the point was last updated.
        /// </summary>
        public DateTime UpdateTime { get; set; }

        /// <summary>
        /// Logical value of the point is in need of update.
        /// </summary>
        public Int32 UpdateNeeded { get; set; }

        /// <summary>
        /// Additional information given about the trash.
        /// </summary>
        public String? Note { get; set; }

        /// <summary>
        /// Current status of the trash (possible values defined by <see cref="Enums.Status"/>).
        /// </summary>
        public String? Status { get; set; }

        /// <summary>
        /// Size of the trash (possible values defined by <see cref="Enums.Size"/>).
        /// </summary>
        public String? Size { get; set; }

        /// <summary>
        /// Images linked to the pont, made about the trash.
        /// </summary>
        public List<TrashFromTrashoutImages>? Images { get; set; }

        /// <summary>
        /// List of trashes found at the point
        /// (possible values defined by <see cref="Enums.TrashType"/>).
        /// </summary>
        public List<String>? Types { get; set; }

        /// <summary>
        /// Logical values of different accessibilities of the trash
        /// (possible values defined by <see cref="Enums.Accessibility"/>).
        /// </summary>
        public TrashFromTrashoutAccessibility? Accessibility { get; set; }
    }

    /// <summary>
    /// A <see langword="class"/> containing information about the location of the point.
    /// </summary>
    public class TrashFromTrashoutGps
    {
        /// <summary>
        /// Latitude of the location (must be between -180.0 and 180.0).
        /// </summary>
        public Double Lat { get; set; }

        /// <summary>
        /// Longitude of the location (must be between -180.0 and 180.0).
        /// </summary>
        public Double Long { get; set; }

        /// <summary>
        /// Additional information about the location of the point (e. g. locality).
        /// </summary>
        public TrashFromTrashoutGpsArea? Area { get; set; }
    }

    /// <summary>
    /// A <see langword="class"/> containing additional information
    /// about the location of the point (e. g. locality).
    /// </summary>
    public class TrashFromTrashoutGpsArea
    {
        /// <summary>
        /// The country of the location (possible values defined by <see cref="Enums.Country"/>).
        /// </summary>
        public String? Country { get; set; }

        /// <summary>
        /// Locality of the location, typically in which settlement's borders is the point is in.
        /// </summary>
        public String? Locality { get; set; }

        /// <summary>
        /// Sublocality of the location, typically in which district is the point is in.
        /// </summary>
        public String? SubLocality { get; set; }
    }

    /// <summary>
    /// A <see langword="class"/> containing a point's image's full download url.
    /// </summary>
    public class TrashFromTrashoutImages
    {
        /// <summary>
        /// Url of the image, from which the image can be downloaded.
        /// </summary>
        public Uri? FullDownloadUrl { get; set; }
    }

    /// <summary>
    /// A <see langword="class"/> containing logical values of different accessibilities of a trash
    /// (possible values defined by <see cref="Enums.Accessibility"/>).
    /// </summary>
    public class TrashFromTrashoutAccessibility
    {
        /// <summary>
        /// Logical value of the point's accessibility by car.
        /// </summary>
        public Boolean ByCar { get; set; }

        /// <summary>
        /// Logical value of the point being in a cave.
        /// </summary>
        public Boolean InCave { get; set; }

        /// <summary>
        /// Logical value of the point being underwater.
        /// </summary>
        public Boolean UnderWater { get; set; }

        /// <summary>
        /// Logical value of the point not being able to be cleaned up by general means.
        /// </summary>
        public Boolean NotForGeneralCleanup { get; set; }
    }
}