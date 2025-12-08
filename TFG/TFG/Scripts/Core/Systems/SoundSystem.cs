using Microsoft.Xna.Framework;
using TFG.Scripts.Core.Abstractions;
using TFG.Scripts.Core.Managers;

namespace TFG.Scripts.Core.Systems;

public class SoundSystem : ISystem
{
    private readonly AudioManager _audioManager;

    public SoundSystem(AudioManager audioManager)
    {
        _audioManager = audioManager;
    }

    public void Update(Data.World world, GameTime gameTime)
    {
        foreach (var soundEvent in world.GetSoundEvents())
        {
            _audioManager.PlaySound(soundEvent.SoundToPlay);
        }
    }
}