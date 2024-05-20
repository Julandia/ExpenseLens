using System.Text.Json;
using System.Text.Json.Serialization;

namespace Deploy.Infrastructure.Extensions;

public static class SerializationExtensions
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new JsonStringEnumConverter() },
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    public static string Serialize(this object value)
    {
        return JsonSerializer.Serialize(value, Options);
    }

    public static T Deserialize<T>(this string value) where T : notnull
    {
        return JsonSerializer.Deserialize<T>(value, Options)!;
    }
}
