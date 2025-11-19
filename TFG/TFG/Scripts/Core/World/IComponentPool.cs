using System.Collections.Generic;

namespace TFG.Scripts.Core.World;

public interface IComponentPool
{
    void Remove(int entityId);
    IEnumerable<int> GetEntityIds();
}
