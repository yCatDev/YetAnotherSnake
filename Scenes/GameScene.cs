using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nez;
using Nez.Sprites;
using Nez.Systems;
using Nez.UI;
using YetAnotherSnake.Components;

namespace YetAnotherSnake.Scenes
{
    public class GameScene: Scene
    {
        /// <summary>
        /// Default snake length 
        /// </summary>
        private const int SnakeSize = 40;
        
        /// <summary>
        /// Main snake object
        /// </summary>
        private Entity _snake;
        /// <summary>
        /// Background grid object
        /// </summary>
        private Entity gridEntity;

        private Snake _snakeController;
        
        /// <summary>
        /// Pause key listener
        /// </summary>
        private VirtualButton pauseKey;
        
        /// <summary>
        /// UI tables
        /// </summary>
        private Table _pauseTable, _rootTable, _settingsTable;

        private GameUIHelper _uiHelper;
        
        
        public override void Initialize()
        {
            base.Initialize();
            ClearColor = Color.Black;
            SetDesignResolution(Screen.MonitorWidth,Screen.MonitorHeight,SceneResolutionPolicy.ShowAll);
            Camera.AddComponent(new CameraBounds(new Vector2(-1280, -720),new Vector2(1280, 720)));
            
            //Set up listener for Escape and Enter key
            pauseKey = new VirtualButton();
            pauseKey.AddKeyboardKey(Keys.Escape).AddKeyboardKey(Keys.Enter);
            
            //Enabling post-processing
            AddPostProcessor(MyGame.GameInstance.VignettePostProcessor);
            AddPostProcessor(MyGame.GameInstance.BloomPostProcessor).Settings = BloomSettings.PresetSettings[6];
        }

        public override void OnStart()
        {
            base.OnStart();
            
            //Creating background grid
            gridEntity  = CreateEntity("grid");
            gridEntity.AddComponent(new SpringGrid(new Rectangle(-1280, -720, 2560, 1440), new Vector2(30))
            {
                GridMinorThickness = 0f,
                GridMajorThickness = 8,
                GridMajorColor = new Color(61,9,107)
            });
            gridEntity.GetComponent<SpringGrid>().RenderLayer = 9999;
            AddSceneComponent<FoodSpawner>();

            //Create text label for displaying score
            var score = CreateEntity("scoreText");
            score.AddComponent<TextComponent>().AddComponent<ScoreDisplay>();
            
            
            //Create main snake component
            _snake = CreateEntity("SnakeHead");
            _snakeController = AddSceneComponent(new Snake(SnakeSize, _snake.Position,new Vector2(10, 10)));
            
           CreateUI();

            
            
        }

        private void CreateUI()
        {
            //Fix update layer for active UI elements (Nez bug fix)
            AddRenderer(new ScreenSpaceRenderer(100, 9990));
            
            var ui = CreateEntity("UI").AddComponent<UICanvas>();
            ui.IsFullScreen = false;
            ui.RenderLayer = 9990;
            _uiHelper = new GameUIHelper(Content);
            
            _rootTable = ui.Stage.AddElement(new Table());
            _rootTable.SetFillParent( true );
                
            
            _pauseTable = InitPauseMenu();
            
            _settingsTable = InitSettingsMenu();
            
            
            _rootTable.Pack();
           
            _rootTable.SetPosition(0, 0);
            _pauseTable.SetPosition(0, Screen.MonitorHeight);
            _settingsTable.SetPosition(0, Screen.MonitorHeight * 2);
        }
        
        private Table InitPauseMenu()
        {
    
            var table = _rootTable.AddElement(new Table());
            table.SetFillParent(true);
            
            _uiHelper.CreateTitleLabel(table, "PAUSE");
            table.Row();

            _uiHelper.CreateVerticalIndent(table, 200);
            table.Row();

           
            _uiHelper.CreateBtn(table, "Continue", button =>
            {
                Core.StartCoroutine(UIAnimations.MoveToY(_rootTable, Screen.MonitorHeight));
                    
                gridEntity.UpdateInterval = 1;
                MyGame.GameInstance.AudioManager.ResumeMusic();
                MyGame.GameInstance.Pause = false;
            });
            table.Row();
            _uiHelper.CreateBtn(table, "Settings", button =>
            {
                Core.StartCoroutine(UIAnimations.MoveToY(_rootTable, -Screen.MonitorHeight*2));

            });
            table.Row();
            _uiHelper.CreateBtn(table, "Exit", button =>
            {
                MyGame.GameInstance.Pause = false;
                
                Core.StartCoroutine(UIAnimations.MoveToY(_rootTable, Screen.MonitorHeight));
                _snakeController.Die();
                gridEntity.UpdateInterval = 1;
                
            });
            table.Pack();

            return table;
        }
        
         private Table InitSettingsMenu()
        {
            var table = _rootTable.AddElement(new Table());

            table.SetFillParent(true);

            _uiHelper.CreateTitleLabel(table, "Settings").SetFontScale(0.75f);
            table.Row();


            _uiHelper.CreateRegularLabel(table, "Volume");
            table.Row();

            _uiHelper.CreateSlider(table, f =>
            {
                MyGame.GameInstance.SaveSystem.SaveFile.Volume = f;
                MyGame.GameInstance.AudioManager.Volume = f;
            });
            table.Row();

            _uiHelper.CreateVerticalIndent(table, 100);
            table.Row();
            
            _uiHelper.CreateCheckBox(table, "FullScreen", MyGame.GameInstance.SaveSystem.SaveFile.IsFullScreen,b =>
            {
                if (b)
                    Screen.SetSize(Screen.MonitorWidth, Screen.MonitorHeight);
                else
                {
                    Screen.SetSize((int) (Screen.MonitorWidth*0.75f), (int) (Screen.MonitorHeight*0.75f));
                }
                MyGame.GameInstance.SaveSystem.SaveFile.IsFullScreen = b;
                Screen.IsFullscreen = b;
                
            });
            table.Row();
            
            _uiHelper.CreateCheckBox(table, "Enable vignette", MyGame.GameInstance.SaveSystem.SaveFile.IsVignette, b =>
            {
                MyGame.GameInstance.SaveSystem.SaveFile.IsVignette = b;
                MyGame.GameInstance.VignettePostProcessor.Enabled = b;
            });
            table.Row();
            
            _uiHelper.CreateCheckBox(table, "Enable bloom", MyGame.GameInstance.SaveSystem.SaveFile.IsBloom, b =>
            {
                MyGame.GameInstance.SaveSystem.SaveFile.IsBloom = b;
                MyGame.GameInstance.BloomPostProcessor.Enabled = b;
            });
            table.Row();


            _uiHelper.CreateVerticalIndent(table, 100);
            table.Row();
            
            _uiHelper.CreateBtn(table, "Apply", button =>
            {
                MyGame.GameInstance.SaveSystem.SaveChanges();
                Core.StartCoroutine(UIAnimations.MoveToY(_rootTable, -Screen.MonitorHeight));
                
            });
            table.Row();
            _uiHelper.CreateBtn(table, "Back", button =>
            {
                Core.StartCoroutine(UIAnimations.MoveToY(_rootTable, -Screen.MonitorHeight));
            });
            table.Row();
            table.Pack();

            return table;
        }
         

        public override void Update()
        {
            base.Update();
            if (pauseKey.IsPressed && _snakeController.IsAlive)
            {
                Core.StartCoroutine(UIAnimations.MoveToY(_rootTable, -Screen.MonitorHeight));
                MyGame.GameInstance.Pause = true;
                gridEntity.UpdateInterval = 9999;
                MyGame.GameInstance.AudioManager.PauseMusic();
            }

        
        }
    }
    
}