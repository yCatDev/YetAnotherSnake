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

        private Entity _snake;
        private int _snakeSize = 40;
        private VirtualButton pauseKey;
        private Table pauseTable, rootTable, settingsTable;
        private Entity gridEntity;
        
        
        public override void Initialize()
        {
            base.Initialize();
            ClearColor = Color.Black;
            SetDesignResolution(Screen.MonitorWidth,Screen.MonitorHeight,SceneResolutionPolicy.ShowAll);
            Camera.AddComponent(new CameraBounds(new Vector2(-1280, -720),new Vector2(1280, 720)));
            
            pauseKey = new VirtualButton();
            pauseKey.AddKeyboardKey(Keys.Escape).AddKeyboardKey(Keys.Enter);
            
            AddPostProcessor(MyGame.Instance.VignettePostProcessor);
            AddPostProcessor(MyGame.Instance.BloomPostProcessor).Settings = BloomSettings.PresetSettings[6];
        }

        public override void OnStart()
        {
            base.OnStart();
            
            gridEntity  = CreateEntity("grid");
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
            
            AddRenderer(new ScreenSpaceRenderer(100, 9990));

            var ui = CreateEntity("UI").AddComponent<UICanvas>();
            ui.IsFullScreen = false;
            ui.RenderLayer = 9990;

            rootTable = ui.Stage.AddElement(new Table());
            rootTable.SetFillParent( true );
            
            
            pauseTable = rootTable.AddElement(new Table());
            pauseTable.SetFillParent(true);
            settingsTable = rootTable.AddElement(new Table());
            
            
            var title = new Label("PAUSE", MyGame.Instance.Skin.Skin.Get<LabelStyle>("title-label"));
            pauseTable.Add(title);
            pauseTable.Row();
            
            var el = new Container();
            el.SetHeight(200);
            pauseTable.Add(el);
            pauseTable.Row();

            CreateBtn(pauseTable, "Continue", button =>
                {
                    Core.StartCoroutine(Coroutines.MoveToY(rootTable, Screen.MonitorHeight));
                    
                    gridEntity.UpdateInterval = 1;
                    MyGame.Instance.AudioManager.ResumeMusic();
                    MyGame.Instance.Pause = false;
                });
            pauseTable.Row();
            CreateBtn(pauseTable, "Settings", button =>
            {
                Core.StartCoroutine(Coroutines.MoveToY(rootTable, -Screen.MonitorHeight*2));

            });
            pauseTable.Row();
            CreateBtn(pauseTable, "Exit", button =>
            {
                MyGame.Instance.Pause = false;
                
                Core.StartCoroutine(Coroutines.MoveToY(rootTable, Screen.MonitorHeight));
                _snake.GetComponent<Snake>().Die();
                gridEntity.UpdateInterval = 1;
                
            });
            pauseTable.Pack();
            InitSettingsMenu(ref settingsTable);
            
            
            rootTable.Pack();
           
            rootTable.SetPosition(0, 0);
            pauseTable.SetPosition(0, Screen.MonitorHeight);
            settingsTable.SetPosition(0, Screen.MonitorHeight * 2);
            
        }
        
         private void InitSettingsMenu(ref Table table)
        {
            table = rootTable.AddElement(new Table());

            table.SetFillParent(true);

            var title = new Label("Settings", MyGame.Instance.Skin.Skin.Get<LabelStyle>("title-label"));
            title.SetFontScale(0.75f);
            table.Add(title);
            table.Row();
            //
            var info = new Label("Volume", MyGame.Instance.Skin.Skin.Get<LabelStyle>("label"));
            table.Add(info);
            table.Row();
            
            var slider = new Slider(0, 1, 0.05f, false,MyGame.Instance.Skin.Skin.Get<SliderStyle>());
            slider.SetValue(MyGame.Instance.SaveSystem.SaveFile.Volume);
            slider.OnChanged += f =>
            {
                MyGame.Instance.SaveSystem.SaveFile.Volume = f;
                MyGame.Instance.AudioManager.Volume = f;
            }; 
            table.Add(slider);
            table.Row();
            
            var el2 = new Container();
            el2.SetHeight(100);
            table.Add(el2);
            table.Row();


            CreateCheckBox(table, "FullScreen", MyGame.Instance.SaveSystem.SaveFile.IsFullScreen,b =>
            {
                if (b)
                    Screen.SetSize(Screen.MonitorWidth, Screen.MonitorHeight);
                else
                {
                    Screen.SetSize((int) (Screen.MonitorWidth*0.75f), (int) (Screen.MonitorHeight*0.75f));
                }
                MyGame.Instance.SaveSystem.SaveFile.IsFullScreen = b;
                Screen.IsFullscreen = b;
                
            });
            table.Row();
            
            CreateCheckBox(table, "Enable vignette", MyGame.Instance.SaveSystem.SaveFile.IsVignette, b =>
            {
                MyGame.Instance.SaveSystem.SaveFile.IsVignette = b;
                MyGame.Instance.VignettePostProcessor.Enabled = b;
            });
            table.Row();
            
            CreateCheckBox(table, "Enable bloom", MyGame.Instance.SaveSystem.SaveFile.IsBloom, b =>
            {
                MyGame.Instance.SaveSystem.SaveFile.IsBloom = b;
                MyGame.Instance.BloomPostProcessor.Enabled = b;
            });
            table.Row();
            
            
            var el = new Container();
            el.SetHeight(100);
            table.Add(el);
            table.Row();
            
            CreateBtn(table, "Apply", button =>
            {
                MyGame.Instance.SaveSystem.SaveChanges();
                Core.StartCoroutine(Coroutines.MoveToY(rootTable, -Screen.MonitorHeight));
            });
            table.Row();
            CreateBtn(table, "Back", button =>
            {
                Core.StartCoroutine(Coroutines.MoveToY(rootTable, -Screen.MonitorHeight));
            });
            table.Row();
            table.Pack();
        }
        
         private CheckBox CreateCheckBox(Table t, string label, bool defaultState, Action<bool> action)
         {
             var checkBox = new CheckBox(label, Skin.CreateDefaultSkin());
             checkBox.GetLabel().GetStyle().Font = MyGame.Instance.Skin.Skin.Get<LabelStyle>("label").Font;
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
            button.GetLabel().SetStyle(MyGame.Instance.Skin.Skin.Get<LabelStyle>("label"));
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
            if (pauseKey.IsPressed && _snake.GetComponent<Snake>().IsAlive)
            {
                Core.StartCoroutine(Coroutines.MoveToY(rootTable, -Screen.MonitorHeight));
                MyGame.Instance.Pause = true;
                gridEntity.UpdateInterval = 9999;
                MyGame.Instance.AudioManager.PauseMusic();
            }

        
        }
    }
    
}