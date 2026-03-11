using System.Diagnostics;
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
    
    private readonly HitboxManager _hitboxManager = hitboxManager;

    public void Update(World world, GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        double totalTime = gameTime.TotalGameTime.TotalSeconds;
        
        var entities = world.Query()
            .With<CombatStateComponent>()
            .With<InputBufferComponent>()
            .With<TransformComponent>() // Needed to spawn hitbox at correct place
            .With<SpriteComponent>()    // Needed for facing direction
            .Execute();

        for (int i = 0; i < entities.Count; i++)
        {
            int entityId = entities[i];
            
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
                    HandleActiveState(world, entityId, ref combatState, ref transform, ref sprite, dt);
                    break;
                case CombatPhase.Recovery:
                    HandleRecoveryState(ref combatState, ref inputBuffer, totalTime, dt);
                    break;
            }
        }
    }
    
    // PHASE HANDLERS

    private void HandleIdleState(ref CombatStateComponent state, ref InputBufferComponent input, double time)
    {
        // 1. Do we reset combo?
        if (time - state.LastAttackEndTime > 1f)
        {
            state.ComboIndex = 0;
        }
        
        // 2. Check input.
        if (input.HasBufferedAction(PlayerAction.Attack, time))
        {
            //Debug.WriteLine("[Combat System] Buffered attack detected.");
            
            string nextAttack = GetNextAttackName(state.ComboIndex);

            if (nextAttack != null)
            {
                StartAttack(ref state, nextAttack);
                input.Consume();
            }
            else
            {
                state.ComboIndex = 0;
                nextAttack = GetNextAttackName(0);
                if (nextAttack != null)
                {
                    StartAttack(ref state, nextAttack);
                    input.Consume();
                }
            }
        }
    }

    private void HandleStartupState(ref CombatStateComponent state, float dt)
    {
        state.StateTimer += dt;
        
        if (!GameAttacks.MoveList.TryGetValue(state.CurrentAttackName, out var attackData)) return;

        if (state.StateTimer >= attackData.StartUpTime)
        {
            state.Phase = CombatPhase.Active;
            state.StateTimer = 0;
            state.HasSpawnedHitbox = false;
        }
    }

    private void HandleActiveState(World world, int ownerId, ref CombatStateComponent state,
        ref TransformComponent ownerTrans, ref SpriteComponent sprite, float dt)
    {
        Debug.WriteLine($"[Combat System] Attack activated for entity with ID {ownerId}");
        
        state.StateTimer += dt;
        
        if (!GameAttacks.MoveList.TryGetValue(state.CurrentAttackName, out var attackData)) return;

        if (!state.HasSpawnedHitbox)
        {
            bool isFacingLeft = sprite.Effects == SpriteEffects.FlipHorizontally;
            
            int hitboxId = _hitboxManager.GetHitbox(ownerId, attackData, ownerTrans.Position, isFacingLeft);
            
            state.ActiveHitboxId = hitboxId;
            state.HasSpawnedHitbox = true;
        }

        if (state.StateTimer >= attackData.ActiveTime)
        {
            if (state.ActiveHitboxId != -1)
            {
                _hitboxManager.ReturnHitbox(state.ActiveHitboxId);
                state.ActiveHitboxId = -1;
            }
            
            state.Phase = CombatPhase.Recovery;
            state.StateTimer = 0;
        }
    }

    private void HandleRecoveryState(ref CombatStateComponent state, ref InputBufferComponent input, double time,
        float dt)
    {
        Debug.WriteLine("[Combat System] Recovery");
        
        // 1. Update the timer
        state.StateTimer += dt;
        state.LastAttackEndTime = time;

        if (!GameAttacks.MoveList.TryGetValue(state.CurrentAttackName, out var attackData))
        {
            //If no state is found, go to idle
            state.Phase = CombatPhase.None;
            state.IsAttacking=false;
            return;
        }
        
        // 2. Wait until the recovery period is finished
        if (state.StateTimer < attackData.RecoveryTime)
        {
            state.StateTimer = 0;
            return;
        }
        
        // ---------------------------------------------------
        // RECOVER FINISHED
        // ---------------------------------------------------
        
        // 3. Check the buffer (250ms)
        if (input.HasBufferedAction(PlayerAction.Attack, time))
        {
            state.ComboIndex++;
            
            string nextAttack = GetNextAttackName(state.ComboIndex);

            if (nextAttack != null)
            {
                StartAttack(ref state, nextAttack);
                input.Consume();
                state.StateTimer = 0;
                return;
            }
            else
            {
                state.ComboIndex = 0;
            }
        }
        
        // 4. If no buffer or combo finished, idle
        state.Phase = CombatPhase.None;
        state.IsAttacking = false;
        state.StateTimer = 0;

    }
    
    // ---------------------------------------------------
    // HELPERS
    // ---------------------------------------------------

    private void StartAttack(ref CombatStateComponent state, string attackName)
    {
        state.IsAttacking = true;
        state.Phase = CombatPhase.StartUp;
        state.StateTimer = 0;
        state.CurrentAttackName = attackName;
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
}