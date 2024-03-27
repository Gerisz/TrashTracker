using System.ComponentModel.DataAnnotations;

namespace TrashTracker.Data.Models.Enums
{

    /// <summary>
    /// Sizes of trash, starting from 1
    /// </summary>
    public enum Size
    {
        /// <summary>
        /// Fits in a bag
        /// </summary>
        [Display(Name = "Elfér egy zsákban",
            ShortName = "fa-solid fa-box-open")]
        Bag = 1,

        /// <summary>
        /// Fits in a wheelbarrow
        /// </summary>
        [Display(Name = "Elfér egy talicskában",
            ShortName = "fa-solid fa-trailer")]
        Wheelbarrow,

        /// <summary>
        /// A car is required
        /// </summary>
        [Display(Name = "Autóra van szükség",
            ShortName = "fa-solid fa-car")]
        Car
    }
}
