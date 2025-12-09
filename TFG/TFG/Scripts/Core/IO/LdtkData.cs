using System.Text.Json.Serialization;

namespace TFG.Scripts.Core.IO;

// Here we hold the "global" data of the LDtk file.
public class LDtkProject
{
    [JsonPropertyName("defs")]
    public LDtkDefinition Defs {get; set;}
    [JsonPropertyName("levels")]
    public LDtkLevel[] Levels {get; set;}
}

// Here we hold the data of the various tilesets along the LDtk file.
public class LDtkDefinition
{
    [JsonPropertyName("tilesets")]
    public LDtkTileset[] Tilesets {get; set;}
}

// Here we hold the data of a single tileset.
public class LDtkTileset
{
    [JsonPropertyName("uid")]
    public int Uid {get; set;}
    [JsonPropertyName("relPath")]
    public string RelPath {get; set;} // Relative path to the tileset image.
    [JsonPropertyName("tileGridSize")]
    public int TileGridSize {get; set;} // Size of the tileset grid. 64 x 64 / 32 x 32...
}

// Here we hold the data of a single layer.
public class LDtkLevel
{
    [JsonPropertyName("identifier")]
    public string Identifier {get; set;} // To search a level by name.
    [JsonPropertyName("layerInstances")]
    public LDtkLayerInstance[] LayerInstances {get; set;}
}

// Each layer that has been created in the tilemap.
public class LDtkLayerInstance
{
    [JsonPropertyName("__identifier")]
    public string Identifier { get; set; } // Layer's name.
    [JsonPropertyName("__type")]
    public string Type { get; set; } // If the layer it's tile, int grid, or entities.
    [JsonPropertyName("__tilesetDefUid")]
    public int TilesetDefUid { get; set; } // The ID of the tileset.
    [JsonPropertyName("__gridSize")]
    public int GridSize { get; set; } // Size of the grid.
    [JsonPropertyName("gridTiles")]
    public LDtkTileInstance[] GridTiles { get; set; } // List of visual tiles.
    // public LdtkEntityInstance[] entities { get; set; } // List of entities.
    // public LdtkIntGridInstance[] intGrids { get; set; } // List of int grids.
    // Ready for the future, but not used yet.
}

// This is the data of a single tile.
public class LDtkTileInstance
{
    [JsonPropertyName("px")]
    public int[] Px { get; set; }  // Position of the tile.
    [JsonPropertyName("src")]
    public int[] Src { get; set; } // Position of the tile in the tileset.
    [JsonPropertyName("t")]
    public int T { get; set; }     // Tile ID.
}
