namespace TFG.Scripts.Core.Systems.Animation;

public struct Animation
{
    // Identifier.
    public string Name;
    
    // The row of the sprite sheet to use.
    public int RowIndex;
    
    // Number of frames in the animation.
    public int FrameCount;
    
    // Duration of each frame.
    public float FrameDuration;
    
    // Whether the animation should loop.
    public bool Loop;
}