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

public static class GameAttacks
{
    public static Dictionary<string, AttackDefinition> MoveList = new ();

    static GameAttacks()
    {
        MoveList["Ground_Light_1"] = new AttackDefinition
        {
            AttackName =  "Ground_Light_1",
            StartUpTime =  0.1f,
            ActiveTime =  0.1f,
            RecoveryTime =  0.2f,
            HitboxSize =  new Vector2(50, 32),
            HitboxOffset =  new Vector2(0, 0),
            Damage =  10,
            TargetKnockback =   new Vector2(200, 200),
            SelfKnockback =   new Vector2(1, 0),
        };
        
        MoveList["Ground_Light_2"] = new AttackDefinition
        {
            AttackName =  "Ground_Light_2",
            StartUpTime =  0.15f,
            ActiveTime =  0.15f,
            RecoveryTime =  0.25f,
            HitboxSize =  new Vector2(1, 1),
            HitboxOffset =  new Vector2(1, 0),
            Damage =  10,
            TargetKnockback =   new Vector2(5, -3),
            SelfKnockback =   new Vector2(1, 0),
        };
        
        MoveList["Ground_Heavy_3"] = new AttackDefinition
        {
            AttackName =  "Ground_Heavy_3",
            StartUpTime =  0.25f,
            ActiveTime =  0.2f,
            RecoveryTime =  0.4f,
            HitboxSize =  new Vector2(1, 1),
            HitboxOffset =  new Vector2(1, 0),
            Damage =  10,
            TargetKnockback =   new Vector2(10, -5),
            SelfKnockback =   new Vector2(5, 0),
        };

        MoveList["Launch_Air"] = new AttackDefinition()
        {
            AttackName =  "Launch_Air",
            StartUpTime =  0.15f,
            ActiveTime =  0.15f,
            RecoveryTime =  0.3f,
            HitboxSize =  new Vector2(1, 1),
            HitboxOffset =  new Vector2(1, 0),
            Damage =  15,
            TargetKnockback =   new Vector2(10, -5),
            SelfKnockback =   new Vector2(5, 0),
        };
        
        MoveList["Air_Light_1"] = new AttackDefinition()
        {
            AttackName =  "Air_Light_1",
            StartUpTime =  0.1f,
            ActiveTime =  0.1f,
            RecoveryTime =  0.2f,
            HitboxSize =  new Vector2(1, 1),
            HitboxOffset =  new Vector2(1, 0),
            Damage =  10,
            TargetKnockback =   new Vector2(10, -5),
            SelfKnockback =   new Vector2(5, 0),
        };
        
        MoveList["Air_Light_2"] = new AttackDefinition()
        {
            AttackName =  "Air_Light_2",
            StartUpTime =  0.15f,
            ActiveTime =  0.2f,
            RecoveryTime =  0.3f,
            HitboxSize =  new Vector2(1, 1),
            HitboxOffset =  new Vector2(1, 0),
            Damage =  20,
            TargetKnockback =   new Vector2(10, -5),
            SelfKnockback =   new Vector2(5, 0),
        };

        MoveList["Enemy_Basic"] = new AttackDefinition()
        {
            AttackName =  "Enemy_Basic",
            StartUpTime =  0.5f,
            ActiveTime =  0.2f,
            RecoveryTime =  0.5f,
            HitboxSize =  new Vector2(1, 1),
            HitboxOffset =  new Vector2(1, 0),
            Damage =  10,
            TargetKnockback =   new Vector2(10, -5),
            SelfKnockback =   new Vector2(5, 0),
        };
    }
}