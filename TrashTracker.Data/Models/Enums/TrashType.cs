using System.ComponentModel.DataAnnotations;

namespace TrashTracker.Data.Models.Enums
{

    /// <summary>
    /// Types of possible trash
    /// </summary>
    [Flags]
    public enum TrashType
    {
        [Display(Name = "Autóipari",
            ShortName = "fa-solid fa-car")]
        Automotive = 1 << 0,
        [Display(Name = "Építkezési",
            ShortName = "fa-solid fa-trowel-bricks")]
        Construction = 1 << 1,
        [Display(Name = "Veszélyes",
            ShortName = "fa-solid fa-skull-crossbones")]
        Dangerous = 1 << 2,
        [Display(Name = "Állati tetem",
            ShortName = "fa-solid fa-paw")]
        DeadAnimals = 1 << 3,
        [Display(Name = "Háztartási",
            ShortName = "fa-solid fa-house")]
        Domestic = 1 << 4,
        [Display(Name = "Elektronikai",
            ShortName = "fa-solid fa-computer")]
        Electronic = 1 << 5,
        [Display(Name = "Üveg",
        ShortName = "fa-solid fa-wine-glass-empty")]
        Glass = 1 << 6,
        [Display(Name = "Folyékony",
            ShortName = "fa-solid fa-droplet")]
        Liquid = 1 << 7,
        [Display(Name = "Fém",
            ShortName = "fa-solid fa-magnet")]
        Metal = 1 << 8,
        [Display(Name = "Szerves",
            ShortName = "fa-solid fa-seedling")]
        Organic = 1 << 9,
        [Display(Name = "Műanyag",
            ShortName = "fa-solid fa-bottle-water")]
        Plastic = 1 << 10
    }
}
