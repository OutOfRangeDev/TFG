using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TFG.Scripts.Core.Abstractions;
using TFG.Scripts.Core.Components;
using TFG.Scripts.Core.Components.Animation;
using TFG.Scripts.Core.Components.Physics;
using TFG.Scripts.Core.Managers;

namespace TFG.Scripts.Game.Player_Input;

public class PlayerInputSystem(InputManager inputManager) : ISystem
{
    //In the constructor we need to pass the input manager.

    public void Update(Core.Data.World world, GameTime gameTime)
    {
        //Get all the player entities.
        var playerEntities = world.Query().
            With<PlayerControllerComponent>().
            With<TransformComponent>().
            With<PhysicsComponent>().
            With<SpriteComponent>().
            With<AnimatorComponent>().
            Execute();

        foreach (var entity in playerEntities)
        {
            //Get the components. 
            ref var physics = ref world.GetComponent<PhysicsComponent>(entity);
            ref var sprite = ref world.GetComponent<SpriteComponent>(entity);
            ref var animator = ref world.GetComponent<AnimatorComponent>(entity);
            var playerController = world.GetComponent<PlayerControllerComponent>(entity);

            //Check if the player is moving.
            if (inputManager.IsActionHeld("MoveRight"))
            {
                physics.Velocity.X = playerController.Speed;
                animator.CurrentAnimation = "Run";
                sprite.Effects = SpriteEffects.None;
            }
            else if (inputManager.IsActionHeld("MoveLeft"))
            {
                physics.Velocity.X = -playerController.Speed;
                animator.CurrentAnimation = "Run";
                sprite.Effects = SpriteEffects.FlipHorizontally;
            }
            else
            {
                physics.Velocity.X = 0f;
                animator.CurrentAnimation = "Idle";
            }

            //Check if the player is jumping.
            if (inputManager.IsActionHeld("Jump") && physics.IsGrounded)
            {
                physics.Velocity.Y = -playerController.JumpForce;
                physics.IsGrounded = false;
            }
            
            
        }
    }
}