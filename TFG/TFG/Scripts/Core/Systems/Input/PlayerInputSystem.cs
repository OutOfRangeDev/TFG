using Microsoft.Xna.Framework;
using TFG.Scripts.Core.Systems.Core;
using TFG.Scripts.Core.Systems.Physics;

namespace TFG.Scripts.Core.Systems.Input;

public class PlayerInputSystem(InputManager inputManager) : ISystem
{
    //In the constructor we need to pass the input manager.

    public void Update(World.World world, GameTime gameTime)
    {
        //Get all the player entities.
        var playerEntities = world.Query().
            With<PlayerControllerComponent>().
            With<TransformComponent>()
            .With<PhysicsComponent>()
            .Execute();

        foreach (var entity in playerEntities)
        {
            //Get the components. 
            var physics = world.GetComponent<PhysicsComponent>(entity);
            var playerController = world.GetComponent<PlayerControllerComponent>(entity);

            //Check if the player is moving.
            if (inputManager.IsActionHeld("MoveRight"))
            {
                physics.Velocity.X = playerController.Speed;
            }
            else if (inputManager.IsActionHeld("MoveLeft"))
            {
                physics.Velocity.X = -playerController.Speed;
            }
            else
            {
                physics.Velocity.X = 0f;
            }

            //Check if the player is jumping.
            if (inputManager.IsActionHeld("Jump") && physics.IsGrounded)
            {
                physics.Velocity.Y = -playerController.JumpForce;
                physics.IsGrounded = false;
            }
            
            world.SetComponent(entity, physics);
        }
    }
}