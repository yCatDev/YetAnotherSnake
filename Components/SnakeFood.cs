﻿using Microsoft.Xna.Framework;
 using Nez;

namespace YetAnotherSnake.Components
{
    /// <summary>
    /// Entity component that describe snake food
    /// </summary>
    public class SnakeFood: Component, IUpdatable
    {
        /// <summary>
        /// Grid modifier for creating implosive force 
        /// </summary>
        private GridModifier _modifier;
        
        /// <summary>
        /// Scale for animating
        /// </summary>
        private Vector2 _oldScale;

        public override void OnAddedToEntity()
        {
            base.OnAddedToEntity();
            _modifier = Entity.GetOrCreateComponent<GridModifier>();
            _oldScale = Entity.Scale;
            Entity.Scale = Vector2.Zero;
        }

        public void Update()
        {
            if (MyGame.GameInstance.Pause)
                return;
            
            if (Entity.Scale!=_oldScale)
                Entity.Scale = Utils.Move(Entity.Scale, _oldScale, 0.05f);
            
            
            Entity.LocalRotation += 0.05f;
            _modifier.Implosive(5,150);
        }
    }
}