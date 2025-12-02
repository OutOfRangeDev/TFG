using TFG.Scripts.Core.Abstractions;

namespace TFG.Scripts.Core.Components;

public struct PlayerControllerComponent(float speed, float jumpForce) : IComponent
{
    public float Speed = speed;
    public float JumpForce = jumpForce;
}