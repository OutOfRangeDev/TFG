using TFG.Scripts.Core.Abstractions;
using TFG.Scripts.Core.Data;

namespace TFG.Scripts.Game.Components.Combat;

public struct OwnerComponent : IComponent
{
    // IS MORE FOR DEBUG PURPOSES
    public Entity Owner {get; set;}
}