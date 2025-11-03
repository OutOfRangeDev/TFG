using Microsoft.Xna.Framework;
using TFG.Scripts.Core.Systems.Core;

namespace TFG.Scripts.Core.Systems.Physics;

public struct PhysicsComponent : IComponent
{
    public Vector2 Velocity;
    public float Drag;
    public float GravityScale;
    public bool IsStatic;
    public bool IsGrounded;
}