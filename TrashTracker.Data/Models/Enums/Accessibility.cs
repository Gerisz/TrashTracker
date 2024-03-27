using System.ComponentModel.DataAnnotations;

namespace TrashTracker.Data.Models.Enums
{

    /// <summary>
    /// Trash's accessibilities
    /// </summary>
    [Flags]
    public enum Accessibility
    {
        /// <summary>
        /// Can be accessed by a car
        /// </summary>
        [Display(Name = "Autóval megközelíthető",
            ShortName = "fa-solid fa-car")]
        ByCar = 1 << 0,

        /// <summary>
        /// Found in a cave
        /// </summary
        [Display(Name = "Barlangban található",
            ShortName = "fa-solid fa-dungeon")]
        InCave = 1 << 1,

        /// <summary>
        /// Found underwater
        /// </summary>
        [Display(Name = "Víz alatt/vízparton található",
            ShortName = "fa-solid fa-water")]
        UnderWater = 1 << 2,

        /// <summary>
        /// General cleanup is not enough
        /// </summary>
        [Display(Name = "Egyszerű takarítás nem elégséges",
            ShortName = "fa-solid fa-triangle-exclamation")]
        NotForGeneralCleanup = 1 << 3
    }
}
