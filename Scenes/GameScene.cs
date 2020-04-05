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
        public VignettePostProcessor _vignettePostProcessor;
        
        public override void Initialize()
        {
            base.Initialize();
            ClearColor = Color.Black;
            SetDesignResolution(Screen.MonitorWidth,Screen.MonitorHeight,SceneResolutionPolicy.ShowAll);
            Camera.AddComponent(new CameraBounds(new Vector2(-1280, -720),new Vector2(1280, 720)));
            
            
            AddPostProcessor(MyGame.Instance.VignettePostProcessor);
            AddPostProcessor(MyGame.Instance.BloomPostProcessor).Settings = BloomSettings.PresetSettings[6];
        }

        public override void OnStart()
        {
            base.OnStart();
            
            var gridEntity = CreateEntity("grid");
            gridEntity.AddComponent(new SpringGrid(new Rectangle(-1280, -720, 2560, 1440), new Vector2(30))
            {
                GridMinorThickness = 0f,
                GridMajorThickness = 8,
                GridMajorColor = new Color(61,9,107)
            });
            gridEntity.GetComponent<SpringGrid>().RenderLayer = 9999;
            gridEntity.AddComponent<FoodSpawner>();

            var score = CreateEntity("scoreText");
            score.AddComponent<TextComponent>().AddComponent<ScoreDisplay>();
            
            _snake = CreateEntity("SnakeHead");
            _snake.AddComponent(new Snake(_snakeSize, _snake.Position,new Vector2(10, 10)));
            
           
        }
    }
}