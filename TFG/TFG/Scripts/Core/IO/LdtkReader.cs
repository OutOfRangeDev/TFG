using System.IO;
using System.Text.Json;

namespace TFG.Scripts.Core.IO;

public class LdtkReader
{
    public static LdtkProject LoadFromFile(string path)
    {
        //Path to the file, and read it.
        string Json = File.ReadAllText(path);
        // Deserialize the JSON string into a LdtkProject object.
        LdtkProject projectData = JsonSerializer.Deserialize<LdtkProject>(Json);
        // Return the project data.
        return projectData;
    }
}