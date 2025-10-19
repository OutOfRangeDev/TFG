using System;
using System.Collections.Generic;
using TFG.Scripts.Core.Systems.Core;
using TFG.Scripts.Core.World;

namespace TFG.Scripts.Core.Systems;

public class QueryBuilder
{
    //Variables
    //Save the state of the query as it is built.
    private readonly World.World _world;
    private readonly List<Type> _requiredComponents = new();

    // Constructor. Called by the World class.
    // The world is passed in to access the component stores.
    public QueryBuilder(World.World world)
    {
        _world = world;
    }
    
    //Methods
    //Add a required component to the query.
    public QueryBuilder With<TComponent>() where TComponent : IComponent
    {
        //Add the component to the list of required components.
        _requiredComponents.Add(typeof(TComponent));
        
        //Return the query builder / itself.
        return this;
    }

    public IEnumerable<Entity> Execute()
    {
        //If no required components, return empty.
        if(_requiredComponents.Count == 0)
            yield break;
        
        // First, get all the entities that have the component.
        var initialStore = _world.GetComponentStore(_requiredComponents[0]);
        // And make a set of their IDs.
        var validEntityIDs = new HashSet<int>(initialStore.Keys);
        
        // Then intersect with the rest of the components. Starting from index 1.
        for(int i = 1; i < _requiredComponents.Count; i++)
        {
            // Get the set of entity IDs that have the next component.
            var store = _world.GetComponentStore(_requiredComponents[i]);
            // And intersect with the previous set that we have.
            validEntityIDs.IntersectWith(store.Keys);
            // This stores the set of entity IDs that have both the components.
            // We repeat this for each component.
        }

        // Finally, return the entities with the required components.
        foreach (var id in validEntityIDs)
        {
            yield return new Entity(id);
        }
    }
    
}