using System;
using Microsoft.Xna.Framework.Graphics;
using TFG.Scripts.Core.Abstractions;

namespace TFG.Scripts.Core.Components;

public enum ButtonState { Normal, Pressed, Hovered }

public struct UiButtonComponent : IComponent
{
    public bool IsInteractable;
    public ButtonState State;

    public Action OnClick;
    
    public Texture2D NormalTexture;
    public Texture2D PressedTexture;
    public Texture2D HoveredTexture;
}