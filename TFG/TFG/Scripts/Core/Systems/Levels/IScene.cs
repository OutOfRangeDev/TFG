namespace TFG.Scripts.Core.Systems.Levels;

public interface IScene
{
    void Load(World.World world);
    void Unload(World.World world);
}