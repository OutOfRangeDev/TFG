using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TFG.Scripts.Core.Systems.Collisions;
using TFG.Scripts.Core.Systems.Core;
using TFG.Scripts.Core.Systems.Input;
using TFG.Scripts.Core.Systems.Physics;
using TFG.Scripts.Core.Systems.SpriteRenderer;

namespace TFG.Scripts.Core.World;

public class EntityFactory
{
    private static AssetManager _assetManager;

    public static void Initialize(AssetManager assetManager)
    {
        _assetManager = assetManager;
    }
    
    public static void CreatePlayerEntity(World world, Vector2 position)
    {
        var entity = world.CreateEntity();
        
        world.AddComponent(entity, new TransformComponent { Position = position });
        
        world.AddComponent(entity, new PlayerControllerComponent
        {
            Speed = 100f,
            JumpForce = 200f
        });
        
        world.AddComponent(entity, new ColliderComponent
        {
            IsTrigger = false,
            Layer = CollisionLayer.Player,
            Size = new Vector2(32, 32),
            Offset = Vector2.Zero
        });
        
        world.AddComponent(entity, new PhysicsComponent 
        { 
            Velocity = Vector2.Zero, 
            GravityScale = 1f, 
            Drag = 0.1f,
            IsStatic = false
        });
        
        world.AddComponent(entity, new SpriteComponent 
        { 
            Texture = _assetManager.Load<Texture2D>("Test/Character/hello_kitty"),
            SourceRectangle = new Rectangle(0, 0, 32, 32), 
            Color = Color.White,
            Rotation = 0f, 
            Origin = Vector2.Zero, 
            Scale = new Vector2(1f),
            Effects = SpriteEffects.None,
            LayerDepth = 0f 
        });
        
    }
}