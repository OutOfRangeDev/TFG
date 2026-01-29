using Microsoft.Xna.Framework;
using TFG.Scripts.Core.Abstractions;

namespace TFG.Scripts.Core.Components;

public struct PhysicsComponent : IComponent
{
    public Vector2 Velocity {get; set;}
    public float SkinWidth {get; set;}
    public float Drag {get; set;}
    public float GravityScale {get; set;}
    public bool IsStatic {get; set;}
    public bool IsGrounded {get; set;}

    public PhysicsComponent()
    {
        Velocity = Vector2.Zero;
        SkinWidth = 0f;
        Drag = 0f;
        GravityScale = 0f;
        IsStatic = false;
        IsGrounded = false;
    }
}