using System;
using Microsoft.Xna.Framework;
using TFG.Scripts.Core.Abstractions;
using TFG.Scripts.Core.Components;
using TFG.Scripts.Core.Data;
using TFG.Scripts.Game.Components.Combat;
using TFG.Scripts.Game.Managers;

namespace TFG.Scripts.Game.Systems.Combat;

public class DamageSystem(HitboxManager hitboxManager) : ISystem
{
    public void Update(World world, GameTime gameTime)
    {
        foreach (var collision in world.GetCollisionEvents())
        {
            ProcessCollision(world, collision.EntityA, collision.EntityB);
            ProcessCollision(world, collision.EntityB, collision.EntityA);
        }
    }

    private void ProcessCollision(World world, int hitboxId, int victimId)
    {
        // ---------------------------------------------------
        // VALIDATION AND GETS
        // ---------------------------------------------------
        
        #region Validation
        
        // The hitbox needs definition and the state components.
        if(!world.HasComponent<AttackDefinitionComponent>(hitboxId) 
           || !world.HasComponent<HitboxStateComponent>(hitboxId)) return;
        
        // And the victim cannot be invencible and must have a health
        if(!world.HasComponent<HealthComponent>(victimId) 
           || world.HasComponent<InvincibleComponent>(victimId)) return;
        
        // Now  we get the needed components.
        ref var attackDef = ref world.GetComponent<AttackDefinitionComponent>(hitboxId);
        var history = world.GetComponent<HitboxStateComponent>(hitboxId);
        ref var ownerComp = ref world.GetComponent<OwnerComponent>(hitboxId);
        ref var health = ref world.GetComponent<HealthComponent>(victimId);
        
        // Final validation before applying the hit and damage.
        // If the victim it's the emitter or already hit, ignore.
        if (ownerComp.Owner.Id == victimId) return;
        if (history.HitEntities.Contains(victimId)) return;
        
        #endregion
        
        // ---------------------------------------------------
        // HIT CONFIRMED
        // ---------------------------------------------------
        
        #region Hit
        
        //Console.WriteLine("[DamageSystem] Entity " + ownerComp.Owner.Id + " has hit Entity " + victimId);
        
        // Record the hit
        history.HitEntities.Add(victimId);
        
        // Apply the damage
        int finalDamage = (int)(attackDef.Damage * health.DamageMultiplier);
        health.CurrentHealth -= finalDamage;
        
        // Tell the attacker the combo can continue
        int attackerId = ownerComp.Owner.Id;
        if (world.HasComponent<CombatStateComponent>(attackerId))
        {
            ref var combatState = ref world.GetComponent<CombatStateComponent>(attackerId);
            combatState.HasHitEnemy = true;
        }
        
        // Death check, if the victim has less than 0, add death flag. For death system to check.
        if (health.CurrentHealth <= 0) 
        {
            if (world.HasComponent<PlayerControllerComponent>(victimId))
            {
                
            }else
            {
                world.AddComponent(victimId, new DeadComponent
                {
                    Stripped = false,
                    Timer = health.CorpseLifespan,
                });
            }
            return;
        }
        
        // Stop the time
        if(!world.HasComponent<HitStopComponent>(victimId))
            world.AddComponent(victimId, new HitStopComponent{ Timer = 0.05f });
        
        if(!world.HasComponent<HitStopComponent>(attackerId))
            world.AddComponent(attackerId, new HitStopComponent{ Timer = 0.05f });
        
        // Knockback for the victim
        if (world.HasComponent<PhysicsComponent>(victimId))
        {
            ref var victimPhysics = ref world.GetComponent<PhysicsComponent>(victimId);
            if (!victimPhysics.IsStatic)
            {
                victimPhysics.Velocity += attackDef.TargetKnockback;
            }
        }
        
        // Stunt
        // If it has "super armor" we do not apply stunt
        if (world.HasComponent<SuperArmorComponent>(victimId)) return;
        
        // To know in which direction apply the knockback.
        int hitDir = attackDef.TargetKnockback.X > 0 ? -1 : 1;

        // If it already has stunned, update the values.
        if (world.HasComponent<StunnedComponent>(victimId))
        {
            Console.WriteLine("[DamageSystem] Entity " + victimId + " has been stunned.");
            ref var stun = ref world.GetComponent<StunnedComponent>(victimId);
            stun.Timer = health.StunDurationOnHit;
            stun.HitDirectionX = hitDir;
        }
        else
        {
            world.AddComponent(victimId, new StunnedComponent
            {
                Timer = health.StunDurationOnHit,
                HitDirectionX = hitDir
            });
        }

        // In case it was attacking, remove/reset the attack status
        if (world.HasComponent<CombatStateComponent>(victimId))
        {
            ref var combatState = ref world.GetComponent<CombatStateComponent>(victimId);

            if (combatState.Phase == CombatPhase.Active && combatState.ActiveHitboxId != -1)
            {
                hitboxManager.ReturnHitbox(combatState.ActiveHitboxId);
                combatState.ActiveHitboxId = -1;
            }
            
            combatState.Phase = CombatPhase.None;
            combatState.IsAttacking = false;
        }

        // And add invincibility to the player
        if (world.HasComponent<PlayerControllerComponent>(victimId))
        {
            Console.WriteLine("[DamageSystem] Entity " + victimId + " has been given invincibility.");
            world.AddComponent(victimId, new InvincibleComponent{Timer = 1f});
        }
        

        #endregion
    }
}