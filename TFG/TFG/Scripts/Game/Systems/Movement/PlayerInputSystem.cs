using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TFG.Scripts.Core.Abstractions;
using TFG.Scripts.Core.Components;
using TFG.Scripts.Core.Managers;
using TFG.Scripts.Game.Components.Combat;
using TFG.Scripts.Game.Data;

namespace TFG.Scripts.Game.Systems.Movement;

public class PlayerInputSystem(InputManager inputManager) : ISystem
{
    //In the constructor we need to pass the input manager.

    public void Update(Core.Data.World world, GameTime gameTime)
    {
        double currentTime = gameTime.TotalGameTime.TotalMilliseconds;
        
        //Get all the player entities.
        var playerEntities = world.Query().
            With<PlayerControllerComponent>().
            With<InputBufferComponent>().
            With<TransformComponent>().
            With<PhysicsComponent>().
            With<SpriteComponent>().
            With<AnimatorComponent>().
            Execute();

        for (int i = 0; i < playerEntities.Count; i++)
        {
            int entityId = playerEntities[i];
            
            // Get the components
            ref var buffer = ref world.GetComponent<InputBufferComponent>(entityId);
            ref var physics  = ref world.GetComponent<PhysicsComponent>(entityId);
            ref var sprite = ref world.GetComponent<SpriteComponent>(entityId);
            ref var animator = ref world.GetComponent<AnimatorComponent>(entityId);
            var controller =  world.GetComponent<PlayerControllerComponent>(entityId);
            
            // Store directional input in buffer
            buffer.MoveDirection = inputManager.GetRawDirection();
            
            // Action buffer
            if (inputManager.IsActionPressed(PlayerAction.Attack))
            {
                buffer.Buffer = new BufferedCommand()
                {
                    Action = PlayerAction.Attack,
                    TimeStamp = currentTime,
                    DirectionSnapshot = inputManager.GetRawDirection()
                };
            }
            else if (inputManager.IsActionPressed(PlayerAction.Jump))
            {
                buffer.Buffer = new BufferedCommand()
                {
                    Action = PlayerAction.Jump,
                    TimeStamp = currentTime,
                    DirectionSnapshot = inputManager.GetRawDirection()
                };
            }

            bool isBusy = false;

            if (world.HasComponent<CombatStateComponent>(entityId))
            {
                var combatState = world.GetComponent<CombatStateComponent>(entityId);
                // For now block movement while attacking
                if(combatState.Phase != CombatPhase.None) isBusy = true;
            }

            if (!isBusy)
            {
                physics.Velocity = physics.Velocity with { X = buffer.MoveDirection.X * controller.Speed };

                if (buffer.MoveDirection.X != 0)
                {
                    animator.CurrentAnimation = "Run";
                    sprite.Effects = buffer.MoveDirection.X > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                }
                else
                {
                    physics.Velocity = physics.Velocity with { X = 0};
                    animator.CurrentAnimation = "Idle";
                }
            }
        }
    }
}