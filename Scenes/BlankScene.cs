using Microsoft.Xna.Framework;
using Nez;

namespace YetAnotherSnake.Scenes
{
    public class BlankScene: Scene
    {
        public override void OnStart()
        {
            base.OnStart();
            ClearColor = Color.Black;
            Core.StartSceneTransition(new FadeTransition(() => new MenuScene()));
        }
    }
}