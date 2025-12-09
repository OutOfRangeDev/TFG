using System.IO;
using System.Text.Json;

namespace TFG.Scripts.Core.IO;

public static class LDtkReader
{
    public static LDtkProject LoadFromFile(string path)
    {
        //Path to the file and read it.
        var json = File.ReadAllText(path);
        // Deserialize the JSON string into an LDtkProject object.
        var projectData = JsonSerializer.Deserialize<LDtkProject>(json);
        // Return the project data.
        return projectData;
    }
}