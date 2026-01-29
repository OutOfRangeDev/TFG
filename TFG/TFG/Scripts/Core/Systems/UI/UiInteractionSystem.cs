using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TFG.Scripts.Core.Abstractions;
using TFG.Scripts.Core.Components;
using TFG.Scripts.Core.Data;
using TFG.Scripts.Core.Managers;

namespace TFG.Scripts.Core.Systems.UI;

public class UiInteractionSystem(InputManager inputManager, Viewport viewport) : ISystem
{
    private readonly InputManager _inputManager = inputManager;
    private readonly Viewport _viewport = viewport;

    public void Update(World world, GameTime gameTime)
    {
        var mousePosition = _inputManager.GetMousePosition();
        
        var buttonEntities = world.Query().
            With<RectTransformComponent>().
            With<UiButtonComponent>().
            With<UiImageComponent>().
            Execute();

        foreach (var entity in buttonEntities)
        {
            ref var rectTransform = ref world.GetComponent<RectTransformComponent>(entity);
            ref var button = ref world.GetComponent<UiButtonComponent>(entity);
            ref var image = ref world.GetComponent<UiImageComponent>(entity);
            
            if(!button.IsInteractable) continue;
            
            var bounds = rectTransform.GetBounds(_viewport);

            bool isHovered = bounds.Contains(mousePosition);
            
            // ------------ State Machine ------------

            button.State = ButtonState.Normal;

            if (isHovered)
            {
                button.State = ButtonState.Hovered;
                
                if(_inputManager.IsMouseButtonHeld(InputManager.MouseButton.Left))
                {
                    button.State = ButtonState.Pressed;
                }
                if(_inputManager.IsMouseButtonReleased(InputManager.MouseButton.Left))
                {
                    button.OnClick?.Invoke();
                }
            }
            
            // ------------ Visual Feedback ------------
            image.Texture = button.State switch
            {
                ButtonState.Normal => button.NormalTexture,
                ButtonState.Hovered => button.HoveredTexture,
                ButtonState.Pressed => button.PressedTexture,
                _ => button.NormalTexture
            };
        }
    }
}