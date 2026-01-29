using TFG.Scripts.Core.Abstractions;

namespace TFG.Scripts.Core.Components;

public struct PlayerControllerComponent : IComponent
{
    public float Speed { get; set;}
    public float JumpForce { get; set;}
}