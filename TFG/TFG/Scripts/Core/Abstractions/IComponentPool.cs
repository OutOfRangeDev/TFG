using System.Collections.Generic;

namespace TFG.Scripts.Core.Abstractions;

public interface IComponentPool
{
    void Remove(int entityId);
    IEnumerable<int> GetEntityIds();
}
