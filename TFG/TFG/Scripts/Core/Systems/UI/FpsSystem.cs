using Microsoft.Xna.Framework;
using TFG.Scripts.Core.Abstractions;
using TFG.Scripts.Core.Components;
using TFG.Scripts.Core.Data;

namespace TFG.Scripts.Core.Systems.UI;

public struct FpsCounterTag : IComponent {}

public class FpsSystem : ISystem
{
    private int _frameCount;
    private float _timer;

    public void Update(World world, GameTime gameTime)
    {
        // Calculate the FPS
        _timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
        _frameCount++;

        // Only update the text every 0.5 secodns
        if (_timer >= 0.5f)
        {
            int fps = (int)(_frameCount / _timer);
            
            // Find FPS entity and update it
            var fpsEntities = world.Query().With<FpsCounterTag>().With<UiTextComponent>().Execute();

            if (fpsEntities.Count > 0)
            {
                ref var textComp = ref world.GetComponent<UiTextComponent>(fpsEntities[0]);
                textComp.Text = $"FPS: {fps}";
            }
            
            // Reset timer
            _frameCount = 0;
            _timer = 0;
        }
    }
}