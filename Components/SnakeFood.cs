﻿using Microsoft.Xna.Framework;
 using Nez;

namespace YetAnotherSnake.Components
{
    public class SnakeFood: Component, IUpdatable
    {
        private GridModifier _modifier;
        private Vector2 oldScale;

        public override void OnAddedToEntity()
        {
            base.OnAddedToEntity();
            _modifier = Entity.GetOrCreateComponent<GridModifier>();
            oldScale = Entity.Scale;
            Entity.Scale = Vector2.Zero;
        }

        public void Update()
        {
            if (MyGame.Instance.Pause)
                return;
            
            if (Entity.Scale!=oldScale)
            {
                Entity.Scale = Utils.Move(Entity.Scale, oldScale, 0.05f);
            }
            
            Entity.LocalRotation += 0.05f;
            _modifier.Implosive(5,150);
        }
    }
}