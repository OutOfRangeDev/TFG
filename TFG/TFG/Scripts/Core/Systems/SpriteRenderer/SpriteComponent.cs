using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TFG.Scripts.Core.Systems.Core;

namespace TFG.Scripts.Core.Systems.SpriteRenderer;

public struct SpriteComponent : IComponent
{
    public Texture2D Texture;
    public Rectangle SourceRectangle;
    public Color Color;
    public float Rotation;
    public Vector2 Origin;
    public Vector2 Scale;
    public SpriteEffects Effects;
    public float LayerDepth;
}