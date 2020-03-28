using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nez;
using Nez.Sprites;

namespace YetAnotherSnake.Components
{
    public class Snake: Component, IUpdatable
    {
        
        private float _step;
        private VirtualButton _leftArrow, _rightArrow, _space;
        private List<Entity> _snakeParts;
        private int _startSnakeSize;
        private Scene _scene;
        private Entity _marker, _snakeHead;
        private Texture2D _bodySprite;
        private Vector2 _startDirection;
        private BoxCollider _snakeHeadCollider;
        
        public Snake(int startSnakeSize, Vector2 startPosition, Vector2 startDirection, float step = 0.05f)
        {

            _startDirection = startDirection;
            _step = step;
            
            _leftArrow = new VirtualButton();
            _leftArrow.AddKeyboardKey(Keys.Left);
            
            _rightArrow = new VirtualButton();
            _rightArrow.AddKeyboardKey(Keys.Right);
            _startSnakeSize = startSnakeSize;
            _snakeParts = new List<Entity>();
            
            _space = new VirtualButton();
            _space.AddKeyboardKey(Keys.Space);

        }

        public override void Initialize()
        {
            base.Initialize();
            _scene = Entity.Scene;
            _bodySprite = _scene.Content.Load<Texture2D>(Content.White);
            _marker = _scene.CreateEntity("marker",  Entity.Position+(_startDirection));


            for (int i = 0; i < _startSnakeSize; i++)
            {
                var e =_scene.CreateEntity($"Snake{_snakeParts.Count}", 
                    new Vector2(Entity.Position.X, Entity.Position.Y+(_bodySprite.Height/2)));

                e.AddComponent(new SpriteRenderer(_bodySprite));
                e.AddComponent<GridModifier>();
                
            
                
                if (i > 10)
                    e.AddComponent<BoxCollider>();
                _snakeParts.Add(e);
            }

            _snakeHead = _snakeParts[0];
            _snakeHeadCollider = _snakeHead.AddComponent<BoxCollider>();
            _snakeHeadCollider.Width -= 5;
            _snakeHeadCollider.Height -= 5;
            
            _marker.Parent = _snakeHead.Transform;
            _scene.Camera.Entity.AddComponent(new FollowCamera(_snakeHead));
        }

        public void Update()
        {
            _snakeHead.Position = Utils.Move(_snakeHead.Position, _marker.Position, _step*5);
            if (_leftArrow.IsDown)
                _marker.Position = Utils.RotateAboutOrigin(_marker.Position, _marker.Parent.Position, -0.07f);
            if (_rightArrow.IsDown)
                _marker.Position = Utils.RotateAboutOrigin(_marker.Position, _marker.Parent.Position, 0.07f);
            _snakeHead.Position = Utils.Move(_snakeHead.Position, _marker.Position, _step);

            for (int i = _snakeParts.Count - 1; i > 0; i--)
            {
                _snakeParts[i].Position = Utils.Move(_snakeParts[i].Position,_snakeParts[i - 1].Position, _step*5);

                _snakeParts[i].LocalRotationDegrees = Utils.LookAt(_snakeParts[i].Transform, _snakeParts[i - 1].Transform);
            }
            if (_snakeHeadCollider.CollidesWithAny(out CollisionResult result))
                if (result.Collider.Entity.Name.Contains("Snake"))
                {
                    _snakeHead.GetComponent<GridModifier>().Impulse();
                    //result.Collider.Entity.GetComponent<SpriteRenderer>().Color = Color.Red;
                }

            if (_space.IsPressed)
            {
                AddPart();AddPart();AddPart();
            }
        }

        public void AddPart()
        {
            int last = _snakeParts.Count;
            var e = _scene.CreateEntity($"Snake{last}", 
                new Vector2(_snakeParts[last-1].Position.X, _snakeParts[last-1].Position.Y+(_bodySprite.Height/2)));

            e.AddComponent(new SpriteRenderer(_bodySprite));
            e.AddComponent<GridModifier>();
            
            e.AddComponent<BoxCollider>();
            _snakeParts.Add(e);
        }
        
        private Vector2 MakeStep(Vector2 from, float step)
        {
            if (from.X > 0)
                from.X+=step;
            else
                from.X-=step;
            if (from.Y > 0)
                from.Y+=step;
            else
                from.Y-=step;
            return from;
        }
    }
}