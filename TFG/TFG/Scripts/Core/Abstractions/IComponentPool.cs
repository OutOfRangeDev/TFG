using System.Collections.Generic;

namespace TFG.Scripts.Core.Abstractions;

public interface IComponentPool
{
    void Remove(int entityId);
    bool Contains(int entityId);
    int Count { get; }
    IEnumerable<int> GetEntityIds();
}
