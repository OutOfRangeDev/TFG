using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace TFG.Scripts.Game.Player_Input;

public static class DefaultBindings
{
    public static Dictionary<string, List<Keys>> GetDefaultKeyboardBindings()
    {
        return new Dictionary<string, List<Keys>>
        {
            {"Jump", [Keys.Space] },
            {"MoveRight", [Keys.D, Keys.Right]},
            {"MoveLeft", [Keys.A, Keys.Left]},
            {"Attack", [Keys.J]}
        };
    }
    
    public static Dictionary<string, List<Buttons>> GetDefaultGamepadBindings()
    {
        return new Dictionary<string, List<Buttons>>
        {
            {"Jump", [Buttons.A]},
            {"Attack", [Buttons.X]}
            //Left and Right it's an axis. Joystick.X.
        };
    }
}