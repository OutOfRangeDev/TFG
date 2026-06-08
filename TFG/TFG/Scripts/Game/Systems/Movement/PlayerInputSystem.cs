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
            With<DashStateComponent>().
            Execute();
        
        foreach (var entityId in playerEntities)
        {
            // Get the components
            ref var buffer = ref world.GetComponent<InputBufferComponent>(entityId);
            ref var physics  = ref world.GetComponent<PhysicsComponent>(entityId);
            ref var sprite = ref world.GetComponent<SpriteComponent>(entityId);
            ref var animator = ref world.GetComponent<AnimatorComponent>(entityId);
            ref var controller = ref world.GetComponent<PlayerControllerComponent>(entityId);
            
            // Store directional input in buffer
            buffer.MoveDirection = inputManager.GetRawDirection();
            
            // ---------------------------------------------------
            // BUFFER
            // ---------------------------------------------------

            #region Buffer
            
            // First check if an attack has been pressed
            if (inputManager.IsActionPressed(PlayerAction.Dash))
            {
                buffer.Buffer = new BufferedCommand()
                {
                    Action = PlayerAction.Dash,
                    TimeStamp = currentTime,
                    DirectionSnapshot = inputManager.GetRawDirection()
                };
            }
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
            // DASH
            // ---------------------------------------------------
            
            #region Dash
            
            ref var dash = ref world.GetComponent<DashStateComponent>(entityId);
            
            // Cooldown
            if(dash.CooldownTimer > 0) dash.CooldownTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            // Handle Active Dash
            if (dash.IsDashing)
            {
                dash.Timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (dash.Timer >= controller.DashDuration)
                {
                    dash.IsDashing = false;
                    dash.CooldownTimer = controller.DashCooldown;
                }
                else
                {
                    physics.Velocity = dash.Direction * controller.DashSpeed;
                    continue;
                }
            }

            // Dash buffer
            if (!dash.IsDashing && dash.CooldownTimer <= 0 && buffer.HasBufferedAction(PlayerAction.Dash, currentTime))
            {
                dash.IsDashing = true;
                dash.Timer = 0;
                
                if(buffer.Buffer.DirectionSnapshot.X != 0) 
                    dash.Direction = new Vector2(System.Math.Sign(buffer.Buffer.DirectionSnapshot.X), 0);
                else
                    dash.Direction = new Vector2(sprite.Effects == SpriteEffects.FlipHorizontally ? -1 : 1, 0);
                
                buffer.Consume();
                
                physics.Velocity = new Vector2(dash.Direction.X * controller.DashSpeed, 0);
                
                // Dash abort combat
                if (world.HasComponent<CombatStateComponent>(entityId))
                {
                    ref var combatState = ref world.GetComponent<CombatStateComponent>(entityId);
                    combatState.Phase = CombatPhase.None;
                    combatState.IsAttacking = false;
                }
            
                world.AddComponent(entityId, new InvincibleComponent {Timer = controller.DashDuration});

                continue;
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
            
            // Apply velocity
            physics.Velocity = physics.Velocity with { X = buffer.MoveDirection.X * controller.Speed };
            
            // Apply jump force
            if (buffer.HasBufferedAction(PlayerAction.Jump, currentTime) && physics.IsGrounded)
            {
                physics.Velocity = physics.Velocity with { Y = -controller.JumpForce };
                physics.IsGrounded = false;
                buffer.Consume();
            }
            
            // Set facing direction
            if (buffer.MoveDirection.X != 0)
            {
                sprite.Effects = buffer.MoveDirection.X > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            }
            
            // Set animations
            
            if (!physics.IsGrounded)
            {
                // Airborne
                if (physics.Velocity.Y < 0)
                    animator.ChangeAnimation("JumpStart");
                else
                    animator.ChangeAnimation("Fall");
            }
            else
            {
                // Grounded

                // If we just hit the ground
                if (!physics.WasGrounded)
                {
                    animator.ChangeAnimation("JumpEnd");
                }
                else
                {
                    if(buffer.MoveDirection.X != 0)
                        animator.ChangeAnimation("Run");
                    else
                        animator.ChangeAnimation("Idle");
                }
            }
            
            #endregion
        }
    }
}