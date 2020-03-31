using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez;
using Nez.Sprites;

namespace YetAnotherSnake.Components
{
    public class FoodSpawner: Component, IUpdatable
    {
        private Scene _scene;
        private Entity _food;

        public override void OnAddedToEntity()
        {
            base.OnAddedToEntity();
            _scene = Entity.Scene;
        }

        public void Update()
        {
            if (!_scene.Entities.Contains(_food))
                _food = CreateFood();
        }

        private Entity CreateFood()
        {
            var foodSprite = _scene.Content.Load<Texture2D>(YetAnotherSnake.Content.Blank);
            var food = _scene.CreateEntity("Food").AddComponent<SnakeFood>()
                .AddComponent(new GridModifier()).Entity;
            var foodCollider = food.AddComponent<BoxCollider>();
            foodCollider.Width = 400;
            foodCollider.Height = 400;
            var foodRender = food.AddComponent(new SpriteRenderer(foodSprite));
            foodRender.RenderLayer = 9998;
            food.Scale = new Vector2(0.25f, 0.25f);
            food.Position = new Vector2(Random.Range(-1000,1000),Random.Range(-500,500));
            foodRender.Color.A = 0;
            Core.StartCoroutine(Coroutines.SpawnFood(food));
            return food;
        }
        
    }
}