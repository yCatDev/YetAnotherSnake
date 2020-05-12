using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez;
using Nez.BitmapFonts;
using Nez.UI;
using YetAnotherSnake.Components;
using YetAnotherSnake.UI;

namespace YetAnotherSnake.Scenes
{
    public class MenuScene : Scene
    {
        private GameUIHelper _uiHelper;
        private Table _rootTable, _tableMain, _tableSettings, _tableHowToPlay, _tableMultipayer, _tableMultipayerServer,
            _tableMultipayerClient;

        public override void Initialize()
        {
            base.Initialize();
            ClearColor = Color.Black;

            SetDesignResolution(Screen.MonitorWidth, Screen.MonitorHeight, SceneResolutionPolicy.ShowAll);

            var gridEntity = CreateEntity("grid");
            gridEntity.AddComponent(new SpringGrid(new Rectangle(0, 0, 2560, 1440), new Vector2(30))
            {
                GridMinorThickness = 0f,
                GridMajorThickness = 8,
                GridMajorColor = new Color(61, 9, 107)
            });
            gridEntity.AddComponent(new MenuGrid(3500));
            gridEntity.GetComponent<SpringGrid>().RenderLayer = 9999;

            //Creating UI
            CreateUI();

            MyGame.GameInstance.AudioManager.PlayMusic();

            AddPostProcessor(MyGame.GameInstance.VignettePostProcessor);
            AddPostProcessor(MyGame.GameInstance.BloomPostProcessor).Settings = BloomSettings.PresetSettings[6];
        }


        private void CreateUI()
        {
            var ui = CreateEntity("UI").AddComponent<UICanvas>();
            _rootTable = ui.Stage.AddElement(new Table());
            _uiHelper = new GameUIHelper(Content);

            _tableMain = InitMainMenu();
            _tableHowToPlay = InitHowToPlayMenu();
            _tableSettings = InitSettingsMenu();
            
            _tableMultipayer = InitMultiplayerMenu();
            _tableMultipayerClient = InitMultiplayerClientMenu();
            _tableMultipayerServer = InitMultiplayerServerMenu();

            _rootTable.Pack();

            _tableMain.SetPosition(Screen.MonitorWidth / 2f, Screen.MonitorHeight / 2f);
            _tableHowToPlay.SetPosition(-Screen.MonitorWidth / 2f, Screen.MonitorHeight / 2f);
            _tableSettings.SetPosition(Screen.MonitorWidth * 1.5f, Screen.MonitorHeight / 2f);
            _tableMultipayer.SetPosition(Screen.MonitorWidth / 2f, Screen.MonitorHeight*1.5f);
            _tableMultipayerServer.SetPosition(-Screen.MonitorWidth /2f, Screen.MonitorHeight*1.5f);
            _tableMultipayerClient.SetPosition(Screen.MonitorWidth *1.5f, Screen.MonitorHeight*1.5f);
        }

        private Table InitMultiplayerServerMenu()
        {
            var table = _rootTable.AddElement(new Table());

            _uiHelper.CreateTitleLabel(table, "Create room");
            _uiHelper.CreateVerticalIndent(table, 200);
            table.Row();
            _uiHelper.CreateRegularLabel(table, $"Server address: {MyGame.GameInstance.GameServer.Address}");
                                                table.Row();
            _uiHelper.CreateRegularLabel(table, $"Server port {"8888"}");
            table.Row();
            var connectedLabel = _uiHelper.CreateRegularLabel(table, $"Ready players: {MyGame.GameInstance.GameServer.ConnectedCount}");
            _uiHelper.CreateVerticalIndent(table, 300);
            table.Row();

            MyGame.GameInstance.GameServer.ConnectEvent += () =>
            {
                connectedLabel.SetText($"Ready players: {MyGame.GameInstance.GameServer.ConnectedCount}");
            };
            
            _uiHelper.CreateBtn(table, "Start game", (b) => { });
            table.Row();
            _uiHelper.CreateBtn(table, "Cancel", (b) =>
                { Core.StartCoroutine(UIAnimations.MoveToX(_rootTable, 0));  });
            table.Row();
            
            return table;
        }

        private Table InitMultiplayerClientMenu()
        {
            var table = _rootTable.AddElement(new Table());

            _uiHelper.CreateTitleLabel(table, "Connect to room");
            _uiHelper.CreateVerticalIndent(table, 100);
            table.Row();
            _uiHelper.CreateRegularLabel(table, "Enter server address");
            table.Row();
            var address = _uiHelper.CreateInputField(table, MyGame.GameInstance.GameServer.Address);
            table.Row();
            _uiHelper.CreateRegularLabel(table, "Enter server port");
            table.Row();
            _uiHelper.CreateInputField(table, "8888");
            table.Row();
            
            _uiHelper.CreateVerticalIndent(table, 100);
            table.Row();
            var connectedLabel = _uiHelper.CreateRegularLabel(table, $"Ready: {"No"}");
            table.Row();

            _uiHelper.CreateBtn(table, "Ready/Cancel", (b) =>
            {
                if (!MyGame.GameInstance.GameClient.Connected)
                {
                    MyGame.GameInstance.GameClient.InitClient(address.GetText(), 8888);
                    b.SetText("Cancel");
                    connectedLabel.SetText($"Ready: Yes");
                }
                else
                {
                    MyGame.GameInstance.GameClient.Disconnect();
                    b.SetText("Ready");
                    connectedLabel.SetText($"Ready: No");
                }
            });
            table.Row();
            _uiHelper.CreateBtn(table, "Back", (b) => {Core.StartCoroutine(UIAnimations.MoveToX(_rootTable, 0)); });
            table.Row();
            
            return table;
        }

        private Table InitMultiplayerMenu()
        {
            var table = _rootTable.AddElement(new Table());

            _uiHelper.CreateTitleLabel(table, "Multiplayer mode");
            _uiHelper.CreateVerticalIndent(table, 400);
            table.Row();
            _uiHelper.CreateBtn(table, "Create room", (b) =>
                { Core.StartCoroutine(UIAnimations.MoveToX(_rootTable, Screen.MonitorWidth)); });
            table.Row();
            _uiHelper.CreateBtn(table, "Connect to room", (b) => 
                { Core.StartCoroutine(UIAnimations.MoveToX(_rootTable, -Screen.MonitorWidth));});
            table.Row();
            _uiHelper.CreateBtn(table, "Back", (b) =>
                { Core.StartCoroutine(UIAnimations.MoveToY(_rootTable, 0));});
            table.Row();
            
            return table;
        }
        
        private Table InitMainMenu()
        {
            var table = _rootTable.AddElement(new Table());

            table.SetFillParent(true);
            _uiHelper.CreateTitleLabel(table, "Yet another snake");
            table.Row();
            _uiHelper.CreateRegularLabel(table, $"High score: {MyGame.GameInstance.SaveSystem.SaveFile.Score}");
            _uiHelper.CreateVerticalIndent(table, 200);
            table.Row();

            _uiHelper.CreateBtn(table, "Play", button =>
            {
                MyGame.GameInstance.GameClient.InitClient(MyGame.GameInstance.GameServer.Address, 8888);
                Core.StartSceneTransition(new FadeTransition(() => new GameScene()));
                RemovePostProcessor(MyGame.GameInstance.BloomPostProcessor);
                RemovePostProcessor(MyGame.GameInstance.VignettePostProcessor);
            });
            
            table.Row();
            _uiHelper.CreateBtn(table, "Play with friends", button =>
            {
                Core.StartCoroutine(UIAnimations.MoveToY(_rootTable, -Screen.MonitorHeight));
            });
            table.Row();
            _uiHelper.CreateBtn(table, "Settings",
                button => { Core.StartCoroutine(UIAnimations.MoveToX(_rootTable, -Screen.MonitorWidth)); });
            table.Row();
            _uiHelper.CreateBtn(table, "How to play",
                button => { Core.StartCoroutine(UIAnimations.MoveToX(_rootTable, Screen.MonitorWidth)); });
            table.Row();
            _uiHelper.CreateBtn(table, "Exit", button =>
            {
                MyGame.GameInstance.Quit();
            });
            table.Row();
            table.Pack();

            return table;
        }

        private Table InitHowToPlayMenu()
        {
            var table = _rootTable.AddElement(new Table());

            table.SetFillParent(true);

            _uiHelper.CreateTitleLabel(table, "How to Play").SetFontScale(0.75f);

            var el = new Container();
            el.SetHeight(200);
            table.Add(el);
            table.Row();


            _uiHelper.CreateRegularLabel(table, YetAnotherSnake.Content.HowToPlayText);
            table.Row();
            _uiHelper.CreateBtn(table, "Back", button => { Core.StartCoroutine(UIAnimations.MoveToX(_rootTable, 0)); });
            table.Row();
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

            var el2 = new Container();
            el2.SetHeight(100);
            table.Add(el2);
            table.Row();


            var cFullScreen = _uiHelper.CreateCheckBox(table, "FullScreen",
                MyGame.GameInstance.SaveSystem.SaveFile.IsFullScreen, b =>
                {
                    /*if (b)
                        Screen.SetSize(Screen.MonitorWidth, Screen.MonitorHeight);
                    else
                    {
                        Screen.SetSize((int) (Screen.MonitorWidth*0.75f), (int) (Screen.MonitorHeight*0.75f));
                    }*/
                    //MyGame.GameInstance.SaveSystem.SaveFile.IsFullScreen = b;
                    //Screen.IsFullscreen = b;
                });
            table.Row();

            var cVignette = _uiHelper.CreateCheckBox(table, "Enable vignette",
                MyGame.GameInstance.SaveSystem.SaveFile.IsVignette, b =>
                {
                    //MyGame.GameInstance.SaveSystem.SaveFile.IsVignette = b;
                    //MyGame.GameInstance.VignettePostProcessor.Enabled = b;
                });
            table.Row();

            var cBloom = _uiHelper.CreateCheckBox(table, "Enable bloom",
                MyGame.GameInstance.SaveSystem.SaveFile.IsBloom, b =>
                {
                    //MyGame.GameInstance.SaveSystem.SaveFile.IsBloom = b;
                    //MyGame.GameInstance.BloomPostProcessor.Enabled = b;
                });
            table.Row();


            _uiHelper.CreateVerticalIndent(table, 100);
            table.Row();

            _uiHelper.CreateBtn(table, "Apply", button =>
            {
                var b = cFullScreen.IsChecked;
                if (b)
                    Screen.SetSize(Screen.MonitorWidth, Screen.MonitorHeight);
                else
                {
                    Screen.SetSize((int) (Screen.MonitorWidth * 0.75f), (int) (Screen.MonitorHeight * 0.75f));
                }

                MyGame.GameInstance.SaveSystem.SaveFile.IsFullScreen = b;
                Screen.IsFullscreen = b;

                b = cVignette.IsChecked;
                MyGame.GameInstance.SaveSystem.SaveFile.IsVignette = b;
                MyGame.GameInstance.VignettePostProcessor.Enabled = b;

                b = cBloom.IsChecked;
                MyGame.GameInstance.SaveSystem.SaveFile.IsBloom = b;
                MyGame.GameInstance.BloomPostProcessor.Enabled = b;
                //Core.StartCoroutine(UIAnimations.MoveToX(_rootTable, 0));
                MyGame.GameInstance.SaveSystem.SaveChanges();
            });
            table.Row();
            _uiHelper.CreateBtn(table, "Back", button =>
            {
                cBloom.IsChecked = MyGame.GameInstance.SaveSystem.SaveFile.IsBloom;
                cVignette.IsChecked = MyGame.GameInstance.SaveSystem.SaveFile.IsVignette;
                cFullScreen.IsChecked = MyGame.GameInstance.SaveSystem.SaveFile.IsFullScreen;
                Core.StartCoroutine(UIAnimations.MoveToX(_rootTable, 0));
            });
            table.Row();
            table.Pack();

            return table;
        }
    }
}