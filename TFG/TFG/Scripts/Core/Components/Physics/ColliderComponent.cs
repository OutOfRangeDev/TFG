using Microsoft.Xna.Framework;
using TFG.Scripts.Core.Abstractions;

namespace TFG.Scripts.Core.Components;

public struct ColliderComponent : IComponent
{
    // Collision box offset and size.
    public Vector2 Size;
    public Vector2 Offset;
    
    // Collision layer and trigger.
    public CollisionLayer Layer;
    public bool IsTrigger;
}