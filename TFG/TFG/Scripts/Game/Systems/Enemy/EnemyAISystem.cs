using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TFG.Scripts.Core.Abstractions;
using TFG.Scripts.Core.Components;
using TFG.Scripts.Core.Data;
using TFG.Scripts.Core.Helper;
using TFG.Scripts.Game.Components.Combat;
using TFG.Scripts.Game.Components.Enemies;

namespace TFG.Scripts.Game.Systems.Enemy;

public class EnemyAiSystem : ISystem
{
    public void Update(World world, GameTime gameTime)
    {
        // Find the player

        var player = world.Query().
            With<PlayerControllerComponent>().
            With<TransformComponent>().
            Execute();
        
        int playerId;
        Vector2 playerPosition = default;
        
        if (player.Count > 0)
        {
            playerId = player[0];
            playerPosition = world.GetComponent<TransformComponent>(playerId).Position;
        }
        else
        {
            playerId = -1;
            Console.WriteLine("[EnemyAISystem] Could not find player.");
        }

        
        // Environment for ground check
        var environmentEntities = world.Query().
            With<ColliderComponent>().
            With<TransformComponent>().
            Execute();

        List<int> validEnvironment = new List<int>();
        for (int i = 0; i < environmentEntities.Count; i++)
        {
            int environmentId = environmentEntities[i];
            if (world.GetComponent<ColliderComponent>(environmentId).Layer == CollisionLayer.Environment)
            {
                validEnvironment.Add(environmentId);
            }
        }

        var enemies = world.Query().
            With<EnemyAiComponent>().
            With<PhysicsComponent>().
            With<TransformComponent>().
            With<ColliderComponent>().
            With<SpriteComponent>().
            With<CombatStateComponent>().
            Execute();

        for (int i = 0; i < enemies.Count; i++)
        {
            int enemyId = enemies[i];

            // If enemy is stunned leave
            if (world.HasComponent<StunnedComponent>(enemyId)) continue;

            // Or if enemy is already attacking
            ref var combatState = ref world.GetComponent<CombatStateComponent>(enemyId);
            if (combatState.Phase != CombatPhase.None || combatState.IsAttacking) continue;

            ref var ai = ref world.GetComponent<EnemyAiComponent>(enemyId);
            ref var physics = ref world.GetComponent<PhysicsComponent>(enemyId);
            ref var transform = ref world.GetComponent<TransformComponent>(enemyId);
            ref var sprite = ref world.GetComponent<SpriteComponent>(enemyId);
            
            // Vision
            if (playerId != -1)
            {
                float distanceToPlayer = Vector2.Distance(transform.Position, playerPosition);

                if (distanceToPlayer <= ai.AttackRange)
                {
                    ai.CurrentState = AiState.Attack;
                }
                else if (distanceToPlayer <= ai.DetectionRadius)
                {
                    ai.CurrentState = AiState.Chase;
                }
                else
                {
                    ai.CurrentState = AiState.Patrol;
                }
            }
            else
            {
                ai.CurrentState = AiState.Patrol;
            }
            
            // Execution
            switch (ai.CurrentState)
            {
                case AiState.Attack:
                    // Stop Walking
                    physics.Velocity = physics.Velocity with { X = 0 };
                    
                    // Face the player
                    float dirToPlayer = Math.Sign(playerPosition.X - transform.Position.X);
                    sprite.Effects = dirToPlayer < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

                    combatState.CurrentAttackName = ai.DefaultAttackName;
                    combatState.CurrentAttackName = ai.DefaultAttackName;
                    combatState.Phase = CombatPhase.StartUp;
                    combatState.IsAttacking = true;
                    combatState.StateTimer = 0;
                    combatState.HasSpawnedHitbox = false;
                    combatState.HasHitEnemy = false;
                    
                    break;
                
                case AiState.Chase:
                    // Move torwards player
                    float chaseDir = Math.Sign(playerPosition.X - transform.Position.X);
                    sprite.Effects = chaseDir < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

                    if (IsPathSafe(world, enemyId, chaseDir, validEnvironment))
                    {
                        physics.Velocity = physics.Velocity with { X = chaseDir * ai.ChaseSpeed };
                    }
                    else
                    {
                        // Reached cliff
                        physics.Velocity = physics.Velocity with { X = 0 };
                    }
                    break;
                
                case AiState.Patrol:
                    if (!IsPathSafe(world, enemyId, ai.PatrolDirection, validEnvironment))
                    {
                        // Flip direction if the path is not safe
                        ai.PatrolDirection *= -1; 
                    }

                    physics.Velocity = physics.Velocity with { X = ai.PatrolDirection * ai.PatrolSpeed };
                    sprite.Effects = ai.PatrolDirection < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                    
                    break;
            }
        }
    }

    private bool IsPathSafe(World world, int enemyId, float direction, List<int> validEnvironment)
    {
        Rectangle bounds = CollisionHelper.GetWorldBounds(enemyId, world);
        int dir = Math.Sign(direction);

        Rectangle wallSensor = new Rectangle(dir == 1 ? bounds.Right + 1 : bounds.Left - 6,
            bounds.Center.Y, 5, 5);
        
        Rectangle ledgeSensor = new Rectangle(
            dir == 1 ? bounds.Right + 1 : bounds.Left - 6, 
            bounds.Bottom + 2, 5, 5);

        bool hitWall = false;
        bool hitFloor = false;

        for (int e = 0; e > validEnvironment.Count; e++)
        {
            Rectangle envBounds = CollisionHelper.GetWorldBounds(validEnvironment[e], world);
            
            if (CollisionHelper.AreColliding(wallSensor, envBounds)) hitWall = true;
            if (CollisionHelper.AreColliding(ledgeSensor, envBounds)) hitFloor = true;
        }
        
        return !hitWall && hitFloor;
    }
}