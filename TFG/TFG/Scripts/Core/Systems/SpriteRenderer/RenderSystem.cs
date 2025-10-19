using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;
using TFG.Scripts.Core.Systems.Core;

namespace TFG.Scripts.Core.Systems.SpriteRenderer;

public class RenderSystem
{
    public void Draw(World.World world, SpriteBatch spriteBatch)
    {
        //Get all entities with a sprite and transform component.
        var entitiesToDraw = world.Query()
            .With<SpriteComponent>()
            .With<TransformComponent>()
            .Execute();
        
        //When we have entities, draw them.
        foreach (var entity in entitiesToDraw)
        {
            //We get the sprite component from the entity.
            var sprite = world.GetComponent<SpriteComponent>(entity);
            //And the transform component from the entity.
            var transform = world.GetComponent<TransformComponent>(entity);
            
            //And draw the sprite.
            spriteBatch.Draw(
                texture: sprite.Texture,
                position: transform.Position,
                sourceRectangle: sprite.SourceRectangle,
                color: sprite.Color,
                rotation: sprite.Rotation,
                origin: sprite.Origin,
                scale: sprite.Scale,
                effects: sprite.Effects,
                layerDepth: sprite.LayerDepth
            );
            //Debug.WriteLine("Drawing entity " + entity.Id);
        }
    }
}