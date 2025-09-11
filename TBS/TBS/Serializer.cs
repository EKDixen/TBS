using System.Text.Json;

public static class Serializer
{
    private static readonly JsonSerializerOptions options = new JsonSerializerOptions
    {
        IncludeFields = true
    };

    public static string ToJson<T>(T obj)
    {
        return JsonSerializer.Serialize(obj, options);
    }

    public static T FromJson<T>(string json)
    {
        return JsonSerializer.Deserialize<T>(json, options);
    }
}
