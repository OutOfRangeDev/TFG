namespace TFG.Scripts.Core.Levels;

public interface IScene
{
    void Load(World.World world);
    void Unload(World.World world);
}