using System;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Sprites;

namespace YetAnotherSnake.Components
{
    public class SnakeBounds: Component, IUpdatable
    {
        private Snake _snake;
        public Vector2 Min, Max;

        public SnakeBounds()
        {
            SetUpdateOrder(int.MaxValue);
        }
        
        public SnakeBounds(Vector2 min, Vector2 max) : this()
        {
            Min = min;
            Max = max;
        }
        
        
        public override void OnAddedToEntity()
        {
            base.OnAddedToEntity();
            _snake = Entity.Scene.FindEntity("SnakeHead").GetComponent<Snake>();
        }

        public void Update()
        {
            var snakeHeadPosition = _snake.SnakeHead.Position;
            if (snakeHeadPosition.Y > Min.Y || snakeHeadPosition.X > Min.X || snakeHeadPosition.Y < Max.Y || snakeHeadPosition.X < Max.X)
            {
                _snake.Die();
                Entity.RemoveComponent<SnakeBounds>();
            }
        }
    }
}