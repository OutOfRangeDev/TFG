using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TFG.Scripts.Core.Components;
using TFG.Scripts.Core.Data;
using TFG.Scripts.Core.Managers;
using TFG.Scripts.Core.Systems;
using TFG.Scripts.Core.Systems.UI;
using TFG.Scripts.Game.Data;
using TFG.Scripts.Game.Managers;
using TFG.Scripts.Game.Prefabs;
using TFG.Scripts.Game.Scenes;
using TFG.Scripts.Game.Systems.Combat;
using TFG.Scripts.Game.Systems.Enemy;
using TFG.Scripts.Game.Systems.Movement;

namespace TFG;

public class Game1 : Game
{
    // RESOLUTION VARIABLES
    private const int VirtualWidth = 480;
    private const int VirtualHeight = 270;
    private RenderTarget2D  _renderTarget;
    Viewport _virtualViewport = new Viewport(0, 0, VirtualWidth, VirtualHeight);
    
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
    
    // GAME
    
    private HitboxManager  _hitboxManager;
    private CombatSystem _combatSystem;
    private DamageSystem _damageSystem;
    private StatusSystem _statusSystem;
    private DeathSystem _deathSystem;
    private EnemyAiSystem _aiSystem;
    
    // DEBUG
    
    private FpsSystem _fpsSystem;
    
    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        Window.AllowUserResizing = true;
        _graphics.PreferredBackBufferWidth = 480 * 3;  
        _graphics.PreferredBackBufferHeight = 270 * 3; 
        
        // -------------------------------- FPS ADJUSTMENT--------------------------------
        
        // VSync
        _graphics.SynchronizeWithVerticalRetrace = true;
        
        // UNCAP
        //IsFixedTimeStep = false;
        //_graphics.SynchronizeWithVerticalRetrace = false;
        
        // 240/90...
        //TargetElapsedTime = System.TimeSpan.FromSeconds(1.0/240.0);
        
        _graphics.ApplyChanges();
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
        
        _camera = new Camera(VirtualWidth, VirtualHeight);
        _cameraSystem = new CameraSystem(_camera);
        
        _previousScreenSize = new Point(_virtualViewport.X, _virtualViewport.Y);
        _uiRenderSystem = new UiRenderSystem(_virtualViewport);
        _uiInteractionSystem = new UiInteractionSystem(_inputManager, _virtualViewport);
        
        // GAME
        _hitboxManager = new HitboxManager(_world);
        _combatSystem = new CombatSystem(_hitboxManager);
        _damageSystem  = new DamageSystem(_hitboxManager);
        _statusSystem = new StatusSystem();
        _deathSystem = new DeathSystem(_assetManager);
        _aiSystem = new EnemyAiSystem();
        
        // DEBUG
        _fpsSystem = new FpsSystem();
        
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _renderTarget =  new RenderTarget2D(GraphicsDevice, VirtualWidth, VirtualHeight);
        _assetManager = new AssetManager(Content);
        _prefabManager = new PrefabManager(_assetManager);
        string levelToLoad = System.IO.Path.Combine(Constants.ScenesDirectory, "Test.ldtk");
        _sceneManager.ChangeScene(new LevelScene(levelToLoad, _assetManager));
        EntityFactory.Initialize();
        _prefabManager.LoadPrefabs(Constants.PrefabDirectory);
        _prefabManager.InstantiatePrefab("Player", _world, new Vector2(100, 100));
        _prefabManager.InstantiatePrefab("Dummy",  _world, new Vector2(200, 100));
        CreateFpsEntity();
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
        
        // ------------ Game ------------
        _combatSystem.Update(_world, gameTime);
        _damageSystem.Update(_world, gameTime);
        _deathSystem.Update(_world, gameTime);
        _statusSystem.Update(_world, gameTime);
        _aiSystem.Update(_world, gameTime);
        
        // ------------ Clean up ------------
        _world.ClearCollisionEvents();
        _world.ClearSoundEvents();
        
        // ------------ Debug ------------
        _fpsSystem.Update(_world, gameTime);
        
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.SetRenderTarget(_renderTarget);
        
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
        
        GraphicsDevice.SetRenderTarget(null);
        
        GraphicsDevice.Clear(Color.Black);

        float scaleX = (float)GraphicsDevice.Viewport.Width / VirtualWidth;
        float scaleY = (float)GraphicsDevice.Viewport.Height / VirtualHeight;
        float finalScale = System.Math.Min(scaleX, scaleY);
        
        int newWidth = (int)(VirtualWidth * finalScale);
        int newHeight = (int)(VirtualHeight * finalScale);
        int offsetX = (GraphicsDevice.Viewport.Width - newWidth) / 2;
        int offsetY = (GraphicsDevice.Viewport.Height - newHeight) / 2;
        Rectangle destinationRect = new Rectangle(offsetX, offsetY, newWidth, newHeight);
        
        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
        _spriteBatch.Draw(_renderTarget, destinationRect, Color.White);
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

    private void CreateFpsEntity()
    {
        var debugFont = _assetManager.Load<SpriteFont>("Test/Fonts/DebugFont");
        
        var fpsEntity = _world.CreateEntity();
        
        _world.AddComponent(fpsEntity.Id, new FpsCounterTag());
        
        _world.AddComponent(fpsEntity.Id, new RectTransformComponent(
            UiAnchorPresets.TopRight,
            new Vector2(-10, 10),
            new Vector2(100, 30),
            new Vector2(1, 0),
            0f));
        
        _world.AddComponent(fpsEntity.Id, new UiTextComponent
        {
            Font = debugFont,
            Text = "FPS: --",
            Color = Color.White,
            Alignment = TextAlignment.Right
        });
    }
}