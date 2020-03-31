﻿using Nez;

namespace YetAnotherSnake.Components
{
    public class SnakeFood: Component, IUpdatable
    {
        private GridModifier _modifier;

        public override void OnAddedToEntity()
        {
            base.OnAddedToEntity();
            _modifier = Entity.GetOrCreateComponent<GridModifier>();
        }

        public void Update()
        {
            _modifier.Implosive(5,150);   
        }
    }
}