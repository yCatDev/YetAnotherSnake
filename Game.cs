using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nez;
using YetAnotherSnake.Scenes;

namespace YetAnotherSnake
{
    public class MyGame : Core
    {
        protected override void Initialize()
        {
            base.Initialize();
            Window.AllowUserResizing = false; 
            Screen.EnableMultiSampling = true;
            Screen.IsFullscreen = false;
            Screen.SetSize(1440,900);
            GraphicsDevice.SamplerStates[0] = new SamplerState()
            {
                Filter = TextureFilter.Anisotropic
            };
            Scene = new GameScene();
            
        }
    }
}
