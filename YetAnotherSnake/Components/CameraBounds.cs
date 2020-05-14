using Microsoft.Xna.Framework;
using Nez;

namespace YetAnotherSnake.Components
{
    /// <summary>
    /// Component class that keeps Nez camera in desired area
    /// </summary>
    public class CameraBounds : Component, IUpdatable
    {
        
        /// <summary>
        /// Camera min position
        /// </summary>
        private readonly Vector2 _min;
        /// <summary>
        /// Camera max position
        /// </summary>
        private readonly Vector2 _max;

        


        public CameraBounds(Vector2 min, Vector2 max) 
        {
            // updating last so the camera is already moved before we evaluate its position
            SetUpdateOrder(int.MaxValue);
            _min = min;
            _max = max;
        }




        public void Update()
        {
            
            var cameraBounds = Entity.Scene.Camera.Bounds;

            if (cameraBounds.Top < _min.Y)
                Entity.Position += new Vector2(0, _min.Y - cameraBounds.Top);

            if (cameraBounds.Left < _min.X)
                Entity.Position += new Vector2(_min.X - cameraBounds.Left, 0);

            if (cameraBounds.Bottom > _max.Y)
                Entity.Position += new Vector2(0, _max.Y - cameraBounds.Bottom);

            if (cameraBounds.Right > _max.X)
                Entity.Position += new Vector2(_max.X - cameraBounds.Right, 0);
        }

        /// <summary>
        /// Check if given point is out of camera view
        /// </summary>
        /// <param name="target">Point position</param>
        /// <returns>Given point is out of camera view</returns>
        public bool OutOfBounds(Vector2 target) =>
            (target.Y < _min.Y || target.X < _min.X || target.Y > _max.Y || target.X > _max.X);
    }
}