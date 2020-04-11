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
            gridEntity.AddComponent<FoodSpawner>();

            //Create text label for displaying score
            var score = CreateEntity("scoreText");
            score.AddComponent<TextComponent>().AddComponent<ScoreDisplay>();
            
            
            //Create main snake component
            _snake = CreateEntity("SnakeHead");
            _snakeController = AddSceneComponent(new Snake(SnakeSize, _snake.Position,new Vector2(10, 10)));
            
            AddRenderer(new ScreenSpaceRenderer(100, 9990));

            var ui = CreateEntity("UI").AddComponent<UICanvas>();
            ui.IsFullScreen = false;
            ui.RenderLayer = 9990;

            _rootTable = ui.Stage.AddElement(new Table());
            _rootTable.SetFillParent( true );
            
            
            _pauseTable = _rootTable.AddElement(new Table());
            _pauseTable.SetFillParent(true);
            _settingsTable = _rootTable.AddElement(new Table());
            
            
            var title = new Label("PAUSE", MyGame.GameInstance.Skin.Skin.Get<LabelStyle>("title-label"));
            _pauseTable.Add(title);
            _pauseTable.Row();
            
            var el = new Container();
            el.SetHeight(200);
            _pauseTable.Add(el);
            _pauseTable.Row();

            CreateBtn(_pauseTable, "Continue", button =>
                {
                    Core.StartCoroutine(Coroutines.MoveToY(_rootTable, Screen.MonitorHeight));
                    
                    gridEntity.UpdateInterval = 1;
                    MyGame.GameInstance.AudioManager.ResumeMusic();
                    MyGame.GameInstance.Pause = false;
                });
            _pauseTable.Row();
            CreateBtn(_pauseTable, "Settings", button =>
            {
                Core.StartCoroutine(Coroutines.MoveToY(_rootTable, -Screen.MonitorHeight*2));

            });
            _pauseTable.Row();
            CreateBtn(_pauseTable, "Exit", button =>
            {
                MyGame.GameInstance.Pause = false;
                
                Core.StartCoroutine(Coroutines.MoveToY(_rootTable, Screen.MonitorHeight));
                _snakeController.Die();
                gridEntity.UpdateInterval = 1;
                
            });
            _pauseTable.Pack();
            InitSettingsMenu(ref _settingsTable);
            
            
            _rootTable.Pack();
           
            _rootTable.SetPosition(0, 0);
            _pauseTable.SetPosition(0, Screen.MonitorHeight);
            _settingsTable.SetPosition(0, Screen.MonitorHeight * 2);
            
        }
        
         private void InitSettingsMenu(ref Table table)
        {
            table = _rootTable.AddElement(new Table());

            table.SetFillParent(true);

            var title = new Label("Settings", MyGame.GameInstance.Skin.Skin.Get<LabelStyle>("title-label"));
            title.SetFontScale(0.75f);
            table.Add(title);
            table.Row();
            //
            var info = new Label("Volume", MyGame.GameInstance.Skin.Skin.Get<LabelStyle>("label"));
            table.Add(info);
            table.Row();
            
            var slider = new Slider(0, 1, 0.05f, false,MyGame.GameInstance.Skin.Skin.Get<SliderStyle>());
            slider.SetValue(MyGame.GameInstance.SaveSystem.SaveFile.Volume);
            slider.OnChanged += f =>
            {
                MyGame.GameInstance.SaveSystem.SaveFile.Volume = f;
                MyGame.GameInstance.AudioManager.Volume = f;
            }; 
            table.Add(slider);
            table.Row();
            
            var el2 = new Container();
            el2.SetHeight(100);
            table.Add(el2);
            table.Row();


            CreateCheckBox(table, "FullScreen", MyGame.GameInstance.SaveSystem.SaveFile.IsFullScreen,b =>
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
            
            CreateCheckBox(table, "Enable vignette", MyGame.GameInstance.SaveSystem.SaveFile.IsVignette, b =>
            {
                MyGame.GameInstance.SaveSystem.SaveFile.IsVignette = b;
                MyGame.GameInstance.VignettePostProcessor.Enabled = b;
            });
            table.Row();
            
            CreateCheckBox(table, "Enable bloom", MyGame.GameInstance.SaveSystem.SaveFile.IsBloom, b =>
            {
                MyGame.GameInstance.SaveSystem.SaveFile.IsBloom = b;
                MyGame.GameInstance.BloomPostProcessor.Enabled = b;
            });
            table.Row();
            
            
            var el = new Container();
            el.SetHeight(100);
            table.Add(el);
            table.Row();
            
            CreateBtn(table, "Apply", button =>
            {
                MyGame.GameInstance.SaveSystem.SaveChanges();
                Core.StartCoroutine(Coroutines.MoveToY(_rootTable, -Screen.MonitorHeight));
            });
            table.Row();
            CreateBtn(table, "Back", button =>
            {
                Core.StartCoroutine(Coroutines.MoveToY(_rootTable, -Screen.MonitorHeight));
            });
            table.Row();
            table.Pack();
        }
        
         private CheckBox CreateCheckBox(Table t, string label, bool defaultState, Action<bool> action)
         {
             var checkBox = new CheckBox(label, Skin.CreateDefaultSkin());
             checkBox.GetLabel().GetStyle().Font = MyGame.GameInstance.Skin.Skin.Get<LabelStyle>("label").Font;
             checkBox.GetLabel().SetFontScale(0.75f);
             checkBox.IsChecked = defaultState;
             checkBox.OnChanged += action;
             action.Invoke(defaultState);
             t.Add(checkBox);

             return checkBox;
         }
         
        private TextButton CreateBtn(Table t, string label, Action<Button> action)
        {
            var button = new TextButton(label,TextButtonStyle.Create( Color.Black, new Color(61,9,85), new Color(61,9,107) ) );
            button.GetLabel().SetStyle(MyGame.GameInstance.Skin.Skin.Get<LabelStyle>("label"));
            button.OnClicked += btn =>
            {
                btn.ResetMouseHover();
                action(btn);
            };
            t.Add( button ).SetMinWidth( 400 ).SetMinHeight( 100 );
            
            return button;
        }

        public override void Update()
        {
            base.Update();
            if (pauseKey.IsPressed && _snakeController.IsAlive)
            {
                Core.StartCoroutine(Coroutines.MoveToY(_rootTable, -Screen.MonitorHeight));
                MyGame.GameInstance.Pause = true;
                gridEntity.UpdateInterval = 9999;
                MyGame.GameInstance.AudioManager.PauseMusic();
            }

        
        }
    }
    
}