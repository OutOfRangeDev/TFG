using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TFG.Scripts.Core.Components;
using TFG.Scripts.Core.Data;

namespace TFG.Scripts.Core.Systems.UI;

public class UiRenderSystem(Viewport viewport)
{
    private readonly Viewport _viewport = viewport;
    
    public void Draw(World world, SpriteBatch spriteBatch)
    {
        var uiEntities = world.Query().
            With<RectTransformComponent>().
            With<UiImageComponent>().
            Execute();

        foreach (var entity in uiEntities)
        {
            ref var rectTransform = ref world.GetComponent<RectTransformComponent>(entity);
            ref var image = ref world.GetComponent<UiImageComponent>(entity);
            
            if(image.Texture == null) continue;

            var destinationBounds = rectTransform.GetBounds(_viewport);
            
            spriteBatch.Draw(
                texture: image.Texture,
                destinationRectangle: destinationBounds,
                sourceRectangle: image.SourceRectangle,
                color: image.Color,
                rotation: rectTransform.Rotation,
                origin: Vector2.Zero, 
                effects: SpriteEffects.None,
                layerDepth: 0f
                );
        }
        
        var textEntities = world.Query().
            With<RectTransformComponent>().
            With<UiTextComponent>().
            Execute();
        
        foreach (var entity in textEntities)
        {
            ref var rectTransform = ref world.GetComponent<RectTransformComponent>(entity);
            ref var text = ref world.GetComponent<UiTextComponent>(entity);
            
            if(text.Font == null || string.IsNullOrEmpty(text.Text)) continue;
            
            var destinationBounds = rectTransform.GetBounds(_viewport);

            Vector2 textSize = text.Font.MeasureString(text.Text);
            Vector2 position = new Vector2(destinationBounds.X, destinationBounds.Y);
            Vector2 origin = Vector2.Zero;

            if (text.Alignment == TextAlignment.Center)
            {
                position.X = destinationBounds.Center.X;
                origin.X = textSize.X / 2;
            }
            else if (text.Alignment == TextAlignment.Right)
            {
                position.X = destinationBounds.Right;
                origin.X = textSize.X;
            }
            
            spriteBatch.DrawString(
                spriteFont: text.Font, 
                text: text.Text, 
                position: position, 
                color: text.Color, 
                rotation: rectTransform.Rotation, 
                origin: origin, 
                scale: 1f, 
                effects: SpriteEffects.None, 
                layerDepth: 0f
                );
        }
    }
}