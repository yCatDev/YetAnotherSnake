using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez;
using Nez.Sprites;

namespace YetAnotherSnake.Components
{
    /// <summary>
    /// Scene component that spawn food for snake
    /// </summary>
    public class FoodSpawner: SceneComponent, IUpdatable
    {
        /// <summary>
        /// Food object
        /// </summary>
        private Entity _food;

        /// <summary>
        /// Food texture
        /// </summary>
        private Texture2D _foodSprite;

        public override void OnEnabled()
        {
            base.OnEnabled();
            
            //Loading texture for food
            _foodSprite = Scene.Content.Load<Texture2D>(YetAnotherSnake.Content.White);
        }

        public override void Update()
        {
            if (MyGame.GameInstance.Pause)
                return;
            
            //Check food on scene
            //if (!Scene.Entities.Contains(_food))
            //    _food = CreateFood();
        }

        /// <summary>
        /// Spawn food
        /// </summary>
        /// <returns>Food entity</returns>
        public Entity CreateFood()
        {
            var food = Scene.CreateEntity("Food").AddComponent<SnakeFood>()
                .AddComponent(new GridModifier()).Entity;
            var foodCollider = food.AddComponent<BoxCollider>();
            foodCollider.Width = 300;
            foodCollider.Height = 300;
         
            //Create render
            var foodRender = food.AddComponent(new SpriteRenderer(_foodSprite));
            food.Scale = new Vector2(0.4f, 0.4f);
            food.Position = new Vector2(Random.Range(-1000,1000),Random.Range(-500,500));
            foodRender.Color = Random.NextColor()*1.5f;
            
            //Create outline
            food.AddComponent(new SpriteOutlineRenderer(foodRender)
            {
                OutlineColor =  new Color(61,9,107),
                OutlineWidth = 10
            }).RenderLayer = 9999;
            
            
            return food;
        }
        
        /// <summary>
        /// Spawn food
        /// </summary>
        /// <returns>Food entity</returns>
        public Entity CreateFood(Vector2 position)
        {
            var food = Scene.CreateEntity("Food").AddComponent<SnakeFood>()
                .AddComponent(new GridModifier()).Entity;
            var foodCollider = food.AddComponent<BoxCollider>();
            foodCollider.Width = 300;
            foodCollider.Height = 300;
         
            //Create render
            var foodRender = food.AddComponent(new SpriteRenderer(_foodSprite));
            food.Scale = new Vector2(0.4f, 0.4f);
            food.Position = position;
            foodRender.Color = Random.NextColor()*1.5f;
            
            //Create outline
            food.AddComponent(new SpriteOutlineRenderer(foodRender)
            {
                OutlineColor =  new Color(61,9,107),
                OutlineWidth = 10
            }).RenderLayer = 9999;
            
            
            return food;
        }
        
    }
}