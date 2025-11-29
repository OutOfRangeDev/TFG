using System;
using System.Collections.Generic;
using TFG.Scripts.Core.World;

namespace TFG.Scripts.Core.Systems.Core;

public class QueryBuilder(World.World world)
{
    //Variables
    //Save the state of the query as it is built.
    private readonly List<Type> _requiredComponents = new();

    // Constructor. Called by the World class.
    // The world is passed in to access the component stores.

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
        var initialIds = world.GetEntityIdsForComponent(_requiredComponents[0]);

        //If there are no entities with that component, return empty.
        if (initialIds == null)
        {
            yield break;
        }
        var validEntityIds = new HashSet<int>(initialIds);

        // If not, then we get the ids of all the entities that have all the required components.
        for (int i = 1; i < _requiredComponents.Count; i++)
        {
            var ids = world.GetEntityIdsForComponent(_requiredComponents[i]);
            
            if (ids == null)
            {
                yield break;
            }
            
            //Intersect the ids with the current set of valid ids.
            validEntityIds.IntersectWith(ids);
        }
        
        //And send the answer

        foreach (var id in validEntityIds)
        {
            yield return new Entity(id);
        }
    }
    
}