using Microsoft.Xna.Framework;
using TFG.Scripts.Game.Data;

namespace TFG.Scripts.Core.Abstractions;

public struct BufferedCommand
{
    public PlayerAction Action;
    public double TimeStamp;
    public Vector2 DirectionSnapshot;
}

public struct InputBufferComponent : IComponent
{
    // Continuous input
    public Vector2 MoveDirection;
    
    // The action
    public BufferedCommand Buffer;
    
    // Constants
    public const double BufferWindow = 0.25; //250 ms
    
    public bool HasBufferedAction(PlayerAction action,  double currentTime)
    {
        return Buffer.Action == action && (currentTime - Buffer.TimeStamp) <= BufferWindow;
    }

    public void Consume()
    {
        Buffer.Action = PlayerAction.None;
        Buffer.TimeStamp = 0;
    }
}