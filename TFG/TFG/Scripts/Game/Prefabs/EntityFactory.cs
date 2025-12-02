using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TFG.Scripts.Core.Components;
using TFG.Scripts.Core.Components.Animation;
using TFG.Scripts.Core.Components.Physics;
using TFG.Scripts.Core.Managers;

namespace TFG.Scripts.Game.Prefabs;

public class EntityFactory
{
    private static AssetManager _assetManager;

    public static void Initialize(AssetManager assetManager)
    {
        _assetManager = assetManager;
    }
    
    public static void CreatePlayerEntity(Core.Data.World world, Vector2 position)
    {
        var entity = world.CreateEntity();
        
        world.AddComponent(entity, new TransformComponent { Position = position });
        
        world.AddComponent(entity, new PlayerControllerComponent
        {
            Speed = 100f,
            JumpForce = 500f
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
            SkinWidth = 1f,
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
        
        world.AddComponent(entity, new AnimatorComponent
        {
            Animations = new Dictionary<string, Animation>
            {
                {
                    "Idle", new Animation
                    {
                        Name = "Idle",
                        RowIndex = 0,
                        FrameCount = 4,
                        FrameDuration = 0.2f,
                        Loop = true
                    }
                },
                {
                    "Run", new Animation
                    {
                        Name = "Run",
                        RowIndex = 2,
                        FrameCount = 4,
                        FrameDuration = 0.1f,
                        Loop = true
                    }
                }
            },
        });
    }
}