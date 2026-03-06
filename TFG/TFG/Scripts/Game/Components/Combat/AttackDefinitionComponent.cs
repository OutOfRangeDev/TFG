using Microsoft.Xna.Framework;
using TFG.Scripts.Core.Abstractions;

namespace TFG.Scripts.Game.Components.Combat;

public struct AttackDefinitionComponent : IComponent
{
    // DAMAGE AND PHYSICS
    public int Damage;
    public Vector2 TargetKnockback;
    public Vector2 SelfKnockback;
}