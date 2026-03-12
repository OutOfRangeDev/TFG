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
    public void Update(Core.Data.World world, GameTime gameTime)
    {
        // Global time
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
        
        foreach (var entityId in playerEntities)
        {
            // Get the components
            ref var buffer = ref world.GetComponent<InputBufferComponent>(entityId);
            ref var physics  = ref world.GetComponent<PhysicsComponent>(entityId);
            ref var sprite = ref world.GetComponent<SpriteComponent>(entityId);
            ref var animator = ref world.GetComponent<AnimatorComponent>(entityId);
            var controller =  world.GetComponent<PlayerControllerComponent>(entityId);
            
            // Store directional input in buffer
            buffer.MoveDirection = inputManager.GetRawDirection();
            
            // ---------------------------------------------------
            // BUFFER
            // ---------------------------------------------------

            #region Buffer
            
            // First check if an attack has been pressed
            if (inputManager.IsActionPressed(PlayerAction.Attack))
            {
                //Debug.WriteLine("[INPUT SYSTEM] Buffered attack.");
                buffer.Buffer = new BufferedCommand()
                {
                    Action = PlayerAction.Attack,
                    TimeStamp = currentTime,
                    DirectionSnapshot = inputManager.GetRawDirection()
                };
            } 
            // And check the buffer for the jump
            else if (inputManager.IsActionPressed(PlayerAction.Jump))
            {
                buffer.Buffer = new BufferedCommand()
                {
                    Action = PlayerAction.Jump,
                    TimeStamp = currentTime,
                    DirectionSnapshot = inputManager.GetRawDirection()
                };
            }

            #endregion
            
            // ---------------------------------------------------
            // BUSY
            // ---------------------------------------------------
            
            #region Busy
            
            // If the player is attacking, block the input.
            
            bool isBusy = false;

            if (world.HasComponent<CombatStateComponent>(entityId))
            {
                var combatState = world.GetComponent<CombatStateComponent>(entityId);
                // For now block movement while attacking
                if(combatState.Phase != CombatPhase.None) isBusy = true;
            }

            if (isBusy) continue;
            
            #endregion
            
            // ---------------------------------------------------
            // MOVEMENT
            // ---------------------------------------------------
            
            #region Movement
            
            // Apply the force to the movement
            physics.Velocity = physics.Velocity with { X = buffer.MoveDirection.X * controller.Speed };

            // Now check the animations
            // If the speed in the buffer is not 0
            if (buffer.MoveDirection.X != 0)
            {
                // Play run
                animator.CurrentAnimation = "Run";
                sprite.Effects = buffer.MoveDirection.X > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            }
            else
            {
                // If not, idle
                physics.Velocity = physics.Velocity with { X = 0 };
                animator.CurrentAnimation = "Idle";
            }

            // And if the jump is buffered, apply it.
            if (buffer.HasBufferedAction(PlayerAction.Jump, currentTime) && physics.IsGrounded)
            {
                physics.Velocity = physics.Velocity with { Y = -controller.JumpForce };
                physics.IsGrounded = false;
                    
                buffer.Consume();
            }
            
            #endregion
        }
    }
}