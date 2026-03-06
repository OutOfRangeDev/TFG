using System;
using System.Collections.Generic;
using TFG.Scripts.Core.Abstractions;

namespace TFG.Scripts.Core.Data;

public class Query
{
    private readonly World _world;
    private readonly List<Type> _requiredComponents = new();

    // This is a static shared buffer, to avoid creating a new list every frame
    // Although not safe for games with multiple updates (multi-thread)
    private static readonly List<int> ResultBuffer = new(1024);

    public Query(World world)
    {
        _world = world;
    }

    public Query With<TComponent>() where TComponent : IComponent
    {
        _requiredComponents.Add(typeof(TComponent));
        return this;
    }

    public List<int> Execute()
    {
        ResultBuffer.Clear();

        if (_requiredComponents.Count == 0) return ResultBuffer;
        
        // 1. Get the pool with the fewest entities to optimize intersection.
        IComponentPool smallestPool = null;
        int minCount = int.MaxValue;
        int smallestIndex = -1;

        for (int i = 0; i < _requiredComponents.Count; i++)
        {
            var pool = _world.GetPool(_requiredComponents[i]);

            if (pool == null || pool.Count == 0)
            {
                return  ResultBuffer;
            }

            if (pool.Count < minCount)
            {
                minCount = pool.Count;
                smallestPool = pool;
                smallestIndex = i;
            }
        }

        if (smallestPool == null) throw new Exception("[QUERY] POOL IS NULL");
        
        foreach (var entityId in smallestPool.GetEntityIds())
        {
            bool hasAll = true;

            for (int i = 0; i < _requiredComponents.Count; i++)
            {
                if(i == smallestIndex) continue;
                
                if (!_world.HasComponent(entityId, _requiredComponents[i]))
                {
                    hasAll = false;
                    break;
                }
            }

            if (hasAll)
            {
                ResultBuffer.Add(entityId);
            }
        }
        
        return ResultBuffer;
    }
}