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
        private Table pauseTable;
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
            AddRenderer(new RenderLayerExcludeRenderer(0, 9990));
            var ui = CreateEntity("UI").AddComponent<UICanvas>();
            ui.IsFullScreen = false;
            ui.RenderLayer = 9990;
            pauseTable = ui.Stage.AddElement( new Table() );
            pauseTable.SetFillParent( true );
            
            var title = new Label("PAUSE", MyGame.Instance.Skin.Skin.Get<LabelStyle>("title-label"));
            pauseTable.Add(title);
            pauseTable.Row();
            
            var el = new Container();
            el.SetHeight(200);
            pauseTable.Add(el);
            pauseTable.Row();

            CreateBtn(pauseTable, "Continue", button =>
                {
                    Core.StartCoroutine(Coroutines.MoveToY(pauseTable, -Screen.MonitorHeight));
                    
                    MyGame.Instance.Pause = false;
                });
            pauseTable.Row();
            CreateBtn(pauseTable, "Exit", button =>
            {
                MyGame.Instance.Pause = false;
                
                Core.StartCoroutine(Coroutines.MoveToY(pauseTable, -Screen.MonitorHeight));
                _snake.GetComponent<Snake>().Die();
                
            });

            pauseTable.SetY(-Screen.MonitorHeight);
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
                Core.StartCoroutine(Coroutines.MoveToY(pauseTable, 0));
                MyGame.Instance.Pause = true;
            }

            if (MyGame.Instance.Pause)
            {
                gridEntity.UpdateInterval = 9999;
            }
            else
            {
                gridEntity.UpdateInterval = 1;
            }
        }
    }
    
}