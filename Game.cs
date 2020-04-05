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
        
        
        protected override void Initialize()
        {
            base.Initialize();
            Instance = this;
            var imGuiManager = new ImGuiManager();
            Core.RegisterGlobalManager( imGuiManager );

// toggle ImGui rendering on/off. It starts out enabled.
            imGuiManager.SetEnabled(false);
            
            Window.AllowUserResizing = false; 
           
            DefaultSamplerState = SamplerState.LinearClamp;
            Screen.SetSize(Screen.MonitorWidth,Screen.MonitorHeight);

            AudioManager = new AudioManager();
           
            Skin = new GameSkin(Content);
            SaveSystem = new SaveSystem();
            
            VignettePostProcessor = new VignettePostProcessor(1) {Power = 0.75f};
            BloomPostProcessor = new BloomPostProcessor(3);

            //Here we go again
            Scene = new MenuScene();
        }
    }
}
