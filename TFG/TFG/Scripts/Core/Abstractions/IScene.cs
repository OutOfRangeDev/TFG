namespace TFG.Scripts.Core.Abstractions;

public interface IScene
{
    void Load(Data.World world);
    void Unload(Data.World world);
}