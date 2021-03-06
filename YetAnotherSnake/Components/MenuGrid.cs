﻿using System.Threading;
using Microsoft.Xna.Framework;
using Nez;
using Timer = System.Timers.Timer;

namespace YetAnotherSnake.Components
{
    /// <summary>
    /// Entity component that creates random grid impulses
    /// </summary>
    public class MenuGrid: Component
    {
        /// <summary>
        /// Grid
        /// </summary>
        private SpringGrid _grid;
        /// <summary>
        /// Timer
        /// </summary>
        private Timer _timer;


        public override void OnAddedToEntity()
        {
            base.OnAddedToEntity();
            _grid = Entity.Scene.FindEntity("grid").GetComponent<SpringGrid>();
            _timer.Start();
        }
        
        /// <summary>
        /// Init component and sets impulse interval
        /// </summary>
        /// <param name="interval">Interval</param>
        public MenuGrid(float interval)
        {
            _timer =new Timer()
            {
                AutoReset = true,
                Interval = interval,
            };
            
            //Create handler on timer tick, that create random impulse on grid
            _timer.Elapsed += (sender, args) =>
            {
                Vector2 pos = new Vector2(Random.Range(0, Screen.Width),Random.Range(0, Screen.Height));
                _grid.ApplyDirectedForce(new Vector3(0, 0, 500), new Vector3(pos.X, pos.Y, 0),
                    Random.Range(300,500));
            };
        }
        
       
        
        
    }
}