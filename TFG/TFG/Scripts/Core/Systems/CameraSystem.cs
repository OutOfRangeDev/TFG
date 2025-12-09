using System.Linq;
using Microsoft.Xna.Framework;
using TFG.Scripts.Core.Abstractions;
using TFG.Scripts.Core.Components;

namespace TFG.Scripts.Core.Systems;

public class CameraSystem(Managers.Camera camera) : ISystem
{
    
    public void Update(Data.World world, GameTime gameTime)
    {
        // Get the player entity. Right now is the only camera target expected.
        var playerEntity = world.Query().
            With<TransformComponent>().
            With<PlayerControllerComponent>().
            Execute()
            .SingleOrDefault(); //This ignores multiple entities with the player controller component.
                                //We will only get the first one or the default value.

        
        if (playerEntity.Id == 0) return; //If the player entity is not found, return. 
        // For now, it's not very specific, but if needed, we can throw an exception.
        
        //Set the camera position to the player position.
        camera.Position = world.GetComponent<TransformComponent>(playerEntity).Position;
    }
}