using System.ComponentModel.DataAnnotations;

namespace TrashTracker.Data.Models.Defaults
{
    /// <summary>
    /// Contains the default roles in ascending order by hierarchy
    /// </summary>
    public enum Roles
    {
        [Display(Name = "Felhasználó", ShortName = "fa-solid fa-user")]
        User,
        [Display(Name = "Moderátor", ShortName = "fa-solid fa-user-pen")]
        Moderator,
        [Display(Name = "Adminisztrátor", ShortName = "fa-solid fa-user-gear")]
        Admin
    }
}
