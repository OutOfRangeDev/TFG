using Microsoft.Xna.Framework;
using TFG.Scripts.Core.Abstractions;

namespace TFG.Scripts.Core.Components;

public struct TransformComponent : IComponent
{
    public Vector2 Position {get; set;}
    public float Rotation {get; set;}
    public Vector2 Scale {get; set;}
}