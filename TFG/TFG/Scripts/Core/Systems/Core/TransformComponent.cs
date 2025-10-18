using Microsoft.Xna.Framework;

namespace TFG.Scripts.Core.Systems;

public struct TransformComponent : IComponent
{
    public Vector2 Position;
    public float Rotation;
    public Vector2 Scale;
}