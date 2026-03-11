using System.Collections.Generic;
using TFG.Scripts.Core.Abstractions;

namespace TFG.Scripts.Game.Components.Combat;

public class HitboxStateComponent : IComponent
{
    public readonly HashSet<int> HitEntities = new();
    
    public void Reset()
    {
        HitEntities.Clear();
    }
}