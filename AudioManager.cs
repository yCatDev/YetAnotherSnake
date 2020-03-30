using System;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Nez;

namespace YetAnotherSnake
{
    public class AudioManager: IDisposable
    {
        private Song BackgroundAudio;
        public SoundEffect PickUpSound, DeathSound;
        
        public AudioManager()
        {
            BackgroundAudio = Core.Content.Load<Song>(Content.BackgroundMusic);
            PickUpSound = Core.Content.Load<SoundEffect>(Content.SoundPickUp);
            DeathSound = Core.Content.Load<SoundEffect>(Content.SoundDeath);
            MediaPlayer.Volume = 0.75f;
            MediaPlayer.IsRepeating = true;
        }

        public void PlayMusic() => MediaPlayer.Play(BackgroundAudio);
        public void StopMusic() => MediaPlayer.Stop();


        public void Dispose()
        {
            BackgroundAudio?.Dispose();
            PickUpSound?.Dispose();
            DeathSound?.Dispose();
        }
    }
}