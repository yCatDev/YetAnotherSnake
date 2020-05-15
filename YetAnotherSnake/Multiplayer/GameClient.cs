using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Nez;
using YetAnotherSnake.Scenes;
using Random = Nez.Random;
using Timer = System.Timers.Timer;

namespace YetAnotherSnake.Multiplayer
{
    public class GameClient : IDisposable
    {
        private TcpClient _clientSocket;
        private NetworkStream _serverStream;

        public bool Connected = false, GameStarted;

        public float PossibleWidth, PossibleHeight;
        
        //private Task<int> _reading; 
        
        public delegate void ClientChanged();
        public Dictionary<int, NetworkVector> Snakes { get; private set; }
        
        
        private Timer _writeTimer;
        private Thread _readTask;
        
        public event ClientChanged OnClient;

        private GamePacket _writePacket, _readPacket;
        

        public int Id { get; private set; }

        public void InitClient(string address, int port)
        {
            _clientSocket = new TcpClient();
            _clientSocket.Connect(address, port);
            _serverStream = _clientSocket.GetStream();
            
            Connected = true;
            

            _writePacket = new GamePacket();
            _readPacket = new GamePacket();
            
            _readPacket.OnStartGameReceived += OnGameStartReceived;
            _readPacket.OnSpawnFoodReceived += ReadPacketOnOnSpawnFoodReceived;
            _readPacket.OnPauseReceived += ReadPacketOnOnPauseReceived;
            _readPacket.OnMoveSnakeReceived += ReadPacketOnOnMoveSnakeReceived;
            
            _writeTimer = new Timer {Interval = 10, AutoReset = true};
            _writeTimer.Elapsed += (sender, args) => SendDataToServer();
            _writeTimer.Start();

            _readTask = new Thread(ReceivePacketFromServer);
            _readTask.Start();
            
            OnClient?.Invoke();
        }

        private void ReadPacketOnOnMoveSnakeReceived(MoveSnakePacket received)
        {
            if (received.ClientId!=Id)
                GameScene.Instance.SetSnakePosition(received.ClientId, received.SnakeMarkerPosition, received.SyncDelta);
        }

        private void ReadPacketOnOnPauseReceived(PauseGamePacket received)
        {
            if (received.PauseState)
                GameScene.Instance.Pause();
            else
                GameScene.Instance.UnPause();
        }

        private void ReadPacketOnOnSpawnFoodReceived(SpawnFoodPacket received)
        {
            //if (received.ClientId!=Id)
                GameScene.Instance.FoodSpawner.CreateFood(received.NextFoodPosition.ToVector2());
        }

        private void ReceivePacketFromServer()
        {
            
            while (true)
            {
                if (!Connected) continue;
                if (!_serverStream.DataAvailable) continue;

                var inStream = new byte[10025];
                _serverStream.Read(inStream, 0, inStream.Length);
                try
                {
                    _readPacket.FromBytes(inStream);
                    _readPacket.ProcessAll();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error reading packet, skipping \n-{ex.Message}\n-{ex.StackTrace}\n -END");
                }
                
            }
        }
        private void SendDataToServer()
        {
            //if (!GameStarted) return;
            if (!Connected) return;
            if (_writePacket.IsEmpty()) return;
            
            _serverStream = _clientSocket.GetStream();
            var outStream = _writePacket.ToBytes();
            _serverStream.Write(outStream, 0, outStream.Length);
            _serverStream.Flush();
            //Console.WriteLine($"Send {_writePacket.Count} packages");
            _writePacket.Clear();
        }
        
        private void OnGameStartReceived(StartGamePacket packet)
        {
            Id = packet.GeneratedId;
            MenuScene.Instance.RemovePostProcessor(MyGame.GameInstance.BloomPostProcessor);
            MenuScene.Instance.RemovePostProcessor(MyGame.GameInstance.VignettePostProcessor);

            Snakes = packet.SnakePositions;


            Core.StartSceneTransition(new FadeTransition(() =>
            {
                var target = new GameScene();
                SpawnFood();
                return target;
            }));

        }

        public void SendSnakePosition(Vector2 position)
        {
            _writePacket.AddPacket(Protocol.MoveSnake, new MoveSnakePacket()
            {
                ClientId = Id,
                SnakeMarkerPosition = position.ToNetworkVector(),
                SyncDelta = Time.DeltaTime
            });
        }

        

        public void Disconnect()
        {
           // _reading.Wait();
           Console.WriteLine($"Disconnect {Id}");
           _writePacket.AddPacket(Protocol.Disconnect, new DisconnectGamePacket()
           {
              ClientId = Id
           });
           Thread.Sleep(200);
           Dispose();
           OnClient?.Invoke();
        }

        public void SpawnFood()
        {
            var foodPos = MyGame.CreateRandomPositionInWindowSpace().ToNetworkVector();
            Console.WriteLine($"Spawn food from {Id}");
            _writePacket.AddPacket(Protocol.SpawnFood, new SpawnFoodPacket()
            {
                ClientId = Id,
                NextFoodPosition = foodPos
            });
        }
        
        public void SetPaused(bool paused)
        {
            Console.WriteLine($"Client {Id} set pause to {paused}");
            _writePacket.AddPacket(Protocol.Pause, new PauseGamePacket()
            {
                PauseState = paused
            });
        }
        
        


        public void Dispose()
        {
            Connected = false;
            _writeTimer.Dispose();
            _clientSocket?.Dispose();
            _serverStream?.Dispose();
        }
    }
}