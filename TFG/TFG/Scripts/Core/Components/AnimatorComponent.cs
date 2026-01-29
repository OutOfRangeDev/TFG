using System.Collections.Generic;
using TFG.Scripts.Core.Abstractions;

namespace TFG.Scripts.Core.Components;

public class AnimatorComponent : IComponent
{
    // All the animations for this entity.
    public Dictionary<string, Animation> Animations { get; set; }
    
    // The current animation playing.
    public string CurrentAnimation { get; set; }
    
    // --- Internals ---
    
    // The timer for the current frame.
    public float FrameTimer { get; set; }

    // The current showing frame.
    public int FrameIndex { get; set; }

    public AnimatorComponent()
    {
        Animations = new Dictionary<string, Animation>();
        CurrentAnimation = "None";
        FrameTimer = 0;
        FrameIndex = 0;
    }
}