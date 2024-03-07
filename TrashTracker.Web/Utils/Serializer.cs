using System.Text;
using System.Text.Json;

namespace TrashTracker.Web.Utils
{
    /// <summary>
    /// Util class for (de-)serializing JSON files (might be expanded in the future),
    /// without the need to give the options to the functions on every call
    /// </summary>
    public class Serializer
    {
        private static JsonSerializerOptions _options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        public static String Serialize(Object value)
        {
            return JsonSerializer.Serialize(value, _options);
        }

        public static async Task<String> SerializeAsync(Object toSerialize)
        {
            MemoryStream stream = new MemoryStream();
            await JsonSerializer.SerializeAsync(stream, toSerialize, _options);
            return stream.ToString() ?? "{}";
        }

        public static T Deserialize<T>(String json)
        {
            return JsonSerializer.Deserialize<T>(json, _options)!;
        }

        public static async Task<T> DeserializeAsync<T>(String json)
        {
            return (await JsonSerializer.DeserializeAsync<T>(new MemoryStream(Encoding.UTF8.GetBytes(json)), _options))!;
        }
    }
}
