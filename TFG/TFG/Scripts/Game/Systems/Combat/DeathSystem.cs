using Microsoft.Xna.Framework;
using TFG.Scripts.Core.Abstractions;
using TFG.Scripts.Core.Components;
using TFG.Scripts.Core.Data;
using TFG.Scripts.Core.Managers;
using TFG.Scripts.Game.Components.Combat;

namespace TFG.Scripts.Game.Systems.Combat;

public class DeathSystem(AssetManager assetManager) : ISystem
{
    public void Update(World world, GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        var entities = world.Query().With<DeadComponent>().Execute();

        foreach (var entity in entities)
        {
            ref var deathComponent = ref world.GetComponent<DeadComponent>(entity);
            ref var spriteComponent = ref world.GetComponent<SpriteComponent>(entity);
            
            if(!deathComponent.Stripped)
            {
                // PHASE 1
                // STRIP HIM ANYTHING THAT CAN INTERFERE
                if (world.HasComponent<ColliderComponent>(entity)) world.RemoveComponent<ColliderComponent>(entity);
                if (world.HasComponent<HitStopComponent>(entity)) world.RemoveComponent<HitStopComponent>(entity);
                if (world.HasComponent<CombatStateComponent>(entity)) world.RemoveComponent<CombatStateComponent>(entity);
                if (world.HasComponent<StunnedComponent>(entity))world.RemoveComponent<StunnedComponent>(entity);

                if (world.HasComponent<AnimatorComponent>(entity))
                {
                    ref var animator = ref world.GetComponent<AnimatorComponent>(entity);
                    animator.CurrentAnimation = "Death";
                    animator.FrameIndex = 0;
                    // IsFinished?
                }
                deathComponent.Stripped = true;
            }
            
            // PHASE 2
            // ANIMATION FLAG FINISHED
            deathComponent.Timer -= dt;

            // DIE
            if (deathComponent.Timer <= 0)
            {
                if (world.HasComponent<SpriteComponent>(entity))
                {
                    var sprite = world.GetComponent<SpriteComponent>(entity);
                    assetManager.Unload(sprite.TextureName);
                }
                
                world.DestroyEntity(entity);
            }
        }
    }
}