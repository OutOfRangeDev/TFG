using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TFG.Scripts.Core.Levels;
using TFG.Scripts.Core.Systems.Collisions;
using TFG.Scripts.Core.Systems.Core;
using TFG.Scripts.Core.Systems.Input;
using TFG.Scripts.Core.Systems.Levels;
using TFG.Scripts.Core.Systems.Physics;
using TFG.Scripts.Core.Systems.SpriteRenderer;
using TFG.Scripts.Core.World;

namespace TFG;

public class Game1 : Game
{
    private SpriteBatch _spriteBatch;

    private GraphicsDeviceManager _graphics;
    
    private World _world;
    private SystemManager _systemManager;
    private AssetManager _assetManager;
    private RenderSystem _renderSystem;
    private PhysicsSystem _physicsSystem;
    private CollisionSystem _collisionSystem;
    private SceneManager _sceneManager;
    private InputManager _inputManager;
    private PlayerInputSystem _playerInputSystem;
    
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
        _systemManager = new SystemManager();
        _renderSystem = new RenderSystem();
        _physicsSystem = new PhysicsSystem();
        _collisionSystem = new CollisionSystem();
        _sceneManager = new SceneManager(_world);
        _inputManager = new InputManager();
        _playerInputSystem = new PlayerInputSystem(_inputManager);
        
        // Then we register the systems.
        _systemManager.RegisterSystem(_playerInputSystem);
        _systemManager.RegisterSystem(_physicsSystem);
        _systemManager.RegisterSystem(_collisionSystem);
        
        
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _assetManager = new AssetManager(Content);
        _sceneManager.ChangeScene(new LdtkScene("Content/Test/TileMap/Test.ldtk", _assetManager));
        EntityFactory.Initialize(_assetManager);
        EntityFactory.CreatePlayerEntity(_world, new Vector2(100, 100));
    }

    protected override void Update(GameTime gameTime)
    {
        _inputManager.Update();
        
        if (_inputManager.IsKeyDown(Keys.Escape) || _inputManager.IsButtonDown(Buttons.Back)) Exit();
        
        // Update all the systems.
        _systemManager.Update(_world, gameTime);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // Draw all the entities.
        _spriteBatch.Begin(sortMode: SpriteSortMode.BackToFront, samplerState: SamplerState.PointClamp);
        _renderSystem.Draw(_world, _spriteBatch);
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}