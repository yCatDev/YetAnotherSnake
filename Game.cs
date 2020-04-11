using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Nez;
using Nez.Audio;
using Nez.ImGuiTools;
using YetAnotherSnake.Scenes;

namespace YetAnotherSnake
{
    public class MyGame : Core
    {
        /// <summary>
        /// Game Instance
        /// </summary>
        public static MyGame GameInstance;
        /// <summary>
        /// Game audio manager
        /// </summary>
        public  AudioManager AudioManager;
        /// <summary>
        /// Save file
        /// </summary>
        public  SaveSystem SaveSystem;
        
        /// <summary>
        /// Vignette
        /// </summary>
        public  VignettePostProcessor VignettePostProcessor;
        public  BloomPostProcessor BloomPostProcessor;
        public bool Pause = false;
        
        protected override void Initialize()
        {
            base.Initialize();
            GameInstance = this;

            //Base setting for game window
            Window.AllowUserResizing = false;
            ExitOnEscapeKeypress = false;
            
            //Set texture filter method
            DefaultSamplerState = SamplerState.LinearClamp;
            Screen.SetSize(Screen.MonitorWidth,Screen.MonitorHeight);

            AudioManager = new AudioManager();
            SaveSystem = new SaveSystem();
            
            VignettePostProcessor = new VignettePostProcessor(1) {Power = 0.75f};
            BloomPostProcessor = new BloomPostProcessor(3);

            //Loading empty scene, for animated loading of next scene 
            Scene = new BlankScene();
        }
    }
}
