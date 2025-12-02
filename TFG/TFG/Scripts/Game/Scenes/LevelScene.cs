using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TFG.Scripts.Core.Abstractions;
using TFG.Scripts.Core.Components;
using TFG.Scripts.Core.Components.Physics;
using TFG.Scripts.Core.Components.Tilemap;
using TFG.Scripts.Core.IO;
using TFG.Scripts.Core.Managers;

namespace TFG.Scripts.Game.Scenes;

public class LevelScene : IScene
{
    
    private readonly string _filePath;
    private readonly AssetManager _assetManager;
    
    public LevelScene(string filePath, AssetManager assetManager)
    {
        _filePath = filePath;
        _assetManager = assetManager;
    }
    
    public void Load(Core.Data.World world)
    {
        // First, read the file.
        var ldtkData = LdtkReader.LoadFromFile(_filePath);

        // Check if the file has levels.
        if (ldtkData.levels == null)
        {
            throw new Exception($"LDtk file {_filePath} has no levels.");
        }
        var levelToLoad = ldtkData.levels[0];
        
        // If it has levels, process, it's layers and entities.
        foreach (var layerInstance in levelToLoad.layerInstances)
        {
            switch (layerInstance.__type)
            {
                case "Tiles":
                    TranslateVisualLayer(world, layerInstance, ldtkData.defs);
                    TranslateCollisionFromTilesLayer(world, layerInstance);
                    break;
                case "IntGrid":
                    // IntGrids are not supported yet. It will be for invisible colliders.
                    break;
                case "Entities":
                    // Entities are not supported yet. 
                    break;
            }
        }
    }
    
    public void Unload(Core.Data.World world)
    {
        
    }

    private void TranslateVisualLayer(Core.Data.World world, LdtkLayerInstance layer, LdtkDefinition defs)
    {
        // Get the tileset.
        int requiredTilesetId = layer.__tilesetDefUid;
        LdtkTileset tileset = defs.tilesets.FirstOrDefault(t => t.uid == requiredTilesetId);

        // Check if the tileset exists.
        if (tileset == null)
        {
            throw new Exception($"Tileset with ID {requiredTilesetId} not found for layer {layer.__identifier}.");
        }

        // Load the tileset variables.
        string levelDirectory = Path.GetDirectoryName(_filePath);
        string finalTexturePath = Path.Combine(levelDirectory, tileset.relPath);
        string assetPathToLoad = finalTexturePath
            .Replace("Content\\", "") 
            .Replace(".png", ""); 
        var tilesetTexture = _assetManager.Load<Texture2D>(assetPathToLoad);
        int tileSize = tileset.tileGridSize;

        // Create the list of tiles for the tilemap.
        var tiles = new List<TileData>();

        // Loop through the tiles and create the tile data.
        foreach (var tileInstance in layer.gridTiles)
        {
            var positionInLevel = new Vector2(tileInstance.px[0], tileInstance.px[1]);

            var sourceOnTexture = new Rectangle(
                tileInstance.src[0],
                tileInstance.src[1],
                tileSize,
                tileSize);

            tiles.Add(new TileData
                { 
                    PositionInLevel = positionInLevel, 
                    SourceRectangle = sourceOnTexture
                }
            );
        }

        // Create the tilemap entity.
        var tilemapEntity = world.CreateEntity();
        // Add the tilemap component.
        world.AddComponent(tilemapEntity, new TilemapComponent
        {
            TilesetTexture = tilesetTexture,
            Tiles = tiles
        });
        
        Debug.WriteLine($"Created tilemap entity with ID {tilemapEntity.Id}.");
    }

    private void TranslateCollisionFromTilesLayer(Core.Data.World world, LdtkLayerInstance layerInstance)
    {
        // Get the grid size.
        int gridSize = layerInstance.__gridSize;

        // Loop through the tiles and create the ground entities.
        foreach (var tileInstance in layerInstance.gridTiles)
        {
            // Check if the tile is not empty.
            if (tileInstance.t != 0)
            {
                // Create the ground entity.
                var groundEntity = world.CreateEntity();
                
                // Get the position and add the components.
                var position = new Vector2(tileInstance.px[0], tileInstance.px[1]);
                
                // Add the components.
                world.AddComponent(groundEntity, new TransformComponent{Position = position});
                world.AddComponent(groundEntity, new ColliderComponent
                {
                    Layer = CollisionLayer.Environment,
                    Size = new Vector2(gridSize, gridSize)
                });
                world.AddComponent(groundEntity, new PhysicsComponent{IsStatic = true});
            }
        }
    }
}