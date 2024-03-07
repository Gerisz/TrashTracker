namespace TrashTracker.Web.Models.Enums
{

    /// <summary>
    /// Trash's accessibilities, starting from 1
    /// </summary>
    public enum Accessibility
    {
        /// <summary>
        /// Can be accessed by a car
        /// </summary>
        ByCar = 1 << 0,

        /// <summary>
        /// Found in a cave
        /// </summary>
        InCave = 1 << 1,

        /// <summary>
        /// Found underwater
        /// </summary>
        UnderWater = 1 << 2,

        /// <summary>
        /// General cleanup is not enough
        /// </summary>
        NotForGeneralCleanup = 1 << 3
    }
}
