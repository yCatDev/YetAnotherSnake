using Microsoft.Xna.Framework;
using Nez;

namespace YetAnotherSnake.Components
{
    public class ScoreDisplay: Component, IUpdatable
    {

        private TextComponent _text;
        private Camera _camera;
        private int _score = 0;

        public override void OnAddedToEntity()
        {
            base.OnAddedToEntity();
            _text = Entity.GetComponent<TextComponent>();
            _text.SetText("Score: 0");
            Entity.Scale *= 4;
            _camera = Entity.Scene.Camera;
        }

        public void Update()
        {
            Entity.Position = new Vector2(_camera.Bounds.Left, _camera.Bounds.Top);
        }

        public void IncScore()
        {
            _score++;
            _text.SetText($"Score: {_score}");
        }
        
    }
}