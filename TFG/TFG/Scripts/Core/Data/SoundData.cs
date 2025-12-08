namespace TFG.Scripts.Core.Data;

public readonly struct SoundData(string name, float volume, float pitch, float volumeVariation, float pitchVariation)
{
    //Sound name.
    public readonly string Name = name;
    
    //Audio parameters.
    public readonly float Volume = volume;
    public readonly float Pitch = pitch;
    
    //Variation parameters.
    public readonly float VolumeVariation = volumeVariation;
    public readonly float PitchVariation = pitchVariation;
}