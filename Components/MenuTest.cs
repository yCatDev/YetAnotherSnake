using Microsoft.Xna.Framework.Input;
using Nez;
using YetAnotherSnake.Scenes;

namespace YetAnotherSnake.Components
{
    public class MenuTest: Component, IUpdatable
    {
        private VirtualButton _enterKey;

        public override void OnAddedToEntity()
        {
            base.OnAddedToEntity();
            
            _enterKey = new VirtualButton();
            _enterKey.AddKeyboardKey(Keys.Enter);
        }

        public void Update()
        {
            if (_enterKey.IsPressed)
            {
                Core.StartSceneTransition(new FadeTransition(()=>new GameScene()));
            }
        }
    }
}