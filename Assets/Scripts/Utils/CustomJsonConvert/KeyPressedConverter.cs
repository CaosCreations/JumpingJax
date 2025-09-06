using Newtonsoft.Json;
using System;
using UnityEngine;

public class KeyPressedConverter : JsonConverter<KeysPressed>
{
    public override KeysPressed ReadJson(JsonReader reader, Type objectType, KeysPressed existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var keysPressed = new KeysPressed();
        keysPressed.SetFlags(Convert.ToInt32(reader.Value));
        return keysPressed;
    }

    public override void WriteJson(JsonWriter writer, KeysPressed value, JsonSerializer serializer)
    {
        var flags = value.GetFlags();
        writer.WriteValue(flags);
    }
}