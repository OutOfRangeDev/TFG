using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TFG.Scripts.Core.Managers;

public class Camera(int virtualWidth, int virtualHeight)
{
    public Vector2 Position;

    public Vector2 Offset = Vector2.Zero; // +X to the left, +Y up.
    
    public float Zoom = 1f; 
    public float Rotation = 0f;
    
    private readonly int _virtualWidth = virtualWidth;
    private readonly int _virtualHeight = virtualHeight;
    
    // ---------------------------------------------------
    // SHAKE
    // ---------------------------------------------------
    private float _shakeTimer;
    private float _shakeIntensity;
    private readonly Random _random = new Random();

    public void AddShake(float intensity, float time)
    {
        _shakeIntensity = intensity;
        _shakeTimer = time;
    }

    public void Update(float dt)
    {
        if (_shakeTimer > 0)
        {
            _shakeTimer -= dt;
            if (_shakeTimer <= 0) _shakeIntensity = 0;
        }
    }

    public Matrix GetViewMatrix()
    {
        // Make the camera look at the target plus offset.
        Vector2 targetPos = Position + Offset;

        if (_shakeTimer > 0)
        {
            targetPos.X += (float)(_random.NextDouble() * 2 - 1) * _shakeIntensity;
            targetPos.Y += (float)(_random.NextDouble() * 2 - 1) * _shakeIntensity;
        }
        
        return 
            // Move the world up and to the right so the camera is centered on the target.
            Matrix.CreateTranslation(-targetPos.X, -targetPos.Y, 0f) *
            // Rotate around the target.
            Matrix.CreateRotationZ(Rotation) *
            // Scale around the new origin.
            Matrix.CreateScale(Zoom) *
            // Now move the world so the player is at the origin plus offset.
            Matrix.CreateTranslation(_virtualWidth / 2f, _virtualHeight / 2f, 0f);
    }
}