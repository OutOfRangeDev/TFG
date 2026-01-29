namespace TFG.Scripts.Core.Components;

public struct Animation
{
    // Identifier.
    public string Name { get; set; }
    
    // The row of the sprite sheet to use.
    public int RowIndex { get; set; }
    
    // Number of frames in the animation.
    public int FrameCount { get; set; }
    
    // Duration of each frame.
    public float FrameDuration { get; set; }
    
    // Whether the animation should loop.
    public bool Loop { get; set; }

    public Animation()
    {
        Name = "None";
        RowIndex = 0;
        FrameCount = 1;
        FrameDuration = 0.1f;
        Loop = false;
    }
}