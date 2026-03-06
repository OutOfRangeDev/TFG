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

    private const int InitialPoolSize = 20;
    
    public  HitboxManager(World world)
    {
        _world = world;
        InitializePool();
    }

    private void InitializePool()
    {
        for (int i = 0; i < InitialPoolSize; i++)
        {
            CreateNewHitboxEntity();
        }
    }

    private int CreateNewHitboxEntity()
    {
        var entity = _world.CreateEntity();
        int id = entity.Id;
        
        _world.AddComponent(id, new TransformComponent { Position = new Vector2(-10000, -10000) }); 
        _world.AddComponent(id, new ColliderComponent { IsTrigger = true, Size = Vector2.Zero });
        _world.AddComponent(id, new AttackDefinitionComponent()); 
        _world.AddComponent(id, new OwnerComponent());
        
        _inactiveHitboxes.Enqueue(id);
        
        return id;
    }

    public int GetHitbox(int ownerId, AttackDefinition attackData, Vector2 origin, bool facingLeft)
    {
        int id;

        if (_inactiveHitboxes.Count > 0)
        {
            id = _inactiveHitboxes.Dequeue();
        }
        else
        {
            id = CreateNewHitboxEntity();
            _inactiveHitboxes.Dequeue();
        }
        
        ref var transform = ref _world.GetComponent<TransformComponent>(id);
        float dir = facingLeft ? -1f : 1f;
        Vector2 offset = new Vector2(attackData.HitboxOffset.X * dir, attackData.HitboxOffset.Y);
        transform.Position = origin + offset;
        
        ref var collider = ref _world.GetComponent<ColliderComponent>(id);
        collider.Size = attackData.HitboxSize;
        
        if (_world.HasComponent<PlayerControllerComponent>(ownerId))
        {
            collider.Layer = CollisionLayer.HitEnemy;
        }
        else
        {
            collider.Layer = CollisionLayer.HitPlayer;
        }
        
        ref var attackDef = ref _world.GetComponent<AttackDefinitionComponent>(id);
        attackDef.Damage = attackData.Damage;
        attackDef.TargetKnockback = new Vector2(attackData.TargetKnockback.X * dir, attackData.TargetKnockback.Y);
        attackDef.SelfKnockback = new Vector2(attackData.SelfKnockback.X * dir, attackData.SelfKnockback.Y);
        
        ref var owner = ref _world.GetComponent<OwnerComponent>(id);
        owner.Owner = new Entity(ownerId);
        
        _activeHitboxes.Add(id);
        
        return id;
    }

    public void ReturnHitbox(int id)
    {
        if (!_activeHitboxes.Contains(id)) return;

        if (_world.HasComponent<TransformComponent>(id))
        {
            ref var transform = ref _world.GetComponent<TransformComponent>(id);
            transform.Position = new Vector2(-10000, -10000);
        }

        if (_world.HasComponent<ColliderComponent>(id))
        {
            ref var collider = ref _world.GetComponent<ColliderComponent>(id);
            collider.Size = Vector2.Zero;
        }
        
        _activeHitboxes.Remove(id);
        _inactiveHitboxes.Enqueue(id);
    }
}