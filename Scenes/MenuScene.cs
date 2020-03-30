using Microsoft.Xna.Framework;
using Nez;
using YetAnotherSnake.Components;

namespace YetAnotherSnake.Scenes
{
    public class MenuScene: Scene
    {
        public override void Initialize()
        {
            base.Initialize();

            var text = CreateEntity("MenuText").AddComponent<MenuTest>().AddComponent<TextComponent>();
            text.Text = "Menu test. Press ENTER to begin game";
            text.Entity.Scale *= 5f;
            
            
        }
    }
}