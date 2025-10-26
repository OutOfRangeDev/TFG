using System;
using System.Collections.Generic;
using System.Diagnostics;
using TFG.Scripts.Core.Systems;
using IComponent = TFG.Scripts.Core.Systems.Core.IComponent;

namespace TFG.Scripts.Core.World;

public class World
{
    //Entities.
    ///List of entities. HashSet is for fast lookup.
    private readonly HashSet<Entity> _activeEntities = new();
    ///Next entity id.
    private int _nextEntityId = 0;
    ///Queue of available ids. Store deleted entity ID's.
    private readonly Queue<int> _availableIDs = new();
    
    // Generic dictionary for storing components.
    // First we store the component type, then we also store inside the entity ID and the interface to the component.
    private readonly Dictionary<Type, Dictionary<int, IComponent>> _componentStores = new();
    
    // A list of collision events.
    private readonly List<CollisionEvent> _collisionEvents = new();
    
    //Systems.
    
    #region Entity Management
    
    //Create a new entity.
    public Entity CreateEntity()
    {
        //Create a new entity variable.
        Entity newEntity;
        //If there is a previously deleted entity id, use it.
        if(_availableIDs.Count > 0)
            newEntity = new Entity(_availableIDs.Dequeue());
        //Otherwise create a new entity id.
        else
            newEntity = new Entity(_nextEntityId++);
        
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
        //Check if the entity exists.
        if (_activeEntities.Contains(entityToDestroy))
        {
            //Remove the entity from the list of active entities. And make the ID available again.
            _activeEntities.Remove(entityToDestroy);
            _availableIDs.Enqueue(entityToDestroy.Id);
        }
        else
        {
            Debug.WriteLine($"[World] Tried to delete non-existing entity with ID {entityToDestroy.Id}.");
        }
    }

    #endregion

    #region Component Management

    //Get a component from an entity.
    // Returns the component of type Component associated with the given entity.
    public TComponent GetComponent<TComponent>(Entity entity) where TComponent : IComponent
    {
        //First we indicate the type of component we want to get.
        var componentType = typeof(TComponent);
        
        // Then we check if we have a store for this Type.
        if(_componentStores.TryGetValue(componentType, out var store))
            // If we do, we check if we have a component for this entity.
            if(store.TryGetValue(entity.Id, out var component))
                // If we do, we return the component.
                return (TComponent) component;
        
        //If in any step we fail, we throw an exception.
        throw new Exception($"[World] Tried to get unrecognized component type {typeof(TComponent)} for entity with ID {entity.Id}.");
    }
    
    public void AddComponent<TComponent>(Entity entity, TComponent component) where TComponent : IComponent
    {
        if (!_activeEntities.Contains(entity))
        {
            // If the entity doesn't exist, throw an exception.
            throw new Exception($"[World] Tried to add component to non-existing entity with ID {entity.Id}.");
        }
        
        // Indicate the type of component we want to add.
        var componentType = typeof(TComponent);

        // Check if we have a store for this Type yet.
        if (!_componentStores.ContainsKey(componentType))
        {
            // If not, create one.
            _componentStores[componentType] = new Dictionary<int, IComponent>();
        }
        
        // Check if we already have a component for this entity.
        var store = _componentStores[componentType];

        if (store.ContainsKey(entity.Id))
        {
            // If we do, throw an exception.
            throw new Exception($"[World] Entity with ID {entity.Id} already has a component of type {componentType.Name}.");
        }
        else
        {
            // Add the component to the correct store, using the entity's ID as the key.
            _componentStores[componentType][entity.Id] = component;
        }
    }
    
    //Replace a component to an entity during execution.
    public void SetComponent<TComponent>(Entity entity, TComponent component) where TComponent : IComponent
    {
        // If the entity doesn't exist, throw an exception.
        if (!_activeEntities.Contains(entity))
        {
            throw new Exception($"[World] Tried to set component to non-existing entity with ID {entity.Id}.");
        }
        
        // Indicate the type of component we want to set.
        var componentType = typeof(TComponent);

        // Check if we have a store for this Type yet. If not, throw an exception.
        if (!_componentStores.TryGetValue(componentType, out var store))
        {
            throw new Exception($"[World] Attempted to set a component of type {componentType.Name} but no components of this type have been added yet.");
        }
        
        // If the entity exists and have the component, replace it. Or create it if it doesn't exist.
        store[entity.Id] = component;
    }

    public bool TryGetComponent<TComponent>(Entity entity, out TComponent component) where TComponent : IComponent
    {
        if (_componentStores.TryGetValue(typeof(TComponent), out var store))
        {
            if (store.TryGetValue(entity.Id, out var c))
            {
                component = (TComponent) c;
                return true;
            }
        }

        component = default;
        return false;
    }

    #endregion

    #region Query

    public QueryBuilder Query()
    {
        // When a system calls Query(), we return a new QueryBuilder.
        return new QueryBuilder(this);
    }
    
    public Dictionary<int, IComponent> GetComponentStore(Type type)
    {
        // When the Query requests for a component, we return all the components of that type with their IDs.
        if (_componentStores.TryGetValue(type, out var store))
        {
            return store;
        }
        // If we don't have a store for that type, return an empty dictionary.
        return new Dictionary<int, IComponent>();
    }

    #endregion
    
    //Events.

    #region Collision Events
    
    // A constructor for collision events.
    public struct CollisionEvent
    {
        public Entity EntityA;
        public Entity EntityB;
        
        public CollisionEvent(Entity entityA, Entity entityB)
        {
            EntityA = entityA;
            EntityB = entityB;
        }
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