using Microsoft.Xna.Framework;

namespace TFG.Scripts.Core.Systems;

public interface ISystem
{
    void Update(World.World world, GameTime gameTime);
}