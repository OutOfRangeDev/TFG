using TFG.Scripts.Core.Abstractions;

namespace TFG.Scripts.Game.Components.Combat;

public enum CombatPhase
{
    None,
    StartUp,
    Active,
    Recovery
}

public struct CombatStateComponent : IComponent
{
    // STATE MACHINE DATA
    public bool IsAttacking;
    public CombatPhase Phase;
    public float StateTimer;

    public string CurrentAttackName;
    
    // COMBO DATA
    public int ComboIndex;
    public double LastAttackEndTime;
    public bool HasHitEnemy;
    
    // HITBOX MANAGEMENT
    public int ActiveHitboxId;
    public bool HasSpawnedHitbox;
    
    // DEFAULT CONSTRUCTOR
    public CombatStateComponent()
    {
        IsAttacking = false;
        Phase = CombatPhase.None;
        StateTimer = 0f;
        CurrentAttackName = "";
        ComboIndex = 0;
        LastAttackEndTime = 0;
        ActiveHitboxId = -1; // -1 = NO HITBOX
        HasSpawnedHitbox = false;
    }
}