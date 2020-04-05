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
        public static MyGame Instance;
        public  AudioManager AudioManager;
        public  Scene GameScene, MenuScene;
        public  GameSkin Skin;
        public  SaveSystem SaveSystem;
        public  VignettePostProcessor VignettePostProcessor;
        public  BloomPostProcessor BloomPostProcessor;
        public bool Pause = false;
        
        protected override void Initialize()
        {
            base.Initialize();
            Instance = this;
            var imGuiManager = new ImGuiManager();
            RegisterGlobalManager( imGuiManager );
            
            imGuiManager.SetEnabled(false);
            Console.WriteLine(Core.GraphicsDevice.Adapter.Description);
            Window.AllowUserResizing = false;
            ExitOnEscapeKeypress = false;
            
            DefaultSamplerState = SamplerState.LinearClamp;
            Screen.SetSize(Screen.MonitorWidth,Screen.MonitorHeight);

            AudioManager = new AudioManager();
           
            Skin = new GameSkin(Content);
            SaveSystem = new SaveSystem();
            
            VignettePostProcessor = new VignettePostProcessor(1) {Power = 0.75f};
            BloomPostProcessor = new BloomPostProcessor(3);

            //Here we go again
            Scene = new BlankScene();
        }
    }
}
