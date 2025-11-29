using System;
using System.Collections.Generic;
using System.Diagnostics;
using TFG.Scripts.Core.Systems.Core;
using IComponent = TFG.Scripts.Core.Systems.Core.IComponent;

namespace TFG.Scripts.Core.World;

public class World
{
    //Entities.
    ///List of entities. HashSet is for fast lookup.
    private readonly HashSet<Entity> _activeEntities = new();
    ///Next entity id.
    private int _nextEntityId;
    ///Queue of available ids. Store deleted entity ID's.
    private readonly Queue<int> _availableIDs = new();
    
    // A dictionary to store the different pools of components.
    private readonly Dictionary<Type, IComponentPool> _componentPools = new();
    
    // A list of collision events.
    private readonly List<CollisionEvent> _collisionEvents = new();
    
    //Systems.
    
    #region Entity Management
    
    //Create a new entity.
    public Entity CreateEntity()
    {
        //Create a new entity variable.
        var newEntity =
            //If there is a previously deleted entity id, use it.
            _availableIDs.Count > 0 ? new Entity(_availableIDs.Dequeue()) :
            //Otherwise create a new entity id.
            new Entity(_nextEntityId++);

        //Add the new entity to the list of active entities.
        _activeEntities.Add(newEntity);
        
        //Debug message.
        Debug.WriteLine($"[World] Created new entity with ID {newEntity.Id}.");
        
        //Add the new entity to the list of active entities.
        return newEntity;
    }

    //Destroy an entity.
    public void DestroyEntity(Entity entityToDestroy)
    {
        //First, check if the entity exists.
        if (!_activeEntities.Remove(entityToDestroy))
        {
            Debug.WriteLine($"[World] Tried to destroy non-existing entity {entityToDestroy.Id}.");
            return;
        }
        
        //Add this id to the queue of available ids.
        _availableIDs.Enqueue(entityToDestroy.Id);

        //Remove every component of this entity from the pools.
        foreach (var componentPool in _componentPools)
        {
            componentPool.Value.Remove(entityToDestroy.Id);
        }
    }

    #endregion

    #region Component Management

    private ComponentPool<T> GetOrCreateComponentPool<T>() where T : IComponent
    {
        var componentType = typeof(T);
        if (!_componentPools.TryGetValue(componentType, out var pool))
        {
            pool = new ComponentPool<T>();
            _componentPools.Add(componentType, pool);
        }
        return (ComponentPool<T>) pool;
    }

    public void AddComponent<T>(Entity entity, T component) where T : IComponent
    {
        if (!_activeEntities.Contains(entity))
        {
            throw new Exception($"[World] Tried to add component {typeof(T).Name} to non-existing entity {entity.Id}.");
        }
        GetOrCreateComponentPool<T>().Add(entity.Id, component);
    }
    
    public bool HasComponent<T>(Entity entity) where T : IComponent
    {
        //Check if the component pool exists.
        var componentType = typeof(T);
        if (_componentPools.TryGetValue(componentType, out var store))
        {
            // After checking if the pool exists, we return the result of the Contains method if it exists.
            return ((ComponentPool<T>) store).Contains(entity.Id);
        }
        // If the pool doesn't exist, return false.
        return false;
    }

    public bool TryGetComponent<T>(Entity entity, out T component) where T : IComponent
    {
        if (HasComponent<T>(entity))
        {
            // If the entity has the component, we return it.
            component = GetComponent<T>(entity);
            return true;
        }
        
        // If the entity doesn't have the component, we return false.
        component = default;
        return false;
    }

    public ref T GetComponent<T>(Entity entity) where T : IComponent
    {
#if DEBUG
        // In debug mode, we make a safety check to make sure the entity has the component.
        if (!HasComponent<T>(entity))
        {
            Debug.WriteLine($"[World] Tried to get component {typeof(T).Name} from entity {entity.Id} but it doesn't exist.");
        }
#endif
        // Just return the component from the pool.
        return ref GetOrCreateComponentPool<T>().Get(entity.Id);
    }

    public void RemoveComponent<T>(Entity entity) where T : IComponent
    {
        var componentType = typeof(T);
        if (_componentPools.TryGetValue(componentType, out var pool))
        {
            ((ComponentPool<T>) pool).Remove(entity.Id);
        }
    }

    #endregion

    #region Query

    public QueryBuilder Query()
    {
        // When a system calls Query(), we return a new QueryBuilder.
        return new QueryBuilder(this);
    }

    public IEnumerable<int> GetEntityIdsForComponent(Type componentType)
    {
        if (_componentPools.TryGetValue(componentType, out var pool))
        {
            return pool.GetEntityIds();
        }

        return null;
    }

    #endregion
    
    //Events.

    #region Collision Events
    
    // A constructor for collision events.
    public struct CollisionEvent(Entity entityA, Entity entityB)
    {
        public Entity EntityA = entityA;
        public Entity EntityB = entityB;
    }
    
    // Gets called by the CollisionSystem. To launch an event.
    public void AddCollisionEvent(CollisionEvent ev)
    {
        _collisionEvents.Add(ev);
    }

    // This is called by other systems to get the collision events of the frame.
    public IEnumerable<CollisionEvent> GetCollisionEvents()
    {
        return _collisionEvents;
    }

    // After the frame we get the collision events, we clear the list.
    public void ClearCollisionEvents()
    {
        _collisionEvents.Clear();
    }

    #endregion
}