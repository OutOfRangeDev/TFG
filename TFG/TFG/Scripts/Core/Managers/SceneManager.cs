using TFG.Scripts.Core.Abstractions;

namespace TFG.Scripts.Core.Managers;

public class SceneManager(Data.World world)
{
    private IScene _currentScene;

    public void ChangeScene(IScene newScene)
    {
        // If we have a current scene, we unload it.
        _currentScene?.Unload(world);
        
        // ADD A LOADING SCREEN.
        
        // Change the current scene.
        _currentScene = newScene;
        
        // Load the new scene.

        //Scene scene = new Scene(_currentScene.Load(_world, 0));
        _currentScene.Load(world);
    }
}