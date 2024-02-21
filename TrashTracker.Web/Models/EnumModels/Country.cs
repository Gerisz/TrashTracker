using System.ComponentModel.DataAnnotations.Schema;

namespace TrashTracker.Web.Models.EnumModels
{
    public enum CountryEnum
    {
        Hungary = 1
    }

    public class Country : EnumTable
    {
        [NotMapped]
        public static Type EnumType { get; } = typeof(CountryEnum);
    }
}