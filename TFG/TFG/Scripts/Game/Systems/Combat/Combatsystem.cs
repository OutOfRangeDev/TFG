using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TFG.Scripts.Core.Abstractions;
using TFG.Scripts.Core.Components;
using TFG.Scripts.Core.Data;
using TFG.Scripts.Game.Components.Combat;
using TFG.Scripts.Game.Data;
using TFG.Scripts.Game.Managers;

namespace TFG.Scripts.Game.Systems.Combat;

public class CombatSystem(HitboxManager hitboxManager) : ISystem
{
    // ---------------------------------------------------
    // UPDATE
    // ---------------------------------------------------
    
    #region Update
    
    public void Update(World world, GameTime gameTime)
    {
        // Deltatime and the total time passed.
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        double totalTime = gameTime.TotalGameTime.TotalSeconds;
        
        var entities = world.Query()
            .With<CombatStateComponent>()
            .With<InputBufferComponent>()
            .With<TransformComponent>() // Needed to spawn hitbox at correct place
            .With<SpriteComponent>()    // Needed for facing direction
            .Execute();

        foreach (var entityId in entities)
        {
            ref var combatState = ref world.GetComponent<CombatStateComponent>(entityId);
            ref var inputBuffer = ref world.GetComponent<InputBufferComponent>(entityId);
            ref var transform = ref world.GetComponent<TransformComponent>(entityId);
            ref var sprite = ref world.GetComponent<SpriteComponent>(entityId);
            
            // STATE MACHINE
            switch (combatState.Phase)
            {
                case CombatPhase.None:
                    HandleIdleState(ref combatState, ref inputBuffer, totalTime);
                    break;
                case CombatPhase.StartUp:
                    HandleStartupState(ref combatState, dt);
                    break;
                case CombatPhase.Active:
                    HandleActiveState(entityId, ref combatState, ref transform, ref sprite, dt);
                    break;
                case CombatPhase.Recovery:
                    HandleRecoveryState(ref combatState, ref inputBuffer, totalTime, dt);
                    break;
            }
        }
    }
    
    #endregion

    // ---------------------------------------------------
    // PHASES
    // ---------------------------------------------------
    
    #region Phases

    private void HandleIdleState(ref CombatStateComponent state, ref InputBufferComponent input, double time)
    {
        // 1. Do we reset combo?
        if (time - state.LastAttackEndTime > 1f)
        {
            state.ComboIndex = 0;
        }
        
        // 2. Do we have an action in the buffer?
        if (input.HasBufferedAction(PlayerAction.Attack, time))
        {
            //Debug.WriteLine("[Combat System] Buffered attack detected.");
            
            // If we do, we look for the next attack.
            string nextAttack = GetNextAttackName(state.ComboIndex);

            // And if you have one, we start the attack
            if (nextAttack != null)
            {
                // And we reference the state component from the hitbox, and the name of the attack.
                StartAttack(ref state, nextAttack);
                // And also delete the buffer.
                input.Consume();
            }
            else
            {
                // If we don't have another attack, this means the combo has been reset.
                state.ComboIndex = 0;
                // And reset the array combo.
                nextAttack = GetNextAttackName(0);
                // And again try to do the attack
                if (nextAttack != null)
                {
                    // Start attack sequence.
                    StartAttack(ref state, nextAttack);
                    // Delete the buffer.
                    input.Consume();
                }
            }
        }
    }

    private void HandleStartupState(ref CombatStateComponent state, float dt)
    {
        // Add time to the state timer
        state.StateTimer += dt;
        
        // If there is no attack that matches, leave.
        if (!GameAttacks.MoveList.TryGetValue(state.CurrentAttackName, out var attackData)) return;

        // ---------------------------------------------------
        // START-UP FINISHED
        // ---------------------------------------------------
        
        // If the attack exists, then check when the state timer meets the start-up of the attack.
        if (state.StateTimer >= attackData.StartUpTime)
        {
            // Change the phase to the one with the hitbox active
            state.Phase = CombatPhase.Active;
            // Reset state timer
            state.StateTimer = 0;
            // Make sure one last time no hitbox was assigned
            state.HasSpawnedHitbox = false;
        }
    }

    private void HandleActiveState(int ownerId, ref CombatStateComponent state,
        ref TransformComponent ownerTrans, ref SpriteComponent sprite, float dt)
    {
        //Debug.WriteLine($"[Combat System] Attack activated for entity with ID {ownerId}");
        
        // Again increment the state timer
        state.StateTimer += dt;
        
        // If the attack doesn't exist, leave
        if (!GameAttacks.MoveList.TryGetValue(state.CurrentAttackName, out var attackData)) return;

        // If we haven't already spawned a hitbox
        if (!state.HasSpawnedHitbox)
        {
            // Make sure we are facing the right direction
            bool isFacingLeft = sprite.Effects == SpriteEffects.FlipHorizontally;
            
            // Create the hitbox
            int hitboxId = hitboxManager.GetHitbox(ownerId, attackData, ownerTrans.Position, isFacingLeft);
            
            // And save that hitbox id
            state.ActiveHitboxId = hitboxId;
            // And also make sure we mark the hitbox is created
            state.HasSpawnedHitbox = true;
        }
        
        // ---------------------------------------------------
        // ATTACK FINISHED
        // ---------------------------------------------------

        // And when the times of the active time finishes
        if (state.StateTimer >= attackData.ActiveTime)
        {
            // If we still have a hitbox
            if (state.ActiveHitboxId != -1)
            {
                // Return it to the pool
                hitboxManager.ReturnHitbox(state.ActiveHitboxId);
                // And make sure no hitbox is assigned
                state.ActiveHitboxId = -1;
            }
            
            // And change to the next phase
            state.Phase = CombatPhase.Recovery;
            // Also reset the timer
            state.StateTimer = 0;
        }
    }

    private void HandleRecoveryState(ref CombatStateComponent state, ref InputBufferComponent input, double time,
        float dt)
    {
        //Debug.WriteLine("[Combat System] Recovery");
        
        // Update the timers
        state.StateTimer += dt;
        state.LastAttackEndTime = time;

        // If no attack is valid, go back idle
        if (!GameAttacks.MoveList.TryGetValue(state.CurrentAttackName, out var attackData))
        {
            state.Phase = CombatPhase.None;
            state.IsAttacking=false;
            return;
        }
        
        // Wait until the recovery period is finished
        if (state.StateTimer < attackData.RecoveryTime)
        {
            return;
        }
        
        // ---------------------------------------------------
        // RECOVER FINISHED
        // ---------------------------------------------------
        
        // Check the buffer (250ms) if we have an action buffered
        if (input.HasBufferedAction(PlayerAction.Attack, time))
        {
            // Move "up" on the combo
            state.ComboIndex++;
            
            // Get the next attack
            string nextAttack = GetNextAttackName(state.ComboIndex);

            // If there is a next attack
            if (nextAttack != null)
            {
                // Attack sequence again
                StartAttack(ref state, nextAttack);
                // And also clean the buffer
                input.Consume();
                return;
            }
            else
            {
                // If it's null, restart combo
                state.ComboIndex = 0;
            }
        }
        
        // And if there is no buffer, just go back to idle.
        state.Phase = CombatPhase.None;
        state.IsAttacking = false;
        state.StateTimer = 0;
    }
    
    #endregion

    // ---------------------------------------------------
    // HELPERS
    // ---------------------------------------------------
    
    #region Helpers

    private void StartAttack(ref CombatStateComponent state, string attackName)
    {
        // Change the entity state to attacking
        state.IsAttacking = true;
        // Start the attack sequence
        state.Phase = CombatPhase.StartUp;
        // Make sure the startup timer is 0
        state.StateTimer = 0;
        // Make sure of the attack name
        state.CurrentAttackName = attackName;
        // And make sure no hitbox is "assigned"
        state.HasSpawnedHitbox = false;
        
        // Debug.WriteLine($"[COMBAT SYSTEM] Started Attack: {attackName}");
    }

    private string GetNextAttackName(int comboIndex)
    {
        return comboIndex switch
        {
            0 => "Ground_Light_1",
            1 => "Ground_Light_2",
            2 => "Ground_Heavy_3",
            _ => null 
        };
    }
    
    #endregion
}