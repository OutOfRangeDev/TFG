using Microsoft.Xna.Framework;

namespace TFG.Scripts.Core.Systems.Core;

public struct TransformComponent : IComponent
{
    public Vector2 Position {get; set;}
    public float Rotation {get; set;}
    public Vector2 Scale {get; set;}
}