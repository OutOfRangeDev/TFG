using TFG.Scripts.Core.Abstractions;

namespace TFG.Scripts.Game.Components.Combat;

// Flag component to indicate that an entity is invincible.
public struct InvincibleComponent : IComponent
{
    public float Timer {get; set;}
}