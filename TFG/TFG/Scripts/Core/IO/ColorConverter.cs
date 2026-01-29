using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Xna.Framework;

namespace TFG.Scripts.Core.IO;

public class ColorConverter : JsonConverter<Color>
{
    public override Color Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException("Expected StartObject token");
        
        float r = 255, g = 255, b = 255, a = 255;
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
                return new Color(r, g, b, a);

            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                string propertyName = reader.GetString();
                reader.Read();
                switch (propertyName.ToUpperInvariant())
                {
                    case "R":
                        r = reader.GetSingle();
                        break;
                    case "G":
                        g = reader.GetSingle();
                        break;
                    case "B":
                        b = reader.GetSingle();
                        break;
                    case "A":
                        a = reader.GetSingle();
                        break;
                }
            }
        }
        throw new JsonException("Unexpected end of JSON.");
    }

    public override void Write(Utf8JsonWriter writer, Color value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteNumber("R", value.R);
        writer.WriteNumber("G", value.G);
        writer.WriteNumber("B", value.B);
        writer.WriteNumber("A", value.A);
        writer.WriteEndObject();
    }
}