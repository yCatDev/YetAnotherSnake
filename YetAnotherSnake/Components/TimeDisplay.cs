using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Nez;

namespace YetAnotherSnake.Components
{
    public class TimeDisplay: Component, IUpdatable
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
        private Stopwatch _timer = new Stopwatch();
        private long _hitime;
        
        public override void OnAddedToEntity()
        {
            base.OnAddedToEntity();
            
            //Setup values
            _text = Entity.GetComponent<TextComponent>();
            _text.SetFont(Entity.Scene.Content.LoadBitmapFont(Content.DefaultTitleFont));
            _text.SetText("Score: 0");
            Entity.Scale *= 0.75f;
            _camera = Entity.Scene.Camera;
            _hitime = MyGame.GameInstance.SaveSystem.SaveFile.TimeAttackScore;
            _timer.Start();
        }
        
        public void Update()
        {
            var newPos = new Vector2(_camera.Bounds.Left, _camera.Bounds.Top)*0.95f;
            Entity.Position = Utils.Move(Entity.Position, newPos, 0.5f);
            _text.SetText(Format(_timer.ElapsedMilliseconds));

            if (MyGame.GameInstance.Pause)
            {
                _timer.Stop();
                return;
            }
            if (!_timer.IsRunning) _timer.Start();
        }
        
        
        public void CheckHigh()
        {
            if (_timer.ElapsedMilliseconds > _hitime)
            {
                MyGame.GameInstance.SaveSystem.SaveFile.TimeAttackScore = _timer.ElapsedMilliseconds;
                MyGame.GameInstance.SaveSystem.SaveChanges();
            }
        }

        public static string Format(long ms) => TimeSpan.FromMilliseconds(ms).ToString(@"hh\:mm\:ss");

    }
}