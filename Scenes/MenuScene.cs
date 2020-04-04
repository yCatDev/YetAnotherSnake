using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez;
using Nez.BitmapFonts;
using Nez.UI;
using YetAnotherSnake.Components;

namespace YetAnotherSnake.Scenes
{
    public class MenuScene: Scene
    {
        public override void Initialize()
        {
            base.Initialize();
            ClearColor = Color.Black;
            
            SetDesignResolution(Screen.MonitorWidth,Screen.MonitorHeight,SceneResolutionPolicy.ShowAll);
            
            /*var text = CreateEntity("MenuText").AddComponent<MenuTest>().AddComponent<TextComponent>();
            text.SetFont(new NezSpriteFont(Content.Load<SpriteFont>(YetAnotherSnake.Content.OswaldFont)));
            text.Text = "Yet another snake";*/
            var skin = new GameSkin(Content);
            
            var gridEntity = CreateEntity("grid");
            gridEntity.AddComponent(new SpringGrid(new Rectangle(0, 0, 2560, 1440), new Vector2(30))
            {
                GridMinorThickness = 0f,
                GridMajorThickness = 8,
                GridMajorColor = new Color(61,9,107)
            });
            gridEntity.GetComponent<SpringGrid>().RenderLayer = 9999;
            
            var ui = CreateEntity("UI").AddComponent<UICanvas>();
            var table = ui.Stage.AddElement( new Table() );
            
            table.SetFillParent( true );

            var title = new Label("Yet another snake", skin.Skin.Get<LabelStyle>("title-label"));
            table.Add(title);
            
            table.Row();

            var score = new Label("High score: 0000", skin.Skin.Get<LabelStyle>("label"));
            table.Add(score);

            var el = new Nez.UI.Container();
            el.SetHeight(200);
            table.Add(el);
            table.Row();
            
            var playBtn = CreateBtn(table, "Play", button =>
            {
                Core.StartSceneTransition(new FadeTransition(()=>new GameScene()));
            });
            table.Row();
            var settingsBtn = CreateBtn(table, "Settings", button => { });
            table.Row();
            var howToPlayBtn = CreateBtn(table, "How to play", button =>
            {
                Dialog m = new Dialog("How to play", Skin.CreateDefaultSkin());
                m.AddText("Test text Test text Test text Test text ");
                m.Show(ui.Stage);
                m.SetSize(500,500);
            });
            table.Row();
            var exitBtn = CreateBtn(table, "Exit", button =>
            {
                MyGame.AudioManager.Dispose();
                Environment.Exit(0);
            });
            table.Row();
            
            table.Pack();
            //table.SetDebug(true);
        }

        private TextButton CreateBtn(Table t, string label, Action<Button> action)
        {
            var button = new TextButton(label,TextButtonStyle.Create( Color.Black, new Color(61,9,85), new Color(61,9,107) ) );
            button.GetLabel().SetStyle(MyGame.Skin.Skin.Get<LabelStyle>("label"));
            button.OnClicked += action;
            
            t.Add( button ).SetMinWidth( 400 ).SetMinHeight( 100 );
            

            return button;
        }
        
    }
}