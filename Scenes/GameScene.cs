using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez;
using Nez.Sprites;
using YetAnotherSnake.Components;

namespace YetAnotherSnake.Scenes
{
    public class GameScene: Scene
    {

        private Entity _snake;
        private int _snakeSize = 80;
        
        
        public override void Initialize()
        {
            base.Initialize();
            ClearColor = Color.Black;

            var gridEntity = CreateEntity("grid");
            gridEntity.AddComponent(new SpringGrid(new Rectangle(100, 100, 1280, 720), new Vector2(30))
            {
                GridMinorThickness = 0f,
                GridMajorThickness = 3,
                GridMajorColor = Color.Purple
            });
            
            _snake = CreateEntity("SnakeHead");
            _snake.AddComponent(new Snake(_snakeSize, _snake.Position,new Vector2(10, 10)));
            
            
            AddPostProcessor(new VignettePostProcessor(1){Power = 0.55f});
            AddPostProcessor(new BloomPostProcessor(3)).Settings = BloomSettings.PresetSettings[0];
            
        }
    }
}