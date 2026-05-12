using TFG.Scripts.Core.Abstractions;

namespace TFG.Scripts.Game.Components.Combat;

public struct DeadComponent : IComponent
{
    // FLAG - MARK TO READY DEAD AND EVENTUAL CLEAN UP
    public bool Stripped;
    public float Timer;
}