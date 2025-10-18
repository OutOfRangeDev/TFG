using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TFG.Scripts.Core.Systems.Core;
using TFG.Scripts.Core.Systems.Physics;
using TFG.Scripts.Core.Systems.SpriteRenderer;
using TFG.Scripts.Core.World;

namespace TFG;

public class Game1 : Game
{
    private SpriteBatch _spriteBatch;
    
    private World _world;
    private SystemManager _systemManager;
    private RenderSystem _renderSystem;
    private PhysicsSystem _physicsSystem;

    public Game1()
    {
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
        
        // Then we register the systems.
        _systemManager.RegisterSystem(_physicsSystem);
        
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();
        
        // Update all the systems.
        _systemManager.Update(_world, gameTime);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // Draw all the entities.
        _spriteBatch.Begin(sortMode: SpriteSortMode.BackToFront);
        _renderSystem.Draw(_world, _spriteBatch);
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}