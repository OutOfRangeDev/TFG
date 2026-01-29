using Microsoft.Xna.Framework;
using TFG.Scripts.Core.Abstractions;

namespace TFG.Scripts.Core.Components;

public struct ColliderComponent : IComponent
{
    // Collision box offset and size.
    public Vector2 Size {get; set;}
    public Vector2 Offset {get; set;}
    
    // Collision layer and trigger.
    public CollisionLayer Layer {get; set;}
    public bool IsTrigger {get; set;}

    public ColliderComponent()
    {
        Size = Vector2.Zero;
        Offset = Vector2.Zero;
        Layer = CollisionLayer.None;
        IsTrigger = false;
    }
}