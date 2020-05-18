using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nez;
using Nez.Sprites;
using Nez.Textures;

namespace YetAnotherSnake.Components
{
    public class SnakeEyes: SceneComponent, IUpdatable
    {
        private Entity _domain, _pupil1, _pupil2, _parent, _lookTarget, _target;
        private SpriteRenderer _eye1, _ee1, _eye2, _ee2;
        private Texture2D _sprite;
        private float _distance;

        public SnakeEyes(Entity parent, Entity lookTarget, Texture2D sprite)
        {
            
            _parent = parent;
            _sprite = sprite;
            _lookTarget = lookTarget;

        }

        public override void OnEnabled()
        {
            base.OnEnabled();
            _domain = Scene.CreateEntity("EyeDomain");
            _eye1 = Scene.CreateEntity("Eye1").AddComponent(new SpriteRenderer(_sprite));
            _eye1.Transform.Scale = new Vector2(0.25f, 0.25f);
            _eye1.Transform.Position = new Vector2(_eye1.Transform.Position.X, _eye1.Transform.Position.Y+10);
            _ee1 = Scene.CreateEntity("ee1").AddComponent(new SpriteRenderer(_sprite));
            _ee1.Transform.Position = new Vector2(_ee1.Transform.Position.X+10, _ee1.Transform.Position.Y);
            _ee1.Color = Color.Black;
            _ee1.Transform.Scale = new Vector2(0.50f, 0.50f);
            _ee1.Transform.Parent = _eye1.Transform;

            _eye1.Transform.Parent = _domain.Transform;
            
            
            _eye1.RenderLayer = -1;
            _ee1.RenderLayer = -2;

            _eye2 = Scene.CreateEntity("Eye2").AddComponent(new SpriteRenderer(_sprite));
            _eye2.Transform.Scale = new Vector2(0.25f, 0.25f);
            _eye2.Transform.Position = new Vector2(_eye2.Transform.Position.X, _eye2.Transform.Position.Y-10);
            _ee2 = Scene.CreateEntity("ee2").AddComponent(new SpriteRenderer(_sprite));
            _ee2.Transform.Position = new Vector2(_ee2.Transform.Position.X+10, _ee2.Transform.Position.Y);
            _ee2.Color = Color.Black;
            _ee2.Transform.Scale = new Vector2(0.50f, 0.50f);
            _ee2.Transform.Parent = _eye2.Transform;
            
            _eye1.RenderLayer = -1;
            _ee1.RenderLayer = -2;
            
            _eye2.Transform.Parent = _domain.Transform;
            
            _domain.Transform.Parent = _parent.Transform;
        }

        public override void Update()
        {
            _domain.RotationDegrees =
                Utils.LookAt(_domain.Transform, _lookTarget.Transform);
            _eye1.RenderLayer = -1;
            _ee1.RenderLayer = -2;
            _eye2.RenderLayer = -1;
            _ee2.RenderLayer = -2;

            if (_target != null)
            {
                
            }
        }

        public void Remove()
        {
            _domain.Destroy();
        }
        
        public void SetLookTarget(Entity target)
        {
            _target = target;
        }

        public void ResetLookTarget()
        {
            _target = null;
        }
        
        private float GetAngleInDegrees(int cx, int cy)
        {
            // draw a line from the center of the circle
            // to the where the cursor is...
            // If the line points:
            // up = 0 degrees
            // right = 90 degrees
            // down = 180 degrees
            // left = 270 degrees
            var x = Mouse.GetState().X;
            var y = Mouse.GetState().Y;
            
            float angle;
            int dy = y - cy;
            int dx = x - cx;
            if (dx == 0) // Straight up and down | avoid divide by zero error!
            {
                if (dy <= 0)
                {
                    angle = 0;
                }
                else
                {
                    angle = 180;
                }
            }
            else
            {
                angle = (float) Math.Atan((Double)dy / (Double)dx);
                angle =  (float) (angle * ((Double)180 / Math.PI));

                if (x <= cx)
                {
                    angle = 180 + angle;
                }
            }

            return angle;
        }
        
    }
}