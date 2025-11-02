using TFG.Scripts.Core.Levels;

namespace TFG.Scripts.Core.World;

public class SceneManager
{
    private readonly World _world;
    
    private IScene _currentScene;
    
    public SceneManager(World world)
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
        _currentScene.Load(_world);
    }
}