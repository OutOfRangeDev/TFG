using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace TFG.Scripts.Game.Data;

public struct AttackDefinition
{
    // NAME
    public string AttackName;
    
    // TIMING
    public float StartUpTime;
    public float ActiveTime;
    public float RecoveryTime;
    
    // HITBOX
    public Vector2 HitboxSize;
    public Vector2 HitboxOffset;
    
    // DAMAGE AND PHYSICS
    public int Damage;
    public Vector2 TargetKnockback;
    public Vector2 SelfKnockback;
}

public static class MoveList
{
    public static Dictionary<string, AttackDefinition> Attacks = new ();
    
    
}