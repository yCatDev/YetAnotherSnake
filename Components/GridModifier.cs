using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Nez;

namespace YetAnotherSnake.Components
{
    public class GridModifier : Component, IUpdatable
    {
        private SpringGrid _grid;
        private Vector2 _lastPosition;
        private float _forceRadius;
        private bool _dynamic;
        
        public GridModifier(float forceRadius = 125f, bool dynamic = true)
        {
            _forceRadius = forceRadius;
            _dynamic = dynamic;
        }
        
        public override void OnAddedToEntity()
        {
            _grid = Entity.Scene.FindEntity("grid").GetComponent<SpringGrid>();
        }


        void IUpdatable.Update()
        {

            if (_dynamic)
            {
                var velocity = Entity.Position - _lastPosition;
                _grid.ApplyExplosiveForce(0.55f * velocity.Length(), Entity.Position, _forceRadius);
                _lastPosition = Entity.Position;
            }
            else
            {
                _grid.ApplyImplosiveForce(5, Entity.Position.ToVector3(),
                    _forceRadius);
            }
        }

        public void Impulse(Vector2 pos, float radius)
        {
            _grid.ApplyDirectedForce(new Vector3(0, 0, 1000), new Vector3(pos.X, pos.Y, 0),
                radius);
        }
    }

}