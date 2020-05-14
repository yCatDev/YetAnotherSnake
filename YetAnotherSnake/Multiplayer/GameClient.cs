using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
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
        private int _id;

        public bool Connected = false, GameStarted;

        public float PossibleWidth, PossibleHeight;
        
        private VirtualButton _rightArrow;
        private VirtualButton _leftArrow;
        private VirtualButton _escape;
        private bool _leftArrowWasPressed = false, _rightArrowWasPressed = false;
        //private Task<int> _reading; 
        
        public delegate void OnClient();

        private Timer _timer;
        
        public event OnClient onClient;
        public int[] SnakeIds;

        public GamePacket Packet;
        
        public GameClient()
        {
            //_id = (byte) Nez.Random.Range(100, 255);
            
            _escape = new VirtualButton();
            _escape.AddKeyboardKey(Keys.Escape);
            
        }

        public int Id => _id;
        
        public void InitClient(string address, int port)
        {
            _clientSocket = new TcpClient();
            _clientSocket.Connect(address, port);
            _serverStream = _clientSocket.GetStream();
            
            Connected = true;
            
            //_reading.Start();
            //_writing.Start();

            Packet = new GamePacket();
            
            _timer = new Timer {Interval = 10, AutoReset = true};
            _timer.Elapsed += (sender, args) =>
            {
                if (_serverStream.DataAvailable)
                {

                    var inStream = new byte[10025];
                    _serverStream.Read(inStream, 0, inStream.Length);
                    var data = GamePacket.FromBytes(inStream);
                    //if (data.Test) Console.WriteLine("Test packet recived");
                    if (data.ServiceData && data.StartGame)
                    {
                        //GameStarted = true;
                        _id = data.Id;
                        MenuScene.Instance.RemovePostProcessor(MyGame.GameInstance.BloomPostProcessor);
                        MenuScene.Instance.RemovePostProcessor(MyGame.GameInstance.VignettePostProcessor);
                        
                        SnakePositions = data.SnakePositions;
                        FoodPositions = data.FoodPositions;
                        SnakeIds = data.idsToCreate;
                        
                        Core.StartSceneTransition(new FadeTransition(()
                            => new GameScene()));
                    }
                    
                    
                    if (!data.ServiceData && GameScene.Instance != null)
                        GameScene.Instance.ProcessData(data.Id, data);
                }
                
                if (!Connected) return;

                if (GameStarted)
                {
                    Packet.Id = _id;
                    Packet.SnakeHeadPosition = (GameScene.Instance.Snakes[_id].SnakeHead.Position.X,
                        GameScene.Instance.Snakes[_id].SnakeHead.Position.Y);
                    
                        
                    SendData(Packet);                    
                }
            };
            _timer.Start();
            
            onClient?.Invoke();
        }

        public (float, float)[] FoodPositions;

        public (float, float)[] SnakePositions;

        public void Disconnect()
        {
           // _reading.Wait();
            SendData(new GamePacket()
            {
                ServiceData = true,
                Disconnect = true
            });
            Dispose();
            onClient?.Invoke();
        }

        public void SpawnFood()
        {
            var possibleWidth = 1280;
            var possibleHeight = 720;
            Packet.SpawnFood = true;
            Packet.NextFoodPosition = (Random.Range(-possibleWidth, possibleWidth),
                Random.Range(-possibleHeight, possibleHeight));
            //SendData(packet);
        }
        
        public void SendData(GamePacket data)
        {
            _serverStream = _clientSocket.GetStream();
            var outStream = GamePacket.GetBytes(data);
            _serverStream.Write(outStream, 0, outStream.Length);
            
            _serverStream.Flush();
        }


        public void Dispose()
        {
            Connected = false;
            _clientSocket?.Dispose();
            _serverStream?.Dispose();
        }
    }
}