using NetTopologySuite.IO.Converters;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TrashTracker.Web.Utils
{
    /// <summary>
    /// Util class for (de-)serializing JSON files (might be expanded in the future),
    /// without the need to give the options to the functions on every call
    /// </summary>
    public class Serializer
    {
        private static readonly ThreadLocal<JsonSerializerOptions> _options = new(() =>
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = new LowerCaseNamingPolicy()
            };
            options.Converters.Add(new GeoJsonConverterFactory());
            options.Converters.Add(new JsonStringEnumConverter());
            return options;
        });

        /// <inheritdoc cref="JsonSerializer.Serialize"/>
        public static String Serialize(Object value)
        {
            return JsonSerializer.Serialize(value, _options.Value);
        }

        /// <inheritdoc cref="JsonSerializer.SerializeAsync"/>
        public static async Task<String> SerializeAsync(Object toSerialize)
        {
            MemoryStream stream = new MemoryStream();
            await JsonSerializer.SerializeAsync(stream, toSerialize, _options.Value);
            return stream.ToString() ?? "{}";
        }

        /// <inheritdoc cref="JsonSerializer.Deserialize{T}"/>
        public static T Deserialize<T>(String json)
        {
            return JsonSerializer.Deserialize<T>(json, _options.Value)!;
        }

        /// <inheritdoc cref="JsonSerializer.DeserializeAsync{T}"/>
        public static async Task<T> DeserializeAsync<T>(String json)
        {
            return (await JsonSerializer.DeserializeAsync<T>(
                new MemoryStream(Encoding.UTF8.GetBytes(json)), _options.Value))!;
        }
    }

    /// <summary>
    /// Determines the naming policy used to convert a string-based name to a lower-case format.
    /// </summary>
    public class LowerCaseNamingPolicy : JsonNamingPolicy
    {
        public override String ConvertName(String name)
        {
            if (String.IsNullOrEmpty(name) || !Char.IsUpper(name[0]))
                return name;

            return name.ToLower();
        }
    }
}
