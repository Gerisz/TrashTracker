using System.ComponentModel.DataAnnotations;

namespace TrashTracker.Data.Models.Attributes
{
    /// <summary>
    /// Validates a <see cref="Double"/>, if it's not null and is between -180 and 180.
    /// </summary>
    public class CoordinateAttribute : ValidationAttribute
    {
        public override Boolean IsValid(Object? value)
        {
            if (value != null && value is Double coordinate)
                return -180.0 < coordinate && coordinate < 180.0;

            return false;
        }
    }
}
