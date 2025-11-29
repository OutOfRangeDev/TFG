using System.Collections.Generic;
using TFG.Scripts.Core.Systems.Core;

namespace TFG.Scripts.Core.Systems.Animation;

public class AnimatorComponent : IComponent
{
    // All the animations for this entity.
    public Dictionary<string, Animation> Animations;
    
    // The current animation playing.
    public string CurrentAnimation;
    
    // --- Internals ---
    
    // The timer for the current frame.
    public float FrameTimer;

    // The current showing frame.
    public int FrameIndex;
}