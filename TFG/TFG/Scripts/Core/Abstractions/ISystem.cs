using Microsoft.Xna.Framework;

namespace TFG.Scripts.Core.Abstractions;

public interface ISystem
{
    void Update(Data.World world, GameTime gameTime);
}