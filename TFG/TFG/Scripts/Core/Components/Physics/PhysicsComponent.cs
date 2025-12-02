using Microsoft.Xna.Framework;
using TFG.Scripts.Core.Abstractions;

namespace TFG.Scripts.Core.Components.Physics;

public struct PhysicsComponent : IComponent
{
    public Vector2 Velocity;
    public float SkinWidth;
    public float Drag;
    public float GravityScale;
    public bool IsStatic;
    public bool IsGrounded;
}