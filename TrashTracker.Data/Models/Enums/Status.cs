namespace TrashTracker.Data.Models.Enums
{

    /// <summary>
    /// Statuses of trash, starting from 1
    /// </summary>
    public enum Status
    {
        /// <summary>
        /// Already cleaned up
        /// </summary>
        Cleaned = 1,

        /// <summary>
        /// Less trash, but still there
        /// </summary>
        Less,

        /// <summary>
        /// More trash since creation
        /// </summary>
        More,

        /// <summary>
        /// Trash still there
        /// </summary>
        StillHere
    }
}
