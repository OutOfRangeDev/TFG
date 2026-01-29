using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TFG.Scripts.Core.Components;

namespace TFG.Scripts.Core.Helper;

public static class LayoutHelper
{
    public static Rectangle CalculateBounds(RectTransformComponent transform, Viewport viewport)
    {
        Vector2 anchorPos = GetAnchorPosition(transform.Anchor, viewport);

        Vector2 pivotPos = anchorPos + transform.PositionOffset;
        
        Vector2 topLeftPos = new Vector2(pivotPos.X - (transform.Size.X * transform.Pivot.X),
                                         pivotPos.Y - (transform.Size.Y * transform.Pivot.Y));
        
        return new Rectangle(
            (int) topLeftPos.X, 
            (int) topLeftPos.Y, 
            (int) transform.Size.X, 
            (int) transform.Size.Y);
    }
    
    private static Vector2 GetAnchorPosition(UiAnchorPresets anchor, Viewport viewport)
    {
        return anchor switch
        {
            UiAnchorPresets.TopLeft => new Vector2(0, 0),
            UiAnchorPresets.TopCenter => new Vector2(viewport.Width / 2f, 0),
            UiAnchorPresets.TopRight => new Vector2(viewport.Width, 0),
            UiAnchorPresets.MiddleLeft => new Vector2(0, viewport.Height / 2f),
            UiAnchorPresets.MiddleCenter => new Vector2(viewport.Width / 2f, viewport.Height / 2f),
            UiAnchorPresets.MiddleRight => new Vector2(viewport.Width, viewport.Height / 2f),
            UiAnchorPresets.BottomLeft => new Vector2(0, viewport.Height),
            UiAnchorPresets.BottomCenter => new Vector2(viewport.Width / 2f, viewport.Height),
            UiAnchorPresets.BottomRight => new Vector2(viewport.Width, viewport.Height),
            _ => Vector2.Zero
        };
    }
}