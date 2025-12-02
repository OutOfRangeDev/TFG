using TFG.Scripts.Core.Abstractions;

namespace TFG.Scripts.Core.World;

public class SceneManager
{
    private readonly Data.World _world;
    
    private IScene _currentScene;
    
    public SceneManager(Data.World world)
    {
        _world = world;
    }

    public void ChangeScene(IScene newScene)
    {
        // If we have a current scene, we unload it.
        _currentScene?.Unload(_world);
        
        // ADD A LOADING SCREEN.
        
        // Change the current scene.
        _currentScene = newScene;
        
        // Load the new scene.

        //Scene scene = new Scene(_currentScene.Load(_world, 0));
        _currentScene.Load(_world);
    }
}