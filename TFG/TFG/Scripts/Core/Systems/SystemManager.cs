using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace TFG.Scripts.Core.Systems;

public class SystemManager
{
    //Systems.
    private readonly List<ISystem> _systems = new();
    
    public void RegisterSystem(ISystem system)
    {
        //Add the system to the list of systems.
        //We make this in Game1 manually.
        _systems.Add(system);
    }

    public void Update(World.World world ,GameTime gameTime)
    {
        //Update all systems.
        foreach (var system in _systems)
        {
            //Update the system.
            system.Update(world, gameTime);
        }
    }
}