using Microsoft.Xna.Framework;
using Nez;
using Nez.BitmapFonts;

namespace YetAnotherSnake.Components
{
    /// <summary>
    /// Entity component that displaying score on text label
    /// </summary>
    public class ScoreDisplay: Component, IUpdatable
    {
        /// <summary>
        /// Text component for setting text
        /// </summary>
        private TextComponent _text;
        /// <summary>
        /// Scene camera
        /// </summary>
        private Camera _camera;
        /// <summary>
        /// Score
        /// </summary>
        private int _score = 0, _hiscore;

        public override void OnAddedToEntity()
        {
            base.OnAddedToEntity();
            
            //Setup values
            _text = Entity.GetComponent<TextComponent>();
            _text.SetFont(Entity.Scene.Content.LoadBitmapFont(Content.DefaultTitleFont));
            _text.SetText("Score: 0");
            Entity.Scale *= 0.75f;
            _camera = Entity.Scene.Camera;
            _hiscore = MyGame.GameInstance.SaveSystem.SaveFile.Score;
        }

        public void Update()
        {
            //Updating score position on screen
            var newPos = new Vector2(_camera.Bounds.Left, _camera.Bounds.Top)*0.95f;
            Entity.Position = Utils.Move(Entity.Position, newPos, 0.5f);
        }

        /// <summary>
        /// Increment score 
        /// </summary>
        public void IncScore()
        {
            _score++;
            _text.SetText($"Score: {_score}");
        }

        /// <summary>
        /// Check and save high score
        /// </summary>
        public void CheckHiScore()
        {
            if (_score > _hiscore)
            {
                MyGame.GameInstance.SaveSystem.SaveFile.Score = _score;
                MyGame.GameInstance.SaveSystem.SaveChanges();
            }
        }

    }
}