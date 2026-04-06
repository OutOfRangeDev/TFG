using TFG.Scripts.Core.Abstractions;

namespace TFG.Scripts.Core.Components;

public struct PlayerControllerComponent : IComponent
{
    public float Speed { get; set;}
    public float JumpForce { get; set;}
    
    // Dash
    public float DashSpeed { get; set;}
    public float DashDuration { get; set;}
    public float DashCooldown { get; set;}
}