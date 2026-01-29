using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TFG.Scripts.Core.Abstractions;

namespace TFG.Scripts.Core.Components;

public enum TextAlignment { Left, Center, Right }

public struct UiTextComponent : IComponent
{
    public string Text;
    public SpriteFont Font;
    public Color Color;
    public TextAlignment Alignment;
}