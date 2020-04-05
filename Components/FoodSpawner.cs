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
            else
            {
                _food.LocalRotation += 0.05f;
            }
        }

        private Entity CreateFood()
        {
            var foodSprite = _scene.Content.Load<Texture2D>(YetAnotherSnake.Content.White);
            var food = _scene.CreateEntity("Food").AddComponent<SnakeFood>()
                .AddComponent(new GridModifier()).Entity;
            var foodCollider = food.AddComponent<BoxCollider>();
            foodCollider.Width = 300;
            foodCollider.Height = 300;
            var foodRender = food.AddComponent(new SpriteRenderer(foodSprite));
            foodRender.RenderLayer = 9997;
            food.Scale = new Vector2(0.4f, 0.4f);
            food.Position = new Vector2(Random.Range(-1000,1000),Random.Range(-500,500));
            foodRender.Color = Random.NextColor()*1.5f;
            food.AddComponent(new SpriteOutlineRenderer(foodRender)
            {
                OutlineColor =  new Color(61,9,107),
                OutlineWidth = 10
            });
            
            
            Core.StartCoroutine(Coroutines.SpawnFood(food));
            return food;
        }
        
    }
}