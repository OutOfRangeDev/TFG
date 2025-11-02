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

public struct TileData(Vector2 positionInLevel, Rectangle sourceRectangle)
{
    public Vector2 PositionInLevel = positionInLevel;
    public Rectangle SourceRectangle = sourceRectangle;
}