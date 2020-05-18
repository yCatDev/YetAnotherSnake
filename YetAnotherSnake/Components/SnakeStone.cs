using System.Diagnostics;
using Microsoft.Xna.Framework;
using Nez;

namespace YetAnotherSnake.Components
{
    public class SnakeStone: Component, IUpdatable
    {
        /// <summary>
        /// Grid modifier for creating implosive force 
        /// </summary>
        private GridModifier _modifier;
        
        /// <summary>
        /// Scale for animating
        /// </summary>
        private Vector2 _oldScale;

        private bool _timeup;
        private Stopwatch _timer = new Stopwatch();
        private float _to;

        public override void OnAddedToEntity()
        {
            base.OnAddedToEntity();
            _modifier = Entity.GetOrCreateComponent<GridModifier>();
            _oldScale = Entity.Scale;
            Entity.Scale = Vector2.Zero;
            Entity.LocalRotation += Random.Range(0,360);
            _timer.Start();
            _to = Random.Range(5, 16) * 1000;
        }

        public void Update()
        {
            if (MyGame.GameInstance.Pause)
            {
                _timer.Stop();
                return;
            }
            if (!_timer.IsRunning) _timer.Start();

            if (!_timeup && Entity.Scale!=_oldScale)
                Entity.Scale = Utils.Move(Entity.Scale, _oldScale, 0.05f);

            if (_timer.ElapsedMilliseconds > _to)
                _timeup = true;
            
            if (_timeup && Vector2.Distance(Entity.Scale, Vector2.Zero)>0.1f)
                Entity.Scale = Utils.Move(Entity.Scale, Vector2.Zero, 0.05f);
            else if (_timeup) Entity.Destroy();
            
            _modifier.Implosive(5,150);
        }
    }
}