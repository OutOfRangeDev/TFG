using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TFG.Scripts.Core.Data;
using TFG.Scripts.Core.Managers;
using TFG.Scripts.Core.Systems;
using TFG.Scripts.Core.Systems.Camera;
using TFG.Scripts.Core.Systems.Input;
using TFG.Scripts.Core.World;
using TFG.Scripts.Game.Data;
using TFG.Scripts.Game.Prefabs;
using TFG.Scripts.Game.Scenes;

namespace TFG;

public class Game1 : Game
{
    private SpriteBatch _spriteBatch;

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
    
    private AudioManager _audioManager;
    private SoundSystem _soundSystem;
    
    private Camera _camera;
    private CameraSystem _cameraSystem;
    
    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
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
        
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _assetManager = new AssetManager(Content);
        _sceneManager.ChangeScene(new LevelScene("Content/Test/TileMap/Test.ldtk", _assetManager));
        EntityFactory.Initialize(_assetManager);
        EntityFactory.CreatePlayerEntity(_world, new Vector2(100, 100));
    }

    protected override void Update(GameTime gameTime)
    {
        // Update the systems.
        // ------------ Core Systems ------------
        
        // First, the input manager.
        _inputManager.Update();
        
        // If the escape key is pressed, exit the game.
        if (_inputManager.IsKeyDown(Keys.Escape) || _inputManager.IsButtonDown(Buttons.Back)) Exit();
        
        _playerInputSystem.Update(_world, gameTime);
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
        _spriteBatch.Begin(transformMatrix: _camera.GetViewMatrix(), 
            sortMode: SpriteSortMode.BackToFront, 
            samplerState: SamplerState.PointClamp);
        
        _renderSystem.Draw(_world, _spriteBatch);
        
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}