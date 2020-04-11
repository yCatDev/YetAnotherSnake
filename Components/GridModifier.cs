using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Nez;

namespace YetAnotherSnake.Components
{
    //Entity component for controlling grid 
    public class GridModifier : Component
    {
        /// <summary>
        /// Grid
        /// </summary>
        private SpringGrid _grid;
        
        
        public override void OnAddedToEntity()
        {
            _grid = Entity.Scene.FindEntity("grid").GetComponent<SpringGrid>();
        }

        
        
        /// <summary>
        /// Apply impulse force to the grid
        /// </summary>
        /// <param name="radius">Impulse radius</param>
        public void Impulse(float radius)
        {
            var pos = Entity.Position;
            _grid.ApplyDirectedForce(new Vector3(0, 0, 500), new Vector3(pos.X, pos.Y, 0),
                radius);
        }
        
        /// <summary>
        /// Add implosive force to grid
        /// </summary>
        /// <param name="force">Force</param>
        /// <param name="radius">Radius of force area</param>
        public void Implosive(float force, float radius) =>
            _grid.ApplyImplosiveForce(force, Entity.Position.ToVector3(),
                radius);
    }

}