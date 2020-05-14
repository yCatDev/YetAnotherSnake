using System;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Nez;

namespace YetAnotherSnake
{
    /// <summary>
    /// Audio controlling class
    /// </summary>
    public class AudioManager: IDisposable
    {
        private readonly Song BackgroundAudio;
        public readonly SoundEffect PickUpSound;
        public readonly SoundEffect DeathSound;

        public AudioManager()
        {
            BackgroundAudio = Core.Content.Load<Song>(Content.BackgroundMusic);
            PickUpSound = Core.Content.Load<SoundEffect>(Content.SoundPickUp);
            DeathSound = Core.Content.Load<SoundEffect>(Content.SoundDeath);
            MediaPlayer.Volume = 0.75f;
            MediaPlayer.IsRepeating = true;
        }

        public float Volume { get=>MediaPlayer.Volume; set=>MediaPlayer.Volume = value; }

        public void PlayMusic() => MediaPlayer.Play(BackgroundAudio);
        public void StopMusic() => MediaPlayer.Stop();

        public void PauseMusic() => MediaPlayer.Pause();

        public void ResumeMusic()
        {
            if (MediaPlayer.State==MediaState.Paused)
                MediaPlayer.Resume();
        }
        

        public void Dispose()
        {
            BackgroundAudio?.Dispose();
            PickUpSound?.Dispose();
            DeathSound?.Dispose();
        }
    }
}