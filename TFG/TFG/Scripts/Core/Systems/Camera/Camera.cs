using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TFG.Scripts.Core.Systems.Camera;

public class Camera
{
    public Vector2 Position;
    public Vector2 Offset;
    public float Zoom = 1f; 
    public float Rotation = 0f;
    
    private readonly Viewport _viewport;
    
    public Camera(Viewport viewport)
    {
        _viewport = viewport;
    }
    
    public Matrix GetViewMatrix()
    {
        // Make the camera look at the target plus offset.
        Vector2 targetPos = Position + Offset;
        
        return 
            // Move the world up and to the right so the camera is centered on the target.
            Matrix.CreateTranslation(-targetPos.X, -targetPos.Y, 0f) *
            // Rotate around the target.
            Matrix.CreateRotationZ(Rotation) *
            // Scale around the new origin.
            Matrix.CreateScale(Zoom) *
            // Now move the world so the player is at the origin.
            Matrix.CreateTranslation(_viewport.Width / 2f, _viewport.Height / 2f, 0f);
    }
}