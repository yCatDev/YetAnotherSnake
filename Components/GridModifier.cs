using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Nez;

namespace YetAnotherSnake.Components
{
    public class GridModifier : Component, IUpdatable
    {
        SpringGrid _grid;
        Vector2 _lastPosition;


        public override void OnAddedToEntity()
        {
            _grid = Entity.Scene.FindEntity("grid").GetComponent<SpringGrid>();
        }


        void IUpdatable.Update()
        {
            
            var velocity = Entity.Position - _lastPosition;
            
            _grid.ApplyExplosiveForce(0.5f * velocity.Length(), Entity.Position, 120);

            _lastPosition = Entity.Position;
        }

        public void Impulse()
        {
            _grid.ApplyDirectedForce(new Vector3(0, 0, 1000), new Vector3(Entity.Position.X, Entity.Position.Y, 0),
                200);
        }
    }

}