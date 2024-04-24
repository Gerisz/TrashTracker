using System.ComponentModel.DataAnnotations;

namespace TrashTracker.Data.Models.Attributes
{
    /// <summary>
    /// Validates a <see cref="Double"/>, if it's not null and is between -90 and 90.
    /// </summary>
    public class CoordinateAttribute : ValidationAttribute
    {
        public override Boolean IsValid(Object? value)
        {
            if (value != null && value is Double coordinate)
                return -90.0 < coordinate && coordinate < 90.0;

            return false;
        }
    }
}
