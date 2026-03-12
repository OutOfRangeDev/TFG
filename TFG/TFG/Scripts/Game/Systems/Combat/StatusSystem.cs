using Microsoft.Xna.Framework;
using TFG.Scripts.Core.Abstractions;
using TFG.Scripts.Core.Data;
using TFG.Scripts.Game.Components.Combat;

namespace TFG.Scripts.Game.Systems.Combat;

public class StatusSystem : ISystem
{
    // This script just updates the timers of the status components
    
    public void Update(World world, GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        
        // ---------------------------------------------------
        // INVINCIBILITY
        // ---------------------------------------------------
        
        #region Invincibility
        
        var invincibilityEntities = world.Query().
            With<InvincibleComponent>().
            Execute();
        foreach (var entity in invincibilityEntities)
        {
            ref var inv = ref world.GetComponent<InvincibleComponent>(entity);
            
            inv.Timer -= dt;
            if (inv.Timer <= 0) world.RemoveComponent<InvincibleComponent>(entity);
        }
        
        #endregion
        
        // ---------------------------------------------------
        // STUNNED
        // ---------------------------------------------------
        
        #region Stunned
        
        var stunnedEntities = world.Query().
            With<StunnedComponent>().
            Execute();
        foreach (var entity in stunnedEntities)
        {
            ref var stunned = ref world.GetComponent<StunnedComponent>(entity);
            
            stunned.Timer -= dt;
            if (stunned.Timer <= 0) world.RemoveComponent<StunnedComponent>(entity);
        }
        
        #endregion
    }
}