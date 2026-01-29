using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TFG.Scripts.Core.Components;
using TFG.Scripts.Core.Data;
using TFG.Scripts.Core.IO;

namespace TFG.Scripts.Core.Managers;

public class PrefabManager(AssetManager assetManager)
{
    // We create a dictionary to store the prefabs.
    private readonly Dictionary<string, PrefabBlueprint> _blueprints = new();

    // Load all prefabs from the specified directory.
    public void LoadPrefabs(string directoryPath)
    {
        // Check if the directory exists. If it doesn't, alert and return.
        if (!Directory.Exists(directoryPath))
        {
            Console.WriteLine($"[ERROR] Prefab directory '{directoryPath}' does not exist or not found.");
            return;
        }
        
        // If the directory exists, look at all the JSON files in it, even inside the subdirectories.
        var prefabFiles = Directory.GetFiles(directoryPath, "*.json", SearchOption.AllDirectories);

        // For each prefab file, load it and add it to the dictionary.
        foreach (var filePath in prefabFiles)
        {
            // Read the file.
            var jsonText = File.ReadAllText(filePath);

            // Deserialize the JSON into a PrefabBlueprint object.
            var blueprint = JsonSerializer.Deserialize<PrefabBlueprint>(jsonText);

            // If it has a blueprint name, and it isn't empty, add it to the dictionary.
            if (blueprint != null && !string.IsNullOrEmpty(blueprint.Name))
            {
                _blueprints[blueprint.Name] = blueprint;
                Console.WriteLine($"Loaded prefab '{blueprint.Name}' from '{filePath}'.");
            }
            else
            {
                Console.WriteLine($"[WARNING] Prefab '{filePath}' has no name or is empty.");
            }
        }
    }

    // Instantiate a prefab.
    public Entity InstantiatePrefab(string prefabName, World world, Vector2 position)
    {
        // Check if the prefab exists. If it doesn't, throw an exception.
        if (!_blueprints.TryGetValue(prefabName, out var blueprint))
        {
            throw new Exception($"[PrefabManager] Could not find a prefab blueprint named '{prefabName}'.");
        }
        
        // But if the prefab exists, first we create an entity.
        var entity = world.CreateEntity();
        Debug.WriteLine($"[PrefabManager] Creating entity {entity.Id} for prefab '{prefabName}'.");
        
        // Then we add all the components to it.
        foreach (var component in blueprint.Components)
        {
            // The component name is the key, and the component data are the values stored within the dictionary.
            var componentName = component.Key;
            var componentData = (JsonElement) component.Value;

            // As we don't know what type of component it is, we need to deserialize it dynamically.
            // We first create a generic IComponent variable to hold the instance.

            Debug.WriteLine($"--> Found component '{componentName}' in JSON.");
            
            // If the component is a SpriteComponent, we need to handle it differently.
            if (componentName == "SpriteComponent")
            {
                // Deserialize this component into a SpriteComponentBlueprint object.
                var spriteBlueprint = JsonSerializer.Deserialize<SpriteComponentBlueprint>(
                    componentData.GetRawText(),
                    JsonOptions.Default);

                // Then we load the texture from the asset manager.
                var texture = assetManager.Load<Texture2D>(spriteBlueprint.TextureName);

                if(texture == null) Debug.WriteLine($"[PrefabManager] Could not find texture '{spriteBlueprint.TextureName}' for prefab '{prefabName}'.");
                
                // And instantiate the SpriteComponent with the texture and other properties.
                var spriteComponent = new SpriteComponent{
                    Texture = texture, 
                    SourceRectangle = spriteBlueprint.SourceRectangle,
                    Color = spriteBlueprint.Color,
                    Rotation = spriteBlueprint.Rotation,
                    Origin = spriteBlueprint.Origin,
                    Scale = spriteBlueprint.Scale,
                    Effects = spriteBlueprint.Effects,
                    LayerDepth = spriteBlueprint.LayerDepth
                    };
                
                // And finally, add the component to the entity.
                world.AddComponent(entity, spriteComponent);
            }
            else if (componentName == "AnimatorComponent") 
            {
                // 1. Deserialize the JSON into our simple Blueprint object.
                var animBlueprint = JsonSerializer.Deserialize<AnimatorComponentBlueprint>(
                    componentData.GetRawText(),
                    JsonOptions.Default);

                // 2. MANUALLY BUILD the real AnimatorComponent.
                var animatorComponent = new AnimatorComponent
                {
                    // Create the real dictionary.
                    Animations = new Dictionary<string, Animation>(),
                    CurrentAnimation = animBlueprint.CurrentAnimationName,
                    // Reset the state for this new instance.
                    FrameTimer = 0f,
                    FrameIndex = 0
                };

                // 3. Loop through the blueprint's animations and create the REAL Animation structs.
                foreach (var animEntry in animBlueprint.Animations)
                {
                    var animName = animEntry.Key;
                    var animData = animEntry.Value;

                    Debug.WriteLine($"    ... adding animation '{animName}'");
                    
                    // Create the final, real Animation struct from the blueprint data.
                    var newAnimation = new Animation
                    {
                        Name = animData.Name,
                        RowIndex = animData.RowIndex,
                        FrameCount = animData.FrameCount,
                        FrameDuration = animData.FrameDuration,
                        Loop = animData.Loop // Use the correct property name
                    };

                    // Add the real Animation struct to the real component's dictionary.
                    animatorComponent.Animations[animName] = newAnimation;
                }

                // 4. Add the fully constructed, real component to the entity.
                world.AddComponent(entity, animatorComponent);

                Debug.WriteLine($"     ... successfully added 'AnimatorComponent'.");
            }
            else
            {
                // If it's not a SpriteComponent, we need to find the component type dynamically.
                var assemblyName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
                var fullTypeName = $"TFG.Scripts.Core.Components.{componentName}, {assemblyName}";
                // We look for the component type in the assembly of the current domain.
                var componentType = Type.GetType(fullTypeName);
                
                // If we can't find it, we log a warning and skip the component.
                if(componentType == null)
                {
                    Debug.WriteLine(
                        $"[PrefabManager] WARNING: Could not find component type '{componentName}' for prefab '{prefabName}'. " +
                        $"Check for typos in the JSON or C# class. Skipping component.");
                    continue;
                }
                
                // Otherwise, we deserialize the component data into an instance of the component type.
                var componentInstance = ComponentDeserializer.Deserialize(componentData, componentType);
                
                // And finally, add the component to the entity. With the reflection AddComponent method.
                world.AddComponent(entity, componentType, componentInstance);
            }
            Debug.WriteLine($"    ... successfully added '{componentName}'.");
        }
        
        // And finally, we set the entity's position.
        ref var transform = ref world.GetComponent<TransformComponent>(entity);
        transform.Position = position;
        
        // And return the entity.
        return entity;
    }
}