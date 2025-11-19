using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TFG.Scripts.Core.Systems.Core;
namespace TFG.Scripts.Core.Levels;

public struct TilemapComponent : IComponent
{
    public Texture2D TilesetTexture { get; set; }
    public List<TileData> Tiles { get; set; }
}

public struct TileData()
{
    public Vector2 PositionInLevel;
    public Rectangle SourceRectangle;
}