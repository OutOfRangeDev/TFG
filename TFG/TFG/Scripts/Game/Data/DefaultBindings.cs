using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace TFG.Scripts.Game.Data;

public static class DefaultBindings
{
    public static Dictionary<PlayerAction, List<Keys>> GetDefaultKeyboardBindings()
    {
        return new Dictionary<PlayerAction, List<Keys>>
        {
            {PlayerAction.Jump, [Keys.Space] },
            {PlayerAction.Attack, [Keys.C]},
            {PlayerAction.Dash, [Keys.LeftShift]},
        };
    }
    
    public static Dictionary<PlayerAction, List<Buttons>> GetDefaultGamepadBindings()
    {
        return new Dictionary<PlayerAction, List<Buttons>>
        {
            {PlayerAction.Jump, [Buttons.A]},
            {PlayerAction.Attack, [Buttons.X]},
            {PlayerAction.Dash, [Buttons.RightShoulder]},
        };
    }
}