using TFG.Scripts.Core.Abstractions;

namespace TFG.Scripts.Game.Components.Enemies;

public enum AIState{Idle, Patrol, Chase, Attack}

public struct EnemyAIComponent : IComponent
{
    public AIState CurrentState  { get; set; }
    public int TargetEntityId {get; set;}
    public float PatrolDirection  {get; set;}
    
    public float DetectionRadius {get; set;}
    public float AttackRange  {get; set;}
    public float PatrolSpeed {get; set;}
    public float ChaseSpeed {get; set;}
    
    public string DefaultAttackName  {get; set;}

    public EnemyAIComponent()
    {
        CurrentState = AIState.Patrol;
        TargetEntityId = -1;
        PatrolDirection = 1f; // Start facing right by default
        
        DetectionRadius = 200f;
        AttackRange = 40f;
        PatrolSpeed = 50f;
        ChaseSpeed = 100f;
        
        DefaultAttackName = "Enemy_Basic";
    }
}