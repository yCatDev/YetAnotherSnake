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
using YetAnotherSnake.Multiplayer;
using YetAnotherSnake.UI;

namespace YetAnotherSnake.Scenes
{
    public class GameScene: Scene
    {
        /// <summary>
        /// Default snake length 
        /// </summary>
        private const int SnakeSize = 10;
        
        /// <summary>
        /// Background grid object
        /// </summary>
        private Entity _gridEntity;

        public static GameScene Instance;

        public Dictionary<int, Snake> Snakes { get; private set; }
        
        
        /// <summary>
        /// Pause key listener
        /// </summary>
        private VirtualButton _pauseKey;
        
        /// <summary>
        /// UI tables
        /// </summary>
        private Table _pauseTable, _rootTable, _settingsTable;

        private GameUIHelper _uiHelper;

        private float width, height;

        private bool _started = false;

        public FoodSpawner FoodSpawner;

        public override void Initialize()
        {
            base.Initialize();
            ClearColor = Color.Black;
            SetDesignResolution(Screen.MonitorWidth,Screen.MonitorHeight,SceneResolutionPolicy.ShowAll);
            Instance = this;
            
            Console.WriteLine($"Connected {MyGame.GameInstance.GameClient.Snakes.Count} clients");
            width = 1280;
            height = 720;
            Camera.AddComponent(new CameraBounds(new Vector2(-width, -height),new Vector2(width, height)));
            
            //Set up listener for Escape and Enter key
            _pauseKey = new VirtualButton();
            _pauseKey.AddKeyboardKey(Keys.Escape).AddKeyboardKey(Keys.Enter);

            Snakes = new Dictionary<int, Snake>(1);
            FoodSpawner = AddSceneComponent<FoodSpawner>();
            
            //Enabling post-processing
            AddPostProcessor(MyGame.GameInstance.VignettePostProcessor);
            AddPostProcessor(MyGame.GameInstance.BloomPostProcessor).Settings = BloomSettings.PresetSettings[6];
            MyGame.GameInstance.GameClient.GameStarted = true;
        }

        public override void OnStart()
        {
            base.OnStart();
            
            //Creating background grid
            _gridEntity  = CreateEntity("grid");
            _gridEntity.AddComponent(new SpringGrid(new Rectangle((int) -width, (int) -height, (int) (width+1280), (int) (height+720)), new Vector2(30))
            {
                GridMinorThickness = 0f,
                GridMajorThickness = 8,
                GridMajorColor = new Color(61,9,107)
            });
            _gridEntity.GetComponent<SpringGrid>().RenderLayer = 9999;
            

            //Create text label for displaying score
            var score = CreateEntity("scoreText");
            score.AddComponent<TextComponent>().AddComponent<ScoreDisplay>();
            
            foreach (var item in MyGame.GameInstance.GameClient.Snakes)
            {
                var id = item.Key;
                
                var snake = CreateEntity("SnakeHead" + id);
                //snake.Position = item.Value.ToVector2();
                Console.WriteLine($"Snake id: {id}, client id: {MyGame.GameInstance.GameClient.Id}");
                var direction = (Vector2.Zero - item.Value.ToVector2());
                direction.Normalize();
                var s = AddSceneComponent(new Snake(
                    id == MyGame.GameInstance.GameClient.Id, 
                    SnakeSize,
                    item.Value.ToVector2(),
                    direction*12
                    ));
                s.SnakeHead.Position = item.Value.ToVector2();
                Snakes.Add(id, s);

                //var p = MyGame.GameInstance.GameClient.FoodPositions[i];
                //FoodSpawner.CreateFood(new Vector2(p.Item1, p.Item2));
            }

            CreateUI();
            _isReady = true;
        }

        public void Start()
        {
            _started = true;
        }

        private GamePacket _lastPacket;
        private bool _isReady = false;

        /*
        public void ProcessData(int id, GamePacket data)
        {
            if (data == _lastPacket) return;
            if (!_isReady) return;
            //Console.WriteLine($"Recived data from {id}");
            //if (!data.StartGame) return;
            if (Snakes.ContainsKey(id))
            {
                if (MyGame.GameInstance.GameClient.Id != id)
                {
                    Snakes[id].SnakeHead.Position = Utils.Move(Snakes[id].SnakeHead.Position,
                        new Vector2(data.SnakeHeadPosition.Item1, data.SnakeHeadPosition.Item2), 10f);
                    
                }
                //Console.WriteLine($"ID ({id}): {data.SnakeHeadPosition.Item1} {data.SnakeHeadPosition.Item2}");
                //Console.WriteLine($"Recived packet from {id} to {MyGame.GameInstance.GameClient.Id}");

                if (data.SpawnFood)
                {
                    Console.WriteLine("Food spawned");
                    FoodSpawner.CreateFood(new Vector2(data.NextFoodPosition.Item1, data.NextFoodPosition.Item2));
                    MyGame.GameInstance.GameClient.Packet.SpawnFood = false;
                }
            }

            _lastPacket = data;
            //MyGame.GameInstance.GameServer.SyncData(id,data);
        }
        */
        
        #region UI
        
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
                    
                _gridEntity.UpdateInterval = 1;
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
                //_snakeController.Die();
                _gridEntity.UpdateInterval = 1;
                
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
            
            var cFullScreen = _uiHelper.CreateCheckBox(table, "FullScreen", MyGame.GameInstance.SaveSystem.SaveFile.IsFullScreen,b =>
            {
                /*if (b)
                    Screen.SetSize(Screen.MonitorWidth, Screen.MonitorHeight);
                else
                {
                    Screen.SetSize((int) (Screen.MonitorWidth*0.75f), (int) (Screen.MonitorHeight*0.75f));
                }
                MyGame.GameInstance.SaveSystem.SaveFile.IsFullScreen = b;
                Screen.IsFullscreen = b;*/
                
            });
            table.Row();
            
            var cVignette = _uiHelper.CreateCheckBox(table, "Enable vignette", MyGame.GameInstance.SaveSystem.SaveFile.IsVignette, b =>
            {
                /*MyGame.GameInstance.SaveSystem.SaveFile.IsVignette = b;
                MyGame.GameInstance.VignettePostProcessor.Enabled = b;*/
            });
            table.Row();
            
            var cBloom = _uiHelper.CreateCheckBox(table, "Enable bloom", MyGame.GameInstance.SaveSystem.SaveFile.IsBloom, b =>
            {
                /*MyGame.GameInstance.SaveSystem.SaveFile.IsBloom = b;
                MyGame.GameInstance.BloomPostProcessor.Enabled = b;*/
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
                //Core.StartCoroutine(UIAnimations.MoveToY(_rootTable, -Screen.MonitorHeight));
                MyGame.GameInstance.SaveSystem.SaveChanges();
                
            });
            table.Row();
            _uiHelper.CreateBtn(table, "Back", button =>
            {
                cBloom.IsChecked = MyGame.GameInstance.SaveSystem.SaveFile.IsBloom;
                cVignette.IsChecked = MyGame.GameInstance.SaveSystem.SaveFile.IsVignette;
                cFullScreen.IsChecked = MyGame.GameInstance.SaveSystem.SaveFile.IsFullScreen;
                Core.StartCoroutine(UIAnimations.MoveToY(_rootTable, -Screen.MonitorHeight));
            });
            table.Row();
            table.Pack();

            return table;
        }

         #endregion

     
         
        public override void Update()
        {
            base.Update();
            /*if (_pauseKey.IsPressed && _snakeController.IsAlive)
            {
                Core.StartCoroutine(UIAnimations.MoveToY(_rootTable, -Screen.MonitorHeight));
                MyGame.GameInstance.Pause = true;
                _gridEntity.UpdateInterval = 9999;
                MyGame.GameInstance.AudioManager.PauseMusic();
            }*/

        
        }
    }
    
}