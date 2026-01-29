using System.Collections.Generic;
using TFG.Scripts.Core.Components;
using TFG.Scripts.Core.Data;
using TFG.Scripts.Core.Managers;

namespace TFG.Scripts.Game.Prefabs;

public static class EntityFactory
{
    private static AssetManager _assetManager;

    public static void Initialize(AssetManager assetManager)
    {
        _assetManager = assetManager;
    }

    public static PrefabBlueprint CreatePlayerPrefab()
    {
        var blueprint = new PrefabBlueprint { Name = "Player" };

        blueprint.Components["TransformComponent"] = new{};
        
        blueprint.Components["PlayerControllerComponent"] = new
        {
            Speed = 100f,
            JumpForce = 500f
        };

        blueprint.Components["ColliderComponent"] = new
        {
            IsTrigger = false,
            Layer = CollisionLayer.Player,
            Size = new {X = 32, Y = 32},
        };
        
        blueprint.Components["PhysicsComponent"] = new
        {
            SkinWidth = 1f,
            GravityScale = 1f, 
            Drag = 0.1f,
            IsStatic = false
        };

        blueprint.Components["SpriteComponent"] = new
        {
            TextureName = "Test/Character/hello_kitty",
            SourceRectangle = new {Width = 32, Height = 32}, 
            Scale = new { X = 1f, Y = 1f }
        };
        
        blueprint.Components["AnimatorComponent"] = new
        {
            Animations = new Dictionary<string, object>
            {
                {
                    "Idle", new 
                    {
                        Name = "Idle",
                        RowIndex = 0,
                        FrameCount = 4,
                        FrameDuration = 0.2f,
                        Loop = true
                    }
                },
                {
                    "Run", new 
                    {
                        Name = "Run",
                        RowIndex = 2,
                        FrameCount = 4,
                        FrameDuration = 0.1f,
                        Loop = true
                    }
                }
            }
        };
        
        return blueprint;
    }
}