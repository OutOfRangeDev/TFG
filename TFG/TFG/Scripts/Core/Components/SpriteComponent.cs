using System.Text.Json.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TFG.Scripts.Core.Abstractions;

namespace TFG.Scripts.Core.Components;

public struct SpriteComponent : IComponent
{
    public string TextureName { get; set; }
    [JsonIgnore] public Texture2D Texture { get; set; }
    public Rectangle SourceRectangle { get; set; }
    public Color Color { get; set; }
    public float Rotation { get; set; }
    public Vector2 Origin { get; set; }
    public Vector2 Scale { get; set; }
    public SpriteEffects Effects { get; set; }
    public float LayerDepth { get; set; }

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