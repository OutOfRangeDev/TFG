namespace TFG.Scripts.Core.IO;

// Here we hold the "global" data of the LDtk file.
public class LdtkProject
{
    public LdtkDefinition defs {get; set;}
    public LdtkLevel[] levels {get; set;}
}

// Here we hold the data of the various tilesets along the LDtk file.
public class LdtkDefinition
{
    public LdtkTileset[] tilesets {get; set;}
}

// Here we hold the data of a single tileset.
public class LdtkTileset
{
    public int uid {get; set;}
    public string relPath {get; set;} // Relative path to the tileset image.
    public int tileGridSize {get; set;} // Size of the tileset grid. 64 x 64 / 32 x 32...
}

// Here we hold the data of a single layer.
public class LdtkLevel
{
    public string identifier {get; set;} // To search a level by name.
    public LdtkLayerInstance[] layerInstances {get; set;}
}

// Each layer that has been created in the tilemap.
public class LdtkLayerInstance
{
    public string __identifier { get; set; } // Layer's name.
    public string __type { get; set; } // If the layer it's tile, int grid, or entities.
    public int __tilesetDefUid { get; set; } // The ID of the tileset.
    public int __gridSize { get; set; } // Size of the grid.
    public LdtkTileInstance[] gridTiles { get; set; } // List of visual tiles.
    // public LdtkEntityInstance[] entities { get; set; } // List of entities.
    // public LdtkIntGridInstance[] intGrids { get; set; } // List of int grids.
    // Ready for the future, but not used yet.
}

// This is the data of a single tile.
public class LdtkTileInstance
{
    public int[] px { get; set; }  // Position of the tile.
    public int[] src { get; set; } // Position of the tile in the tileset.
    public int t { get; set; }     // Tile ID.
}
