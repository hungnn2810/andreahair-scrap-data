using System.Text.Json;
using System.Text.Json.Serialization;

namespace CrawlData.Model
{
    public static class JsonExtensions
    {
        private static JsonSerializerOptions _defaultOptions;

        private static JsonSerializerOptions DefaultOptions => _defaultOptions ??= CreateDefaultOptions();

        private static JsonSerializerOptions CreateDefaultOptions()
        {
            var setting = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            return setting;
        }

        public static string ToJson<T>(this T obj) where T : class
        {
            return JsonSerializer.Serialize(obj, DefaultOptions);
        }

        public static T ToObject<T>(this string json)
        {
            return JsonSerializer.Deserialize<T>(json, DefaultOptions);
        }
    }
}