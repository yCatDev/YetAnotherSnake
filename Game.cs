﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Nez;
using Nez.Audio;
using YetAnotherSnake.Scenes;

namespace YetAnotherSnake
{
    public class MyGame : Core
    {
        public static AudioManager AudioManager;
        public static Scene GameScene, MenuScene;
        
        protected override void Initialize()
        {
            base.Initialize();
         
            Window.AllowUserResizing = false; 
           
            Core.DefaultSamplerState = SamplerState.LinearClamp;
                Screen.IsFullscreen = false;
            Screen.SetSize(1440,900);

            AudioManager = new AudioManager();
            
            
            
            Scene = new MenuScene();
            
        }
    }
}
