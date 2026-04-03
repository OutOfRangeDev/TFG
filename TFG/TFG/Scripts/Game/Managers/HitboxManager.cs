using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TFG.Scripts.Core.Components;
using TFG.Scripts.Core.Data;
using TFG.Scripts.Game.Components.Combat;
using TFG.Scripts.Game.Data;

namespace TFG.Scripts.Game.Managers;

public class HitboxManager
{
    private readonly World _world;

    // Pool of available id's
    private readonly Queue<int> _inactiveHitboxes = new();
    // And keep track of active ones
    private readonly HashSet<int> _activeHitboxes = new();
    // Initial pool of created hitboxes
    private const int InitialPoolSize = 20;
    
    // Constructor
    public  HitboxManager(World world)
    {
        _world = world;
        InitializePool();
    }

    // Create the initial 20 pool size of hitboxes
    private void InitializePool()
    {
        for (int i = 0; i < InitialPoolSize; i++)
        {
            CreateNewHitboxEntity();
        }
    }

    // Create a new hitbox entity
    private int CreateNewHitboxEntity()
    {
        var entity = _world.CreateEntity();
        int id = entity.Id;
        
        _world.AddComponent(id, new TransformComponent { Position = new Vector2(-10000, -10000) }); 
        _world.AddComponent(id, new ColliderComponent { IsTrigger = true, Size = Vector2.Zero });
        _world.AddComponent(id, new AttackDefinitionComponent()); 
        _world.AddComponent(id, new OwnerComponent());
        _world.AddComponent(id, new HitboxStateComponent());
        
        _inactiveHitboxes.Enqueue(id);
        
        return id;
    }

    // Used by the combat system
    public int GetHitbox(int ownerId, AttackDefinition attackData, Vector2 origin, bool facingLeft)
    {
        
        // Get the hitbox id
        #region GetHitbox

        int id;
        // Get a hitbox from the list
        if (_inactiveHitboxes.Count > 0)
        {
            id = _inactiveHitboxes.Dequeue();
        }
        // But if there are no available, create a new hitbox, and save it
        else
        {
            id = CreateNewHitboxEntity();
            _inactiveHitboxes.Dequeue();
        }

        // And reset the history of previously hit entities
        ref var history = ref _world.GetComponent<HitboxStateComponent>(id);
        history.Reset();
        
        #endregion

        // Add the components and set te values
        #region SetUp

        // Get the transform
        ref var transform = ref _world.GetComponent<TransformComponent>(id);
        // Look if it's facing left
        float dir = facingLeft ? -1f : 1f;
        
        // Calculate the position based of the center of the player (assuming it's 32x32) ----------------- MAGIC NUMBER / LOOK OVER
        Vector2 playerCenter = origin + new Vector2(16, 16);
        // Then the hitbox center playerCenter + the offset
        Vector2 hitboxCenter = playerCenter + new Vector2(attackData.HitboxOffset.X * dir, attackData.HitboxOffset.Y);
        // Then the position is the hitbox center minus (up) half the size of the hitbox
        transform.Position = hitboxCenter - (attackData.HitboxSize / 2f);
        
        // Change the hitbox size to the one it should be
        ref var collider = ref _world.GetComponent<ColliderComponent>(id);
        collider.Size = attackData.HitboxSize;
        
        // And set the correct layer
        collider.Layer = _world.HasComponent<PlayerControllerComponent>(ownerId) ? CollisionLayer.HitEnemy : CollisionLayer.HitPlayer;

        // Now set the damage parameters, first get the component
        ref var attackDef = ref _world.GetComponent<AttackDefinitionComponent>(id);
        // Then set the damage
        attackDef.Damage = attackData.Damage;
        // And the knockbacks
        attackDef.TargetKnockback = new Vector2(attackData.TargetKnockback.X * dir, attackData.TargetKnockback.Y);
        attackDef.SelfKnockback = new Vector2(attackData.SelfKnockback.X * dir, attackData.SelfKnockback.Y);
        
        // Also set the owner
        ref var owner = ref _world.GetComponent<OwnerComponent>(id);
        owner.Owner = new Entity(ownerId);
        
        // Make it active and return
        _activeHitboxes.Add(id);
        
        return id;
        
        #endregion
    }

    // Used bt combat and damage systems
    public void ReturnHitbox(int id)
    {
        // If the hitbox isn't active then return
        if (!_activeHitboxes.Contains(id)) return;

        // If it is, then remove from its place
        if (_world.HasComponent<TransformComponent>(id))
        {
            ref var transform = ref _world.GetComponent<TransformComponent>(id);
            transform.Position = new Vector2(-10000, -10000);
        }
        
        // And reset the component
        if (_world.HasComponent<ColliderComponent>(id))
        {
            ref var collider = ref _world.GetComponent<ColliderComponent>(id);
            collider.Size = Vector2.Zero;
        }
        
        // And back to the inactive list
        _activeHitboxes.Remove(id);
        _inactiveHitboxes.Enqueue(id);
    }
}