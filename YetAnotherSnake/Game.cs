using System;
using System.Runtime.InteropServices;
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
        
        public TimeSpan TargetFrameRate {
            get => TargetElapsedTime;
            set => TargetElapsedTime = value;
        }
        
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


            //Instance.Deactivated += (sender, args) => DisposeAll(); 
            //Instance.Disposed += (sender, e) => DisposeAll();
            EnableOrDisableCloseButton(false);
            GameServer = new GameServer();
            GameClient = new GameClient();
            //Console.WriteLine(TargetElapsedTime.TotalMilliseconds);
           // TargetElapsedTime = TimeSpan.FromSeconds(1d / 30d);
            IsFixedTimeStep = true;
            //Console.WriteLine(TargetElapsedTime.TotalMilliseconds);
            //AppDomain.CurrentDomain.ProcessExit += (sender, args) => Quit(); 
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
        
        [DllImport("user32.dll")]
        static extern bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);
        [DllImport("user32.dll")]
        static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
        internal const UInt32 SC_CLOSE = 0xF060;
        internal const UInt32 MF_ENABLED = 0x00000000;
        internal const UInt32 MF_GRAYED = 0x00000001;
        internal const UInt32 MF_DISABLED = 0x00000002;
        internal const uint MF_BYCOMMAND = 0x00000000;
     
        public void EnableOrDisableCloseButton(bool Enabled)
        {
            IntPtr hSystemMenu = GetSystemMenu(this.Window.Handle, false);
            EnableMenuItem(hSystemMenu, SC_CLOSE, (uint)(MF_ENABLED | (Enabled ? MF_ENABLED : MF_GRAYED)));
        }
    }
}
