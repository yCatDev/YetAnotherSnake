using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez;
using Nez.BitmapFonts;
using Nez.ImGuiTools;
using Nez.UI;
using YetAnotherSnake.Components;

namespace YetAnotherSnake.Scenes
{
    public class MenuScene: Scene
    {
        [Inspectable]
        private Table _rootTable, _tableMain, _tableSettings, _tableHowToPlay;
        
        public override void Initialize()
        {
            base.Initialize();
            ClearColor = Color.Black;
            
            SetDesignResolution(Screen.MonitorWidth,Screen.MonitorHeight,SceneResolutionPolicy.ShowAll);
            
            /*var text = CreateEntity("MenuText").AddComponent<MenuTest>().AddComponent<TextComponent>();
            text.SetFont(new NezSpriteFont(Content.Load<SpriteFont>(YetAnotherSnake.Content.OswaldFont)));
            text.Text = "Yet another snake";*/

            var gridEntity = CreateEntity("grid");
            gridEntity.AddComponent(new SpringGrid(new Rectangle(0, 0, 2560, 1440), new Vector2(30))
            {
                GridMinorThickness = 0f,
                GridMajorThickness = 8,
                GridMajorColor = new Color(61,9,107)
            });
            gridEntity.AddComponent(new MenuGrid(3500));
            gridEntity.GetComponent<SpringGrid>().RenderLayer = 9999;
            
            var ui = CreateEntity("UI").AddComponent<UICanvas>();
            _rootTable = ui.Stage.AddElement( new Table() );
            
            InitMainMenu(ref _tableMain);
            InitHowToPlayMenu(ref _tableHowToPlay);
            InitSettingsMenu(ref _tableSettings);
            
            _rootTable.Pack();

            _tableMain.SetPosition(Screen.MonitorWidth/2f, Screen.MonitorHeight/2f);
            _tableHowToPlay.SetPosition(-Screen.MonitorWidth/2f, Screen.MonitorHeight/2f);
            _tableSettings.SetPosition(Screen.MonitorWidth*1.5f, Screen.MonitorHeight/2f);
            
            
            MyGame.Instance.AudioManager.PlayMusic();
            
            AddPostProcessor(MyGame.Instance.VignettePostProcessor);
            AddPostProcessor(MyGame.Instance.BloomPostProcessor).Settings = BloomSettings.PresetSettings[6];
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
        private void InitMainMenu(ref Table table)
        {
            table = _rootTable.AddElement(new Table());
            
            table.SetFillParent( true );

            var title = new Label("Yet another snake", MyGame.Instance.Skin.Skin.Get<LabelStyle>("title-label"));
            table.Add(title);
            
            table.Row();

            var score = new Label($"High score: {MyGame.Instance.SaveSystem.SaveFile.Score}",
                MyGame.Instance.Skin.Skin.Get<LabelStyle>("label"));
            table.Add(score);

            var el = new Container();
            el.SetHeight(200);
            table.Add(el);
            table.Row();
            
            CreateBtn(table, "Play", button =>
            {
                Core.StartSceneTransition(new FadeTransition(()=>new GameScene()));
                RemovePostProcessor(MyGame.Instance.BloomPostProcessor);
                RemovePostProcessor(MyGame.Instance.VignettePostProcessor);
            });
            table.Row();
            CreateBtn(table, "Settings", button =>
            {
                Core.StartCoroutine(Coroutines.MoveToX(_rootTable, -Screen.MonitorWidth));
            });
            table.Row();
            CreateBtn(table, "How to play", button =>
            {
                Core.StartCoroutine(Coroutines.MoveToX(_rootTable, Screen.MonitorWidth));
            });
            table.Row();
            CreateBtn(table, "Exit", button =>
            {
                MyGame.Instance.AudioManager.Dispose();
                Environment.Exit(0);
            });
            table.Row();
            table.Pack();

        }

        private void InitHowToPlayMenu(ref Table table)
        {
            table = _rootTable.AddElement(new Table());
            
            table.SetFillParent( true );
            
            var title = new Label("How to Play", MyGame.Instance.Skin.Skin.Get<LabelStyle>("title-label"));
            title.SetFontScale(0.75f);
            table.Add(title);
            
            var el = new Container();
            el.SetHeight(200);
            table.Add(el);
            table.Row();

            const string infoText = @"
            Use the left and right arrow to rotate the snake head. 
                         Eat food and try not to crash. 
                    The more you eat, the more you become!
                ";
            var info = new Label(infoText, MyGame.Instance.Skin.Skin.Get<LabelStyle>("label"));
            table.Add(info);
            table.Row();
            CreateBtn(table, "Back", button =>
                {
                    Core.StartCoroutine(Coroutines.MoveToX(_rootTable, 0));
                });
            table.Row();
            table.Pack();
        }

        private void InitSettingsMenu(ref Table table)
        {
            table = _rootTable.AddElement(new Table());

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
                Core.StartCoroutine(Coroutines.MoveToX(_rootTable, 0));
            });
            table.Row();
            CreateBtn(table, "Back", button =>
            {
                Core.StartCoroutine(Coroutines.MoveToX(_rootTable, 0));
            });
            table.Row();
            table.Pack();
        }
    }
}