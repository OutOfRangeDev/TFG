using System.Collections.Generic;
using System.Diagnostics;

namespace TFG.Scripts.Core.World;

public class World
{
    //Entities.
    ///List of entities. HashSet is for fast lookup.
    private HashSet<Entity> _activeEntities = new HashSet<Entity>();
    ///Next entity id.
    private int _nextEntityId = 0;
    ///Queue of available ids. Store deleted entity ID's.
    private Queue<int> _availableIDs = new Queue<int>();

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
        return newEntity;
    }

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
}