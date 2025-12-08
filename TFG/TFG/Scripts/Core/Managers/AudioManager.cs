using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using TFG.Scripts.Core.Data;

namespace TFG.Scripts.Core.Managers;

public class AudioManager(ContentManager content)
{
    private readonly Random _random = new();
    
    // Dictionary to store loaded sounds.
    private readonly Dictionary<string, SoundEffect> _loadedSounds = new();
    
    // Dictionary to store the number of references to each sound.
    private readonly Dictionary<string, int> _soundReferenceCounts = new();

    public SoundEffect LoadSound(string soundName)
    {
        // Check if sound is already loaded.
        if (_loadedSounds.TryGetValue(soundName, out var sound))
        {
            // Increment reference count if sound is already loaded.
            _soundReferenceCounts[soundName]++;
            // Return the loaded sound.
            return sound;
        }
        else
        {
            // If it's not loaded, load the sound and add it to the dictionary.
            sound = content.Load<SoundEffect>(soundName);
            _loadedSounds[soundName] = sound;
            // And start the reference count at 1.
            _soundReferenceCounts[soundName] = 1;
            
            return sound;
        }
    }

    public void UnloadSound(string soundName)
    {
        // If the sound exists
        if (_soundReferenceCounts.TryGetValue(soundName, out var count))
        {
            // Decrement the reference count.
            count--;
            _soundReferenceCounts[soundName] = count;

            // If the reference count is 0, unload the sound.
            if (count == 0)
            {
                _loadedSounds.Remove(soundName);
                _soundReferenceCounts.Remove(soundName);
            }
        }
    }
    
    // This is a method for small sound effects.
    public void PlaySound(SoundData sound)
    {
        // Warning if sound is not loaded.
        if(!_loadedSounds.TryGetValue(sound.Name, out var soundEffect))
        {
            System.Diagnostics.Debug.WriteLine($"[WARNING - AUDIO MANAGER] Tried to play sound {sound.Name} not found or loaded.");
            return;
        }
        
        // Apply some variation.
        float finalVolume = sound.Volume + ((float)_random.NextDouble() * 2f - 1f) * sound.VolumeVariation;
        float finalPitch = sound.Pitch + ((float)_random.NextDouble() * 2f - 1f) * sound.PitchVariation;
        
        // Play the sound. Fire and forget.
        soundEffect.Play(
            volume : Math.Clamp(finalVolume, 0f, 1f),
            pitch : Math.Clamp(finalPitch, 0.5f, 2f),
            pan : 0f);
    }
}