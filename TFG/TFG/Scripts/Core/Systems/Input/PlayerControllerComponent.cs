using TFG.Scripts.Core.Systems.Core;

namespace TFG.Scripts.Core.Systems.Input;

public struct PlayerControllerComponent(float speed, float jumpForce) : IComponent
{
    public float Speed = speed;
    public float JumpForce = jumpForce;
}