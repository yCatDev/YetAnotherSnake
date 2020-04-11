using Microsoft.Xna.Framework;
using Nez;

namespace YetAnotherSnake.Scenes
{
    
    /// <summary>
    /// Empty scene for creating animated transition of other scenes
    /// </summary>
    public class BlankScene: Scene
    {
        public override void OnStart()
        {
            base.OnStart();
            ClearColor = Color.Black;
            //Load menu scene
            Core.StartSceneTransition(new FadeTransition(() => new MenuScene()));
        }
    }
}