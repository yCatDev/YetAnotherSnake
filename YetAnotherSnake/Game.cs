using System;
using Microsoft.Xna.Framework;
using Nez;
using YetAnotherSnake.Scenes;
using Microsoft.Xna.Framework.Graphics;
using YetAnotherSnake.Multiplayer;
using Random = Nez.Random;

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
        
        public GameServer GameServer;
        public GameClient GameClient;
        
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

            GameServer = new GameServer();
            GameClient = new GameClient();

            Exiting += OnExit;
            
            //Loading empty scene, for animated loading of next scene 
            Scene = new BlankScene();
        }

        public static Vector2 CreateRandomPositionInWindowSpace()
        {
            var possibleWidth = 1280;
            var possibleHeight = 720;
            return new Vector2(Random.Range(-possibleWidth, possibleWidth),
                Random.Range(-possibleHeight, possibleHeight));
        }
        
        private void OnExit(object? sender, EventArgs e)
        {
            DisposeAll();
        }

        public void Quit()
        {
            DisposeAll();
            Environment.Exit(0);
        }

        private void DisposeAll()
        {
            AudioManager.Dispose();
            GameClient.Dispose();
            GameServer.Dispose();
        }
        
    }
}
