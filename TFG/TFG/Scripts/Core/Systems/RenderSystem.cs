using Microsoft.Xna.Framework.Graphics;
using TFG.Scripts.Core.Components;
using Color = Microsoft.Xna.Framework.Color;

namespace TFG.Scripts.Core.Systems;

public class RenderSystem
{
    public void Draw(Data.World world, SpriteBatch spriteBatch)
    {
        var tilemapEntities = world.Query(). With<TilemapComponent>().Execute();
        foreach (var entity in tilemapEntities)
        {
            var tilemap = world.GetComponent<TilemapComponent>(entity);
            
            if (tilemap.TilesetTexture == null || tilemap.Tiles == null) continue;

            foreach (var tile in tilemap.Tiles)
            {
                spriteBatch.Draw(
                    texture: tilemap.TilesetTexture,
                    position: tile.PositionInLevel,
                    sourceRectangle: tile.SourceRectangle,
                    color: Color.White
                    );
            }
        }
        
        //Get all entities with a sprite and transform component.
        var entitiesToDraw = world.Query()
            .With<SpriteComponent>()
            .With<TransformComponent>()
            .Execute();
        
        //System.Diagnostics.Debug.WriteLine($"[RenderSystem] Found {entitiesToDraw.Count()} entities to draw this frame.");
        
        //When we have entities, draw them.
        foreach (var entity in entitiesToDraw)
        {
            //System.Diagnostics.Debug.WriteLine($"--> Drawing entity {entity.Id}");
            //We get the sprite component from the entity.
            ref var sprite = ref world.GetComponent<SpriteComponent>(entity);
            //And the transform component from the entity.
            ref var transform = ref world.GetComponent<TransformComponent>(entity);
            
            if (sprite.Texture == null) continue;
            
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