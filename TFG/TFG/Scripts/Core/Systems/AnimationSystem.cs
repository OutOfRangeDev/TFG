using System.Diagnostics;
using Microsoft.Xna.Framework;
using TFG.Scripts.Core.Abstractions;
using TFG.Scripts.Core.Components;

namespace TFG.Scripts.Core.Systems;

public class AnimationSystem : ISystem
{
    public void Update(Data.World world, GameTime gameTime)
    {
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        // Query to get all entities with an animator and sprite component.
        var entitiesToAnimate = world.Query().
            With<AnimatorComponent>().
            With<SpriteComponent>().
            Execute();

        foreach (var entity in entitiesToAnimate)
        {
            // Get the components.
            ref var animator = ref world.GetComponent<AnimatorComponent>(entity);
            ref var sprite = ref world.GetComponent<SpriteComponent>(entity);

            // ------------- Safety checks --------------
            
            // If there is no animation to play, or the animations dictionary is empty, we skip this entity.
            if (string.IsNullOrEmpty(animator.CurrentAnimation) || animator.Animations == null)
            {
                Debug.WriteLine($"[AnimationSystem] {entity} has no animations or no current animation.");
                continue;
            }

            // Get the current animation safely.
            if (!animator.Animations.TryGetValue(animator.CurrentAnimation, out var currentAnimation))
            {
                Debug.WriteLine($"[AnimationSystem] {entity} couldn't get the animation {animator.CurrentAnimation}.");
                continue;
            }

            // ------------- Animation logic --------------
            
            // Update the frame timer.
            animator.FrameTimer += deltaTime;

            // Check if it's time to change the frame.
            if (animator.FrameTimer >= currentAnimation.FrameDuration)
            {
                // Change the frame.
                animator.FrameIndex++;
                
                // Reset the timer. It's safer to do subtracting in case there is leftover time.
                animator.FrameTimer -= currentAnimation.FrameDuration;

                // Check if we reached the end of the animation. 
                if (animator.FrameIndex >= currentAnimation.FrameCount)
                {
                    if (currentAnimation.Loop)
                    {
                        // If we did, and loops, reset the frame index.
                        animator.FrameIndex = 0;
                    }
                    else
                    {
                        // Otherwise, set it to the last frame.
                        animator.FrameIndex = currentAnimation.FrameCount - 1;
                    }
                }
            }

            // ------------- Drawing logic --------------
            sprite.SourceRectangle = new Rectangle(
                animator.FrameIndex * sprite.SourceRectangle.Width,
                currentAnimation.RowIndex * sprite.SourceRectangle.Height,
                sprite.SourceRectangle.Width,
                sprite.SourceRectangle.Height);
        }
    }
}