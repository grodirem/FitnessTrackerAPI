using System.Text.Json;

namespace BLL.Extensions;

public static class JsonExtensions
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public static string ToJson<T>(this T obj) =>
        JsonSerializer.Serialize(obj, Options);

    public static T? FromJson<T>(this string json) =>
        JsonSerializer.Deserialize<T>(json, Options);
}
