using System.Threading;
using Microsoft.Xna.Framework;
using Nez;
using Timer = System.Timers.Timer;

namespace YetAnotherSnake.Components
{
    public class MenuGrid: Component
    {
        private SpringGrid _grid;
        private Timer _timer;


        public MenuGrid(float interval)
        {
            _timer =new Timer()
            {
                AutoReset = true,
                Interval = interval,
            };
            _timer.Elapsed += (sender, args) =>
            {
                Vector2 pos = new Vector2(Random.Range(0, Screen.Width),Random.Range(0, Screen.Height));
                _grid.ApplyDirectedForce(new Vector3(0, 0, 500), new Vector3(pos.X, pos.Y, 0),
                    Random.Range(300,500));
            };
        }
        
        public override void OnAddedToEntity()
        {
            base.OnAddedToEntity();
            _grid = Entity.Scene.FindEntity("grid").GetComponent<SpringGrid>();
            _timer.Start();
        }
        
        
    }
}