using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Nez;

namespace YetAnotherSnake.Components
{
    public class GridModifier : Component, IUpdatable
    {
        private SpringGrid _grid;

        
        public GridModifier()
        {
       
        }
        
        public override void OnAddedToEntity()
        {
            _grid = Entity.Scene.FindEntity("grid").GetComponent<SpringGrid>();
        }


        void IUpdatable.Update()
        {
            
        }

        public void Impulse(float radius)
        {
            var pos = Entity.Position;
            _grid.ApplyDirectedForce(new Vector3(0, 0, 1000), new Vector3(pos.X, pos.Y, 0),
                radius);
        }
        
        public void Implosive(float force, float radius)
        {
            _grid.ApplyImplosiveForce(force, Entity.Position.ToVector3(),
                radius);
        }
        
    }

}