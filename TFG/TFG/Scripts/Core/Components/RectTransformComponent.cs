using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TFG.Scripts.Core.Abstractions;
using TFG.Scripts.Core.Helper;

namespace TFG.Scripts.Core.Components;

public enum UiAnchorPresets{ TopLeft, TopCenter, TopRight, MiddleLeft, MiddleCenter, MiddleRight, BottomLeft, BottomCenter, BottomRight}

public struct RectTransformComponent(
    UiAnchorPresets anchor,
    Vector2 positionOffset,
    Vector2 size,
    Vector2 pivot,
    float rotation)
    : IComponent
{
    // Which point is the reference?
    public UiAnchorPresets Anchor = anchor;
    
    // Offset from the anchor point.
    public Vector2 PositionOffset = positionOffset;
    
    // Size of the element.
    public Vector2 Size = size;

    // Where the pivot is located of the element is.
    public Vector2 Pivot = pivot;

    // Rotation of the element.
    public float Rotation = rotation;
    
    // For private use
    private Rectangle _bounds = Rectangle.Empty;
    private bool _isOutdated = true;

    public Rectangle GetBounds(Viewport viewport)
    {
        if (_isOutdated)
        {
            _bounds = LayoutHelper.CalculateBounds(this, viewport);
            _isOutdated = false;
        }
        return _bounds;
    }
    
    public void MakeOutdated()
    {
        _isOutdated = true;
    }
}