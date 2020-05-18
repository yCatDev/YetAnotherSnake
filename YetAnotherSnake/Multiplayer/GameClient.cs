using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public delegate void ClientChanged();
        public Dictionary<int, NetworkVector> Snakes { get; private set; }
        
        private Timer _writeTimer;
        private Thread _readTask;
        
        public event ClientChanged OnClient;
        private GamePacket _writePacket, _readPacket;

        private INetworkScene _currnetScene;
        
        public int Id { get; private set; }

        public bool InitClient(string address, int port)
        {
            _clientSocket = new TcpClient();
            //_clientSocket.Connect(address, port);
            var ct = _clientSocket.ConnectAsync(address, port);
            Stopwatch clock = new Stopwatch();
            clock.Start();
            while (!ct.IsCompleted)
            {
                if (clock.ElapsedMilliseconds>500)
                    return false;
            }
            clock.Stop();

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
            return true;
        }

        private void ReadPacketOnOnMoveSnakeReceived(MoveSnakePacket received)
        {
            if (received.ClientId!=Id)
               _currnetScene.SetSnakePosition(received.ClientId, received.SnakeMarkerPosition, received.SyncDelta);
        }

        private void ReadPacketOnOnPauseReceived(PauseGamePacket received)
        {
            if (received.PauseState)
                _currnetScene.Pause();
            else
                _currnetScene.UnPause();
        }

        private void ReadPacketOnOnSpawnFoodReceived(SpawnFoodPacket received)
        {
            //if (received.ClientId!=Id)
            if (_currnetScene.IsReady)
                _currnetScene.CreateFood(received.NextFoodPosition.ToVector2(), Vector2.Zero);
        }

        private void ReceivePacketFromServer()
        {
            
            while (Connected)
            {
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
            /*if (packet.TargetFrameRate > MyGame.GameInstance.TargetFrameRate.TotalMilliseconds)
                MyGame.GameInstance.TargetFrameRate = TimeSpan.FromMilliseconds(packet.TargetFrameRate);*/
            //MyGame.GameInstance.TargetFrameRate = TimeSpan.FromSeconds(1d / 30d);
            if (packet.Classic)
            {
                Core.StartSceneTransition(new FadeTransition(() =>
                {
                    var target = new GameClassicScene();
                    _currnetScene = target;
                    return target;
                }));
            }
            else
            {
                Core.StartSceneTransition(new FadeTransition(() =>
                {
                    var target = new GameTimeAttackScene();
                    _currnetScene = target;
                    return target;
                }));
            }

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
           Console.WriteLine($"[SERVER] Disconnect client ({Id})");
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
            Console.WriteLine($"[SERVER] Spawn food due to client ({Id})");
            _writePacket.AddPacket(Protocol.SpawnFood, new SpawnFoodPacket()
            {
                ClientId = Id,
                NextFoodPosition = foodPos
            });
        }
        
        public void SetPaused(bool paused)
        {
            Console.WriteLine($"[CLIENT] Client ({Id}) set pause to {paused}");
            _writePacket.AddPacket(Protocol.Pause, new PauseGamePacket()
            {
                PauseState = paused
            });
        }

        public static bool CheckAddress(string address)
        {
            var mask = "###.###.#.###";
            for (var i = 0; i < address.Length; i++)
            {
                var c = address[i];
                if (mask[i] == '#')
                {
                    if (!char.IsNumber(c))
                        return false;
                }
                else
                {
                    if (c!='.')
                        return false;
                }
            }
            return true;
        }


        public void Dispose()
        {
            Connected = false;
            _writeTimer?.Dispose();
            _clientSocket?.Dispose();
            _serverStream?.Dispose();
        }
    }
}