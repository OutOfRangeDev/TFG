using Microsoft.Xna.Framework;
using TFG.Scripts.Core.Abstractions;

namespace TFG.Scripts.Game.Components.Combat;

public struct DashStateComponent : IComponent
{
    public bool IsDashing  { get; set; }
    public float Timer { get; set; }
    public float CooldownTimer { get; set; }
    public Vector2 Direction { get; set; }
}