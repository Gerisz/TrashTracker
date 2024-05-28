using System.ComponentModel.DataAnnotations;

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
        [Display(Name = "Megtisztítva",
            ShortName = "fa-solid fa-circle-check")]
        Cleaned = 1,

        /// <summary>
        /// Less trash, but still there
        /// </summary>
        [Display(Name = "Kevesebb",
            ShortName = "fa-solid fa-circle-minus")]
        Less,

        /// <summary>
        /// More trash since creation
        /// </summary>
        [Display(Name = "Több",
            ShortName = "fa-solid fa-circle-plus")]
        More,

        /// <summary>
        /// Trash still there
        /// </summary>
        [Display(Name = "Még mindig itt van",
            ShortName = "fa-solid fa-circle-xmark")]
        StillHere
    }
}
