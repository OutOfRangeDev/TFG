using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using TFG.Scripts.Core.Abstractions;

namespace TFG.Scripts.Core.Components;

public struct TilemapComponent : IComponent
{
    public Texture2D TilesetTexture { get; set; }
    public List<TileData> Tiles { get; set; }
}

