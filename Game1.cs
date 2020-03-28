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
            
            Scene = new Menu();
        }
    }
}
