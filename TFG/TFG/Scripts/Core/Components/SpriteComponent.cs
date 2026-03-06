using System.Text.Json.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TFG.Scripts.Core.Abstractions;

namespace TFG.Scripts.Core.Components;

public struct SpriteComponent : IComponent
{
    public string TextureName { get; set; } = "NoName";
    [JsonIgnore] public Texture2D Texture { get; set; } = null;
    public Rectangle SourceRectangle { get; set; } = Rectangle.Empty;
    public Color Color { get; set; } = Color.White;
    public float Rotation { get; set; } = 0f;
    public Vector2 Origin { get; set; } = Vector2.Zero;
    public Vector2 Scale { get; set; } = Vector2.One;
    public SpriteEffects Effects { get; set; } = SpriteEffects.None;
    public float LayerDepth { get; set; } = 0.5f;

    public SpriteComponent()
    {
        TextureName = "NoName";
        Texture = null;
        SourceRectangle = Rectangle.Empty;
        Color = Color.White;
        Rotation = 0f;
        Origin = Vector2.Zero;
        Scale = Vector2.One;
        Effects = SpriteEffects.None;
        LayerDepth = 0f;
    }
}