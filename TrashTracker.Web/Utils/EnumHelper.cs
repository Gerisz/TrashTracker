using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace TrashTracker.Web.Utils
{
    /// <summary>
    /// A <see langword="static"/> <see langword="class"/> containing several functions,
    /// to help bind <see langword="enum"/>s and their values to show in views.
    /// </summary>
    /// <typeparam name="TEnum"></typeparam>
    public static class EnumHelper<TEnum>
        where TEnum : struct, Enum
    {
        /// <summary>
        /// Gets all possible values of an <see cref="Enum"/>
        /// and returns then into an <see cref="IList{TEnum}"/>
        /// </summary>
        /// <param name="value">An <see cref="Enum"/> to get values of.</param>
        /// <returns>All the possible values of the given <paramref name="value"/>.</returns>
        public static IList<TEnum> GetValues(Enum value)
        {
            var enumValues = new List<TEnum>();

            foreach (FieldInfo fi in value.GetType()
                .GetFields(BindingFlags.Static | BindingFlags.Public))
            {
                enumValues.Add((TEnum)Enum.Parse(value.GetType(), fi.Name, false));
            }
            return enumValues;
        }

        /// <summary>
        /// Parses a given <see cref="String"/> to an <see cref="Enum"/>.
        /// </summary>
        /// <param name="value">A <see cref="String"/> to parse for.</param>
        /// <returns>
        /// The <see cref="Enum"/> parsed from the given <paramref name="value"/>.
        /// </returns>
        public static TEnum Parse(String value)
        {
            return (TEnum)Enum.Parse(typeof(TEnum), value, true);
        }

        /// <summary>
        /// Gets all names of the given <see cref="Enum"/>.
        /// </summary>
        /// <param name="value">An <see cref="Enum"/> to get names of.</param>
        /// <returns>All the names of the given <paramref name="value"/>.</returns>
        public static IList<String> GetNames(Enum value)
        {
            return value.GetType()
                .GetFields(BindingFlags.Static | BindingFlags.Public)
                .Select(fi => fi.Name)
                .ToList();
        }

        /// <summary>
        /// Get all display values for a given <see cref="Enum"/>.
        /// </summary>
        /// <param name="value">An <see cref="Enum"/> to get display names of.</param>
        /// <returns>All the display names of the given <paramref name="value"/>.</returns>
        public static IList<String> GetDisplayValues(Enum value)
        {
            return GetNames(value).Select(obj => GetDisplayValue(Parse(obj))).ToList();
        }

        private static String LookupResource(Type resourceManagerProvider, String resourceKey)
        {
            var resourceKeyProperty = resourceManagerProvider.GetProperty(resourceKey,
                BindingFlags.Static | BindingFlags.Public, null, typeof(String), [], null);

            if (resourceKeyProperty != null)
                return (String)resourceKeyProperty.GetMethod!.Invoke(null, null)!;

            return resourceKey;
        }

        /// <summary>
        /// Get the display value for the given <typeparamref name="TEnum"/>.
        /// </summary>
        /// <param name="value">
        /// A value of <typeparamref name="TEnum"/> to get display name of.
        /// </param>
        /// <returns>The display name of <paramref name="value"/>'s value.</returns>
        public static String GetDisplayValue(TEnum value)
        {
            var fieldInfo = value.GetType().GetField(value.ToString());

            var descriptionAttributes = fieldInfo.GetCustomAttributes(
                typeof(DisplayAttribute), false) as DisplayAttribute[];

            if (descriptionAttributes![0].ResourceType != null)
                return LookupResource(descriptionAttributes[0].ResourceType!,
                    descriptionAttributes[0].Name!);

            if (descriptionAttributes == null)
                return String.Empty;

            return ((descriptionAttributes.Length > 0)
                ? descriptionAttributes[0].Name
                : value.ToString())!;
        }

        /// <summary>
        /// Get the display vicon class for the given <typeparamref name="TEnum"/>.
        /// </summary>
        /// <param name="value">
        /// A value of <typeparamref name="TEnum"/> to get display icon class of.
        /// </param>
        /// <returns>The display icon class of <paramref name="value"/>'s value.</returns>
        public static String GetDisplayIconClass(TEnum value)
        {
            var fieldInfo = value.GetType().GetField(value.ToString());

            var descriptionAttributes = fieldInfo.GetCustomAttributes(
                typeof(DisplayAttribute), false) as DisplayAttribute[];

            if (descriptionAttributes![0].ResourceType != null)
                return LookupResource(descriptionAttributes[0].ResourceType!,
                    descriptionAttributes[0].ShortName!);

            if (descriptionAttributes == null)
                return String.Empty;

            return ((descriptionAttributes.Length > 0)
                ? descriptionAttributes[0].ShortName
                : value.ToString())!;
        }
    }
}
