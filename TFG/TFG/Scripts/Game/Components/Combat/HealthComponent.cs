using TFG.Scripts.Core.Abstractions;

namespace TFG.Scripts.Game.Components.Combat;

public struct HealthComponent : IComponent
{
    public int MaxHealth { get; set; }
    public int CurrentHealth { get; set; }

    public float DamageMultiplier { get; set; } 
    public float StunDurationOnHit { get; set; } 
    
    public float CorpseLifespan { get; set; }
}