using System.Text.Json;
using System.Text.Json.Serialization;

namespace cangulo.nuke.releasecreator.Constants
{
    public static class SerializerContants
    {
        public static JsonSerializerOptions SERIALIZER_OPTIONS = new JsonSerializerOptions
        {
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        public static JsonSerializerOptions DESERIALIZER_OPTIONS = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }
}
