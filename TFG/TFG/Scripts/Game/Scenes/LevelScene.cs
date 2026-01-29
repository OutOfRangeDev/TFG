using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TFG.Scripts.Core.Abstractions;
using TFG.Scripts.Core.Components;
using TFG.Scripts.Core.Data;
using TFG.Scripts.Core.IO;
using TFG.Scripts.Core.Managers;

namespace TFG.Scripts.Game.Scenes;

public class LevelScene : IScene
{
    private readonly string _filePath;
    private readonly AssetManager _assetManager;

    // Constructor.
    public LevelScene(string filePath, AssetManager assetManager)
    {
        // Check if the file path name is valid.
        if(string.IsNullOrEmpty(filePath))
            throw new ArgumentNullException(nameof(filePath), "File path cannot be null or empty.");
        
        // Save the file path.
        _filePath = filePath;
        _assetManager = assetManager;
        
        if (assetManager == null)
            throw new ArgumentNullException(nameof(assetManager), "Asset manager cannot be null.");
    }
    
    public void Load(World world)
    {
        // First, read the file.
        var ldtkData = LDtkReader.LoadFromFile(_filePath);

        // Check if the file has levels.
        if (ldtkData.Levels == null)
        {
            throw new Exception($"LDtk file {_filePath} has no levels.");
        }
        var levelToLoad = ldtkData.Levels[0];
        
        // If it has levels, process, it's layers and entities.
        foreach (var layerInstance in levelToLoad.LayerInstances)
        {
            switch (layerInstance.Type)
            {
                case "Tiles":
                    TranslateVisualLayer(world, layerInstance, ldtkData.Defs);
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

    private void TranslateVisualLayer(World world, LDtkLayerInstance layer, LDtkDefinition defs)
    {
        // Get the tileset.
        int requiredTilesetId = layer.TilesetDefUid;
        LDtkTileset tileset = defs.Tilesets.FirstOrDefault(t => t.Uid == requiredTilesetId);

        // Check if the tileset exists.
        if (tileset == null)
        {
            throw new Exception($"Tileset with ID {requiredTilesetId} not found for layer {layer.Identifier}.");
        }
        
        // Now check if the properties are set correctly.
        if (string.IsNullOrEmpty(tileset.RelPath))
        {
            Debug.WriteLine($"[WARNING - SCENE LEVEL] - Tileset '{tileset.Uid}' has no image path. Skipping visual layer '{layer.Identifier}'.");
            return;
        }

        // Load the tileset variables.
        string levelDirectory = Path.GetDirectoryName(_filePath);
        string finalTexturePath = Path.Combine(levelDirectory!, tileset.RelPath);
        string assetPathToLoad = finalTexturePath
            .Replace("Content\\", "") 
            .Replace(".png", ""); 
        var tilesetTexture = _assetManager.Load<Texture2D>(assetPathToLoad);
        int tileSize = tileset.TileGridSize;

        // Create the list of tiles for the tilemap.
        var tiles = new List<TileData>();

        // Loop through the tiles and create the tile data.
        foreach (var tileInstance in layer.GridTiles)
        {
            var positionInLevel = new Vector2(tileInstance.Px[0], tileInstance.Px[1]);

            var sourceOnTexture = new Rectangle(
                tileInstance.Src[0],
                tileInstance.Src[1],
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

    private void TranslateCollisionFromTilesLayer(Core.Data.World world, LDtkLayerInstance layerInstance)
    {
        // Get the grid size.
        int gridSize = layerInstance.GridSize;

        // Loop through the tiles and create the ground entities.
        foreach (var tileInstance in layerInstance.GridTiles)
        {
            // Check if the tile is not empty.
            if (tileInstance.T != 0)
            {
                // Create the ground entity.
                var groundEntity = world.CreateEntity();
                
                // Get the position and add the components.
                var position = new Vector2(tileInstance.Px[0], tileInstance.Px[1]);
                
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