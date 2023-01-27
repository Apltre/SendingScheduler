using System.Text.Json;
using System.Text.Json.Serialization;

namespace SendingScheduler.Core.Helpers;

public class JsonHelper
{
    public readonly static JsonSerializerOptions IgnoreNullsOption = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };
    public static object? GetControllerDeserializedParameter(JsonElement? data, Type type)
    {
        if (data is null)
        {
            return null;
        }

        if (type == typeof(string))
        {
            return data.Value.ToString();
        }

        return data.Value.Deserialize(type);
    }

    public static object? GetControllerDeserializedParameter(string? data, Type type)
    {
        if (data is null)
        {
            return null;
        }

        if (type == typeof(string))
        {
            return data;
        }

        return JsonSerializer.Deserialize(data, type);
    }
}
