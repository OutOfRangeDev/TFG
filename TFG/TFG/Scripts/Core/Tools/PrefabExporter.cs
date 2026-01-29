using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using TFG.Scripts.Core.Data;
using TFG.Scripts.Core.IO;
using TFG.Scripts.Game.Prefabs;

namespace TFG.Scripts.Core.Tools;

public class PrefabExporter
{
    public static void ExportAllPrefabs(string outputDirectory)
    {
        // 1. Get all the prefabs to export.
        var blueprintsToExport = new List<PrefabBlueprint>
        {
            EntityFactory.CreatePlayerPrefab(),
        };
        
        // 2. Make sure the output directory exists.
        Directory.CreateDirectory(outputDirectory);
        
        // 4. Export each blueprint to a JSON file.
        foreach (var blueprint in blueprintsToExport)
        {
            string jsonString = JsonSerializer.Serialize(blueprint, JsonOptions.Default);
            string filepath = Path.Combine(outputDirectory, $"{blueprint.Name}.json");
            File.WriteAllText(filepath, jsonString);
            Console.WriteLine($"Exported {blueprint.Name} to {filepath} successfully.");
        }
    }
}