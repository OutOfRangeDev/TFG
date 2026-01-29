using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TFG.Scripts.Core.Components;
using TFG.Scripts.Core.Data;
using TFG.Scripts.Core.Managers;
using TFG.Scripts.Core.Systems;
using TFG.Scripts.Core.Systems.UI;
using TFG.Scripts.Game.Data;
using TFG.Scripts.Game.Player_Input;
using TFG.Scripts.Game.Prefabs;
using TFG.Scripts.Game.Scenes;

namespace TFG;

public class Game1 : Game
{
    private SpriteBatch _spriteBatch;

    // Resharper disable once NotAccessedField.Local
    private GraphicsDeviceManager _graphics;
    
    private World _world;
    private AssetManager _assetManager;
    private RenderSystem _renderSystem;
    private PhysicsSystem _physicsSystem;
    private CollisionSystem _collisionSystem;
    private SceneManager _sceneManager;
    private InputManager _inputManager;
    private PlayerInputSystem _playerInputSystem;
    private AnimationSystem _animationSystem;
    
    private UiRenderSystem _uiRenderSystem;
    private UiInteractionSystem _uiInteractionSystem;
    
    private AudioManager _audioManager;
    private SoundSystem _soundSystem;
    
    private Camera _camera;
    private CameraSystem _cameraSystem;
    
    private PrefabManager _prefabManager;
    
    private Point _previousScreenSize;
    
    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {

#if EXPORT_PREFABS

    PrefabExporter.ExportAllPrefabs("Content/Prefabs");
        Exit();
        return;
        
#endif
        
        // First, we instance the world and the systems.
        _world = new World();
        _renderSystem = new RenderSystem();
        _physicsSystem = new PhysicsSystem();
        
        var collisionMatrix = CollisionRules.CreateCollisionMatrix();
        _collisionSystem = new CollisionSystem(collisionMatrix);
        
        _sceneManager = new SceneManager(_world);
        _inputManager = new InputManager();
        _playerInputSystem = new PlayerInputSystem(_inputManager);
        _animationSystem = new AnimationSystem();
        
        _audioManager = new AudioManager(Content);
        _soundSystem = new SoundSystem(_audioManager);
        
        _camera = new Camera(GraphicsDevice.Viewport);
        _cameraSystem = new CameraSystem(_camera);
        
        _previousScreenSize = new Point(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
        _uiRenderSystem = new UiRenderSystem(GraphicsDevice.Viewport);
        _uiInteractionSystem = new UiInteractionSystem(_inputManager, GraphicsDevice.Viewport);
        
        base.Initialize();
    }

    protected override void LoadContent()
    {
        
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _assetManager = new AssetManager(Content);
        _prefabManager = new PrefabManager(_assetManager);
        string levelToLoad = System.IO.Path.Combine(Constants.ScenesDirectory, "Test.ldtk");
        _sceneManager.ChangeScene(new LevelScene(levelToLoad, _assetManager));
        EntityFactory.Initialize(_assetManager);
        _prefabManager.LoadPrefabs(Constants.PrefabDirectory);
        _prefabManager.InstantiatePrefab("Player", _world, new Vector2(100, 100));
    }

    protected override void Update(GameTime gameTime)
    {
        // Check if the screen size has changed.
        var currentScreenSize = new Point(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
        if (currentScreenSize != _previousScreenSize)
        {
            MarkAllUiOutdated(_world);
            _previousScreenSize = currentScreenSize;
        }
        
        // Update the systems.
        // ------------ Core Systems ------------
        
        // First, the input manager.
        _inputManager.Update();
        
        // If the escape key is pressed, exit the game.
        if (_inputManager.IsKeyDown(Keys.Escape) || _inputManager.IsButtonDown(Buttons.Back)) Exit();
        
        _playerInputSystem.Update(_world, gameTime);
        _uiInteractionSystem.Update(_world, gameTime);
        _physicsSystem.Update(_world, gameTime);
        _collisionSystem.Update(_world, gameTime);
        
        // ------------ Audio Systems ------------
        _soundSystem.Update(_world, gameTime);
        
        // ------------ Visual Systems ------------
        _cameraSystem.Update(_world, gameTime);
        _animationSystem.Update(_world, gameTime);
        
        // ------------ Clean up ------------
        _world.ClearCollisionEvents();
        _world.ClearSoundEvents();
        
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // Draw all the entities.
        
        // ------------ World Pass ------------
        _spriteBatch.Begin(transformMatrix: _camera.GetViewMatrix(), 
            sortMode: SpriteSortMode.BackToFront, 
            samplerState: SamplerState.PointClamp);
        _renderSystem.Draw(_world, _spriteBatch);
        _spriteBatch.End();
        
        // ------------ UI Pass ------------
        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
        _uiRenderSystem.Draw(_world, _spriteBatch);
        _spriteBatch.End();

        base.Draw(gameTime);
    }

    private void MarkAllUiOutdated(World world)
    {
        var uiEntities = world.Query().With<RectTransformComponent>().Execute();
        foreach (var entity in uiEntities)
        {
            ref var rectTransform = ref world.GetComponent<RectTransformComponent>(entity);
            rectTransform.MakeOutdated();
        }
    }
}