using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace TFG.Scripts.Core.Systems.Input;

public static class DefaultBindings
{
    public static Dictionary<string, Keys> GetDefaultKeyboardBindings()
    {
        return new Dictionary<string, Keys>
        {
            {"Jump", Keys.Space},
            {"MoveRight", Keys.D},
            {"MoveLeft", Keys.A},
            {"Attack", Keys.J}
        };
    }
    
    public static Dictionary<string, Buttons> GetDefaultGamepadBindings()
    {
        return new Dictionary<string, Buttons>
        {
            {"Jump", Buttons.A},
            {"Attack", Buttons.X}
            //Left and Right it's an axis. Joystick.X.
        };
    }
}