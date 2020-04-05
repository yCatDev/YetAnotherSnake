using Microsoft.Xna.Framework;
using Nez;

namespace YetAnotherSnake.Components
{
    public class ScoreDisplay: Component, IUpdatable
    {

        private TextComponent _text;
        private Camera _camera;
        private int _score = 0, _hiscore;

        public override void OnAddedToEntity()
        {
            base.OnAddedToEntity();
            _text = Entity.GetComponent<TextComponent>();
            _text.SetText("Score: 0");
            Entity.Scale *= 4;
            _camera = Entity.Scene.Camera;
            _hiscore = MyGame.Instance.SaveSystem.SaveFile.Score;
        }

        public void Update()
        {
            var newPos = new Vector2(_camera.Bounds.Left, _camera.Bounds.Top)*0.95f;
            Entity.Position = Utils.Move(Entity.Position, newPos, 0.5f);
        }

        public void IncScore()
        {
            _score++;
            _text.SetText($"Score: {_score}");
        }

        public void CheckHiScore()
        {
            if (_score > _hiscore)
            {
                MyGame.Instance.SaveSystem.SaveFile.Score = _score;
                MyGame.Instance.SaveSystem.SaveChanges();
            }
        }

    }
}