using System.ComponentModel.DataAnnotations.Schema;

namespace CleanTiszaMap.Data.Models.EnumModels
{

    /// <summary>
    /// Types of possible trash, starting from 1
    /// </summary>
    public enum TrashType
    {
        Automotive = 1 << 0,
        Construction = 1 << 1,
        Dangerous = 1 << 2,
        DeadAnimals = 1 << 3,
        Domestic = 1 << 4,
        Electronic = 1 << 5,
        Glass = 1 << 6,
        Household = 1 << 7,
        Liquid = 1 << 8,
        Metal = 1 << 9,
        Organic = 1 << 10,
        Plastic = 1 << 11
    }
}
