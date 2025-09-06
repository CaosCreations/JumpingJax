using Newtonsoft.Json;
using System;

public class Vec3Converter : JsonConverter<Vec3>
{
    public override Vec3 ReadJson(JsonReader reader, Type objectType, Vec3 existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        return Vec3.FromString((string)reader.Value);
    }

    public override void WriteJson(JsonWriter writer, Vec3 value, JsonSerializer serializer)
    {
        writer.WriteValue(value.ToString());
    }
}
