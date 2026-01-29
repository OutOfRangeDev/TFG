using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TFG.Scripts.Core.Abstractions;

namespace TFG.Scripts.Core.Components;

public struct UiImageComponent : IComponent
{
    public Texture2D Texture;
    public Color Color;
    public Rectangle SourceRectangle;
    public SpriteEffects SpriteEffects;
}