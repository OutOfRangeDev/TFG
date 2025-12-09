using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using TFG.Scripts.Game.Player_Input;

namespace TFG.Scripts.Core.Managers;

public class InputManager
{
    // Input states.
    private KeyboardState _currentKeyboardState;
    private KeyboardState _previousKeyboardState;
    
    private GamePadState _currentGamePadState;
    private GamePadState _previousGamePadState;

    private readonly Dictionary<string, List<Keys>> _keyboardBindings = DefaultBindings.GetDefaultKeyboardBindings();
    private readonly Dictionary<string, List<Buttons>> _gamepadBindings = DefaultBindings.GetDefaultGamepadBindings();

    public void Update()
    {
        // Make the actual states the old states.
        _previousKeyboardState = _currentKeyboardState;
        _previousGamePadState = _currentGamePadState;

        // Update the current states.
        _currentKeyboardState = Keyboard.GetState();
        _currentGamePadState = GamePad.GetState(PlayerIndex.One);
    }
    
    // Keyboard functions.
    public bool IsKeyDown(Keys key) => _currentKeyboardState.IsKeyDown(key);
    public bool IsKeyUp(Keys key) => _currentKeyboardState.IsKeyUp(key);
    public bool IsKeyPressed(Keys key) => _currentKeyboardState.IsKeyDown(key) && _previousKeyboardState.IsKeyUp(key);
    public bool IsKeyReleased(Keys key) => _currentKeyboardState.IsKeyUp(key) && _previousKeyboardState.IsKeyDown(key);
    public bool IsKeyHeld(Keys key) => _currentKeyboardState.IsKeyDown(key);
    
    // GamePad functions.
    public bool IsButtonDown(Buttons button) => _currentGamePadState.IsButtonDown(button);
    public bool IsButtonUp(Buttons button) => _currentGamePadState.IsButtonUp(button);
    public bool IsButtonPressed(Buttons button) => _currentGamePadState.IsButtonDown(button) && _previousGamePadState.IsButtonUp(button);
    public bool IsButtonReleased(Buttons button) => _currentGamePadState.IsButtonUp(button) && _previousGamePadState.IsButtonDown(button);
    public bool IsButtonHeld(Buttons button) => _currentGamePadState.IsButtonDown(button);
    
    // Actions.
    public bool IsActionPressed(string action)
    {
        
        if (!_keyboardBindings.ContainsKey(action) && !_gamepadBindings.ContainsKey(action))
        {
            Debug.WriteLine($"[WARNING] InputManager was asked for an unknown action: '{action}'");
            return false;
        }
        

        // Check if the action is bound to a key or a button and return true if it is.
        
        if (_keyboardBindings.TryGetValue(action, out var key))
        {
            return key.Any(IsKeyPressed);
        }
        
        if (_gamepadBindings.TryGetValue(action, out var button))
        {
            return button.Any(IsButtonPressed);
        }
        
        return false;
    }
    
    public bool IsActionHeld(string action)
    {
        
        if (!_keyboardBindings.ContainsKey(action) && !_gamepadBindings.ContainsKey(action))
        {
            Debug.WriteLine($"[WARNING] InputManager was asked for an unknown action: '{action}'");
            return false;
        }
        

        // Check if the action is bound to a key or a button and return true if it is.
        
        if (_keyboardBindings.TryGetValue(action, out var key))
        {
            return key.Any(IsKeyHeld);
        }
        
        if (_gamepadBindings.TryGetValue(action, out var button))
        {
            return button.Any(IsButtonHeld);
        }
        
        return false;
    }
}