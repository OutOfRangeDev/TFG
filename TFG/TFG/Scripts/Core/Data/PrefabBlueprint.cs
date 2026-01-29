using System.Collections.Generic;
using System.Text.Json;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TFG.Scripts.Core.Data;

public class PrefabBlueprint
{
    // The id of the prefab.
    public string Name { get; set; }

    // Using objects to allow for any component type.
    public Dictionary<string, object> Components { get; set; } = new();
}

// A personalized blueprint for the sprite component because we can't save Textures.
public class SpriteComponentBlueprint
{
    public string TextureName { get; set;}
    public Rectangle SourceRectangle{ get; set;}
    public Color Color{ get; set;}
    public float Rotation{ get; set;}
    public Vector2 Origin{ get; set;}
    public Vector2 Scale{ get; set;}
    public SpriteEffects Effects{ get; set;}
    public float LayerDepth{ get; set;}
}

// This represents the "Animations" object in the JSON
public class AnimationBlueprint
{
    public string Name { get; set; }
    public int RowIndex { get; set; }
    public int FrameCount { get; set; }
    public float FrameDuration { get; set; }
    public bool Loop { get; set; } // Matches your "Loop" property in the JSON
}

// This represents the "AnimatorComponent" object in the JSON
public class AnimatorComponentBlueprint
{
    // The dictionary's value is the blueprint, not the final Animation struct.
    public Dictionary<string, AnimationBlueprint> Animations { get; set; }
    public string CurrentAnimationName { get; set; }
}