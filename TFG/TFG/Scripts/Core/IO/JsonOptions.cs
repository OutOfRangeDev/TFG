using System.Text.Json;
using System.Text.Json.Serialization;

namespace TFG.Scripts.Core.IO;

public class JsonOptions
{
    public static readonly JsonSerializerOptions Default = new()
    {
        // Will make it ignore the difference between camelCase and PascalCase.
        PropertyNameCaseInsensitive = true,
        // Makes the JSON human-readable.
        WriteIndented = true,
        // Register our custom converters.
        Converters = 
        {
            new JsonStringEnumConverter(),
            new ColorConverter(),
            new Vector2Converter(),
            new RectangleConverter(),
        }
    };
}