using System.Collections.Generic;
using System.Diagnostics;
using TFG.Scripts.Core.Systems.Core;

namespace TFG.Scripts.Core.World;

public class ComponentPool <T>: IComponentPool where T : IComponent
{
    // An array to hold all the components of the same type.
    private T[] _components = new T[1024]; // This number can be changed to hold more entities.
    
    // This list tells us which entity is in that slot.
    private readonly Dictionary<int, int> _entityIdToIndex = new();
    
    // Also a dictionary to get the entity ID from the index.
    private readonly Dictionary<int, int> _indexToEntityId = new();
    
    // Tracks the next index to use.
    private int _componentCount;

    #region Actions

    public void Add(int entityId, T component)
    {
        // Safety check. If the entity already has a component of this type, throw an exception.
        if (_entityIdToIndex.ContainsKey(entityId))
        {
            throw new System.Exception($"Component of type {typeof(T).Name} already exists for entity {entityId}.");
        }
        
        // Safety check. Resize the array if we are out of space.
        if (_componentCount >= _components.Length)
        {
            //The array is full!!! Making it bigger.
            Debug.WriteLine($"[WARNING] Component pool {typeof(T).Name} is full. Increasing size to {_components.Length * 2}.");
            // Double the size of the array.
            int newSize = _components.Length * 2;
            // Resize the array.
            System.Array.Resize(ref _components, newSize);
        }
        
        // Get the next index to use.
        int newIndex = _componentCount;
        
        // Add the component to the array open a slot in the array.
        _components[newIndex] = component;
        
        // Add the entity to the dictionary.
        _entityIdToIndex[entityId] = newIndex;
        _indexToEntityId[newIndex] = entityId;
        
        // Increment the next index.
        _componentCount++;
    }

    public bool Contains(int entityId)
    {
        return _entityIdToIndex.ContainsKey(entityId);
    }

    public ref T Get(int entityId)
    {
        // Get the array index for the entity.
        int index = _entityIdToIndex[entityId];

        // Return a reference to the component of that entity.
        return ref _components[index];
    }

    public ref T SafeGet(int entityId)
    {
        if (!_entityIdToIndex.TryGetValue(entityId, out var index))
        {
            Debug.WriteLine($"[WARNING] Entity {entityId} does not have a component of type {typeof(T).Name}.");
        }
        return ref _components[index];
    }

    public void Remove(int entityId)
    {
        // Safety check
        if (!_entityIdToIndex.TryGetValue(entityId, out var indexOfTheItemToRemove))
        {
            return;
        }
        // Create the variables to make the swap easier.
        int lastIndex = _componentCount - 1;
        T lastComponent = _components[lastIndex];
        int lastEntityId = _indexToEntityId[lastIndex];
        
        // We move the last item to the removed slot.
        _components[indexOfTheItemToRemove] = lastComponent;
        
        // We update the dictionary to point to the new slot.
        _entityIdToIndex[lastEntityId] = indexOfTheItemToRemove;
        _indexToEntityId[indexOfTheItemToRemove] = lastEntityId;
        
        // Now remove all the traces of the entity that we want to remove.
        // First the forward mapping.
        _entityIdToIndex.Remove(entityId);
        // And the reverse mapping for the last slot.
        _indexToEntityId.Remove(lastIndex);
        
        // Decrement the count.
        _componentCount--;
    }

    #endregion

    #region Query

    public IEnumerable<int> GetEntityIds()
    {
        return _entityIdToIndex.Keys;
    }

    #endregion
}