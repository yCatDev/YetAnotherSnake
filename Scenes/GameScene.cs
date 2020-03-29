using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez;
using Nez.Sprites;
using Nez.Systems;
using YetAnotherSnake.Components;

namespace YetAnotherSnake.Scenes
{
    public class GameScene: Scene
    {

        private Entity _snake;
        private int _snakeSize = 40;
        private VignettePostProcessor _vignettePostProcessor;
        
        public override void Initialize()
        {
            base.Initialize();
            ClearColor = Color.Black;
            
            _snake = CreateEntity("SnakeHead");
            _snake.AddComponent(new Snake(_snakeSize, _snake.Position,new Vector2(10, 10)));
            
            var gridEntity = CreateEntity("grid");
            gridEntity.AddComponent(new SpringGrid(new Rectangle(-1280, -720, 2560, 1440), new Vector2(30))
            {
                GridMinorThickness = 0f,
                GridMajorThickness = 10,
                GridMajorColor = new Color(61,9,107)
            });
            gridEntity.AddComponent(new CameraBounds(new Vector2(-1280, -720),new Vector2(1280, 720)));
            gridEntity.AddComponent(new SnakeBounds(new Vector2(1280, 720),new Vector2(-1280, -720)));
            gridEntity.GetComponent<SpringGrid>().RenderLayer = 9999;
            gridEntity.AddComponent<FoodSpawner>();
            
            _vignettePostProcessor = new VignettePostProcessor(1) {Power = 0.75f};
            AddPostProcessor(_vignettePostProcessor);
            AddPostProcessor(new BloomPostProcessor(3)).Settings = BloomSettings.PresetSettings[6];
        }
    }
}